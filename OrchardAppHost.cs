using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Autofac;
using Autofac.Builder;
using Autofac.Core;
using Lombiq.OrchardAppHost.Configuration;
using Lombiq.OrchardAppHost.Environment;
using Orchard;
using Orchard.Caching;
using Orchard.Environment;
using Orchard.Environment.Configuration;
using Orchard.Environment.Descriptor;
using Orchard.Environment.Descriptor.Models;
using Orchard.Environment.Extensions.Folders;
using Orchard.Environment.Extensions.Loaders;
using Orchard.Environment.Features;
using Orchard.Environment.State;
using Orchard.Events;
using Orchard.Exceptions;
using Orchard.FileSystems.AppData;
using Orchard.FileSystems.Dependencies;
using Orchard.FileSystems.VirtualPath;
using Orchard.FileSystems.WebSite;
using Orchard.Logging;
using Orchard.Mvc;

namespace Lombiq.OrchardAppHost
{
    public class OrchardAppHost : IOrchardAppHost
    {
        private const string RunScopeTag = "OrchardAppHostRunScope";

        private readonly AppHostSettings _settings;
        private readonly AppHostRegistrations _registrations;
        private IContainer _hostContainer = null;


        // Having an overload without "registrations" enables calling it without the caller having a reference to Autofac.
        public OrchardAppHost(AppHostSettings settings)
            : this(settings, null)
        {
        }

        public OrchardAppHost(
            AppHostSettings settings,
            AppHostRegistrations registrations)
        {
            if (settings == null) settings = new AppHostSettings();
            if (registrations == null) registrations = new AppHostRegistrations();

            _settings = settings;
            _registrations = registrations;
        }


        public async Task Startup()
        {
            // Trying to load not found assemblies from the Dependencies folder. This is instead of the assemblyBinding config
            // in Orchard's Web.config.
            AppDomain.CurrentDomain.AssemblyResolve += (object sender, ResolveEventArgs args) =>
            {
                // Don't run if the host container is not initialized or was disposed.
                if (_hostContainer == null) return null;

                var assemblyName = args.Name.ToAssemblyShortName();
                var dependenciesFolder = _hostContainer.Resolve<IDependenciesFolder>();
                var path = string.Empty;

                var dependency = dependenciesFolder.GetDescriptor(assemblyName);
                if (dependency != null) path = dependency.VirtualPath;
                else if (args.RequestingAssembly != null)
                {
                    dependency = dependenciesFolder.GetDescriptor(args.RequestingAssembly.ShortName());
                    if (dependency != null)
                    {
                        var reference = dependency.References.SingleOrDefault(r => r.Name == assemblyName);
                        if (reference != null) path = reference.VirtualPath;
                    }
                }

                if (!string.IsNullOrEmpty(path)) return Assembly.LoadFile(path);
                return null;
            };

            // Automatically importing OrchardAppHost assemblies.
            if (_settings.ImportedExtensions == null) _settings.ImportedExtensions = Enumerable.Empty<Assembly>();
            _settings.ImportedExtensions = _settings.ImportedExtensions.Union(new[] { this.GetType().Assembly });

            // Automatically enabling OrchardAppHost root feature to register common dependencies.
            if (_settings.DefaultShellFeatureStates == null) _settings.DefaultShellFeatureStates = Enumerable.Empty<DefaultShellFeatureState>();
            if (!_settings.DefaultShellFeatureStates.Any(state => state.ShellName == ShellSettings.DefaultName))
            {
                _settings.DefaultShellFeatureStates = _settings.DefaultShellFeatureStates.Union(new[]
                {
                    new DefaultShellFeatureState { ShellName = ShellSettings.DefaultName }
                });
            }
            foreach (var featureState in _settings.DefaultShellFeatureStates)
            {
                featureState.EnabledFeatures = featureState.EnabledFeatures.Union(new[] { "Lombiq.OrchardAppHost" });
            }

            _hostContainer = HostContainerFactory.CreateHostContainer(this, _settings, _registrations);

            _hostContainer.Resolve<IOrchardHost>().Initialize();

            if (_settings.DefaultShellFeatureStates != null && _settings.DefaultShellFeatureStates.Any())
            {
                foreach (var defaultShellFeatureState in _settings.DefaultShellFeatureStates)
                {
                    await Run(scope => Task.Run(() =>
                        {
                            // Don't run on e.g. the setup shell.
                            if (scope.Resolve<ShellSettings>().State != TenantState.Running) return;

                            // Pre-enabling features to load them early, so they will work even if e.g. they override an
                            // IFeatureEventHandler implementation.
                            var shellDescriptorManager = scope.Resolve<IShellDescriptorManager>();
                            var shellDescriptor = shellDescriptorManager.GetShellDescriptor();
                            shellDescriptor.Features = shellDescriptor.Features.Union(defaultShellFeatureState.EnabledFeatures.Select(feature => new ShellFeature { Name = feature }));
                            shellDescriptorManager.UpdateShellDescriptor(shellDescriptor.SerialNumber, shellDescriptor.Features, shellDescriptor.Parameters);

                            scope.Resolve<IFeatureManager>().EnableFeatures(defaultShellFeatureState.EnabledFeatures);
                        }), defaultShellFeatureState.ShellName);
                }
            }
        }

        public async Task Run(Func<IWorkContextScope, Task> process, string shellName)
        {
            var shellContext = _hostContainer.Resolve<IOrchardHost>().GetShellContext(new ShellSettings { Name = shellName });
            // We need a single HCA, and thus the same HttpContext throughout this scope to carry the work context. Especially
            // important for async code, see: https://orchard.codeplex.com/workitem/20509
            // Using InstancePerLifetimeScope() inside the shell ContinerBuilder still yields multiple instantiations.
            var hca = new AppHostHttpContextAccessor();
            using (var scope = shellContext.LifetimeScope.BeginLifetimeScope(
                RunScopeTag,
                builder =>
                {
                    builder.RegisterInstance(hca).As<IHttpContextAccessor>();
                    // Needed so it gets the AppHostHttpContextAccessor instance registered in Run().
                    builder.RegisterType<WorkContextAccessor>().As<IWorkContextAccessor>().SingleInstance();
                    builder.RegisterType<ProcessingEngineTaskAddedHandler>()
                        .As<IProcessingEngine>()
                        .As<IProcessingEngineTaskAddedHandler>()
                        .SingleInstance();
                    builder.RegisterType<ShellChangeHandler>()
                        .As<IShellChangeHandler>()
                        .As<IEventHandler>()
                        .Named<IEventHandler>(typeof(IShellDescriptorManagerEventHandler).Name)
                        .Named<IEventHandler>(typeof(IShellSettingsManagerEventHandler).Name)
                        .SingleInstance();
                }))
            {
                HttpContextBase httpContext;

                // Resolving will fail if it's just the setup shell. TryResolve() would still cause this exception.
                try
                {
                    // Will return the stub from Orchard.Mvc.MvcModule. There are some direct resolve calls to HttpContextBase in
                    // Orchard but it doesn't matter here (otherwise it does: https://orchard.codeplex.com/workitem/20778) as the
                    // point is to keep the same WorkContext in HttpContext.Items the same throughout this scope (unless a new
                    // WCS is started somewhere).
                    httpContext = scope.Resolve<HttpContextBase>();
                }
                catch (DependencyResolutionException ex)
                {
                    // Unfortunately the only way to identify the specific exception is by its message.
                    if (ex.Message.StartsWith("No scope with a Tag matching 'work' is visible from the scope in which the instance"))
                    {
                        httpContext = new AppHostHttpContextAccessor.HttpContextPlaceholder();
                    }
                    else throw;
                }

                hca.Set(httpContext);

                var logger = scope.Resolve<ILoggerService>();
                var orchardHost = scope.Resolve<IOrchardHost>();
                var previousTenantState = TenantState.Invalid;

                orchardHost.BeginRequest();

                try
                {
                    using (var workContext = scope.CreateWorkContextScope(httpContext))
                    {
                        try
                        {
                            await process(workContext);
                            previousTenantState = workContext.Resolve<ShellSettings>().State;
                        }
                        catch (Exception ex)
                        {
                            if (ex.IsFatal()) throw;

                            logger.Error(ex, "Error when executing work inside Orchard App Host.");
                            throw;
                        }
                    }
                }
                finally
                {
                    var shellSettingManagerEventHandler = _hostContainer.Resolve<IShellSettingsManagerEventHandler>();
                    var shellSettings = scope.Resolve<IShellSettingsManager>().LoadSettings().SingleOrDefault(settings => settings.Name == shellName);

                    // This means that setup just run. During setup the shell-tracking services are not registered.
                    if (previousTenantState == TenantState.Invalid && shellSettings != null && shellSettings.State == TenantState.Running)
                    {
                        shellSettingManagerEventHandler.Saved(shellSettings);
                    }
                    else
                    {
                        // Due to possibly await-ed calls in the process we keep track of everything that uses is stored in ContextState<T> normally.
                        // This is needed because ContextState<T> looses state on thread switch.
                        // Here we re-apply every change so the necessary services will get to know everything.
                        var shellChangeHandler = scope.Resolve<IShellChangeHandler>();
                        var shellDescriptorManagerEventHandler = _hostContainer.Resolve<IShellDescriptorManagerEventHandler>();
                        foreach (var changedShellDescriptor in shellChangeHandler.GetChangedShellDescriptors())
                        {
                            shellDescriptorManagerEventHandler.Changed(changedShellDescriptor.ShellDescriptor, changedShellDescriptor.TenantName);
                        }

                        foreach (var changedShellSettings in shellChangeHandler.GetChangedShellSettings())
                        {
                            shellSettingManagerEventHandler.Saved(changedShellSettings);
                        }

                        var processingEngine = _hostContainer.Resolve<IProcessingEngine>();
                        var processingEngineTaskAddedHandler = scope.Resolve<IProcessingEngineTaskAddedHandler>();
                        foreach (var task in processingEngineTaskAddedHandler.GetAddedTasks())
                        {
                            processingEngine.AddTask(task.ShellSettings, task.ShellDescriptor, task.MessageName, task.Parameters);
                        }
                    }

                    // Either this or running tasks through IProcessingEngine and restarting shells manually.
                    // EndRequest() have unwanted side effects in the future.
                    orchardHost.EndRequest();
                }
            }
        }

        public void Dispose()
        {
            // If this is implemented we should also terminate the Orchard host here: https://orchard.codeplex.com/workitem/20791

            if (_hostContainer != null)
            {
                _hostContainer.Dispose();
                _hostContainer = null;
            }
        }
    }
}
