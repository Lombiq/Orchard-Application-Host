using System;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Routing;
using Autofac;
using Autofac.Builder;
using Lombiq.OrchardAppHost.Configuration;
using Lombiq.OrchardAppHost.Services;
using Lombiq.OrchardAppHost.Services.Environment;
using Lombiq.OrchardAppHost.Services.Extensions;
using Orchard;
using Orchard.Caching;
using Orchard.Environment;
using Orchard.Environment.Configuration;
using Orchard.Environment.Extensions;
using Orchard.Environment.Extensions.Folders;
using Orchard.Environment.Extensions.Loaders;
using Orchard.Environment.Features;
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


        public void Startup()
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

            _hostContainer = CreateHostContainer();

            _hostContainer.Resolve<IOrchardHost>().Initialize();

            if (_settings.DefaultShellFeatureStates != null && _settings.DefaultShellFeatureStates.Any())
            {
                foreach (var defaultShellFeatureState in _settings.DefaultShellFeatureStates)
                {
                    Run(scope => scope.Resolve<IFeatureManager>().EnableFeatures(defaultShellFeatureState.EnabledFeatures),
                        defaultShellFeatureState.ShellName);
                }
            }
        }

        public void Run(Action<IWorkContextScope> process, string shellName)
        {
            var shellContext = _hostContainer.Resolve<IOrchardHost>().GetShellContext(new ShellSettings { Name = shellName });
            using (var scope = shellContext.LifetimeScope.BeginLifetimeScope())
            {
                var httpContext = scope.Resolve<IHttpContextAccessor>().Current();
                using (var workContext = scope.Resolve<IWorkContextAccessor>().CreateWorkContextScope(httpContext))
                {
                    try
                    {
                        process(workContext);
                    }
                    catch (Exception ex)
                    {
                        if (ex.IsFatal()) throw;

                        scope.Resolve<ILoggerService>().Error(ex, "Error when executing work inside Orchard App Host.");
                        throw;
                    }
                }

                // Either this or running tasks through IProcessingEngine as below. EndRequest() also restarts updated shells but
                // can have unwanted side effects in the future.
                _hostContainer.Resolve<IOrchardHost>().EndRequest();
                //var processingEngine = scope.Resolve<IProcessingEngine>();
                //while (processingEngine.AreTasksPending())
                //    processingEngine.ExecuteNextTask();
            }
        }

        public void Dispose()
        {
            if (_hostContainer != null)
            {
                _hostContainer.Dispose();
                _hostContainer = null;
            }
        }

        private IContainer CreateHostContainer()
        {
            return OrchardStarter.CreateHostContainer(builder =>
            {
                builder.Register(context =>
                {
                    var hostEnvironment = new AppHostEnvironment(context.Resolve<IVirtualPathProvider>());
                    hostEnvironment.AppDomainRestartRequested += () =>
                        {
                            // Is anything needed here?
                            //AppDomain.Unload(AppDomain.CurrentDomain)
                            //throw new ApplicationException();
                            //Startup();
                        };
                    return hostEnvironment;
                }).As<IHostEnvironment>().SingleInstance();
                var appDataRootRegistration = builder.RegisterType<AppHostAppDataFolderRoot>().As<IAppDataFolderRoot>().SingleInstance();
                if (!string.IsNullOrEmpty(_settings.AppDataFolderPath))
                {
                    appDataRootRegistration.WithParameter(new NamedParameter("rootPath", _settings.AppDataFolderPath));
                }

                RegisterVolatileProvider<AppHostVirtualPathMonitor, IVirtualPathMonitor>(builder);
                RegisterVolatileProvider<AppHostVirtualPathProvider, IVirtualPathProvider>(builder);
                RegisterVolatileProvider<AppHostWebSiteFolder, IWebSiteFolder>(builder);

                var shellRegistrations = new ShellContainerRegistrations
                {
                    Registrations = shellBuilder =>
                    {
                        // Despite imported assemblies being handled these registrations are necessary, because they are needed too early.
                        shellBuilder.RegisterType<AppHostAppDataFolderRoot>().As<IAppDataFolderRoot>().InstancePerMatchingLifetimeScope("shell");
                        RegisterVolatileProviderForShell<AppHostVirtualPathMonitor, IVirtualPathMonitor>(shellBuilder);
                        RegisterVolatileProviderForShell<AppHostVirtualPathProvider, IVirtualPathProvider>(shellBuilder);
                        RegisterVolatileProviderForShell<AppHostWebSiteFolder, IWebSiteFolder>(shellBuilder);

                        if (_registrations.ShellRegistrations != null)
                        {
                            _registrations.ShellRegistrations(shellBuilder);
                        }
                    }
                };
                builder.RegisterInstance(shellRegistrations).As<IShellContainerRegistrations>();

                // Need to be a new instance per each life time scope because it's needed to carry the work context through the scope.
                builder.RegisterType<AppHostHttpContextAccessor>().As<IHttpContextAccessor>().InstancePerLifetimeScope();

                // Extension folders should be configured.
                if (_settings.ModuleFolderPaths != null && _settings.ModuleFolderPaths.Any())
                {
                    builder.RegisterType<ModuleFolders>().As<IExtensionFolders>().SingleInstance()
                        .WithParameter(new NamedParameter("paths", _settings.ModuleFolderPaths));
                }
                if (_settings.CoreModuleFolderPaths != null && _settings.CoreModuleFolderPaths.Any())
                {
                    builder.RegisterType<CoreModuleFolders>().As<IExtensionFolders>().SingleInstance()
                        .WithParameter(new NamedParameter("paths", _settings.CoreModuleFolderPaths));
                }
                if (_settings.ThemeFolderPaths != null && _settings.ThemeFolderPaths.Any())
                {
                    builder.RegisterType<ThemeFolders>().As<IExtensionFolders>().SingleInstance()
                        .WithParameter(new NamedParameter("paths", _settings.ThemeFolderPaths));
                }

                // Handling imported assemblies.
                if (_settings.ImportedExtensions != null && _settings.ImportedExtensions.Any())
                {
                    builder.RegisterType<ImportedExtensionsProvider>().As<IExtensionFolders, IExtensionLoader>().SingleInstance()
                        .WithParameter(new NamedParameter("extensions", _settings.ImportedExtensions));
                }

                if (_settings.DisableConfiguratonCaches)
                {
                    builder.RegisterModule<ConfigurationCacheDisablingModule>(); 
                }

                builder.RegisterType<AppHostCoreExtensionLoader>().As<IExtensionLoader>().SingleInstance();
                builder.RegisterType<AppHostRawThemeExtensionLoader>().As<IExtensionLoader>().SingleInstance();

                // Either we register MVC singletons or we need at least a new IOrchardShell implementation.
                builder.Register(ctx => RouteTable.Routes).SingleInstance();
                builder.Register(ctx => ModelBinders.Binders).SingleInstance();
                builder.Register(ctx => ViewEngines.Engines).SingleInstance();

                builder.RegisterType<OrchardLog4netFactory>().As<Castle.Core.Logging.ILoggerFactory>().InstancePerLifetimeScope();

                if (_registrations.AppRegistrations != null)
                {
                    _registrations.AppRegistrations(builder);
                }
            });
        }


        private static IRegistrationBuilder<TRegister, ConcreteReflectionActivatorData, SingleRegistrationStyle> RegisterVolatileProvider<TRegister, TService>(ContainerBuilder builder)
            where TService : IVolatileProvider
        {
            return builder.RegisterType<TRegister>()
                .As<TService, IVolatileProvider>()
                .SingleInstance();
        }

        private static IRegistrationBuilder<TRegister, ConcreteReflectionActivatorData, SingleRegistrationStyle> RegisterVolatileProviderForShell<TRegister, TService>(ContainerBuilder builder)
            where TService : IVolatileProvider
        {
            return RegisterVolatileProvider<TRegister, TService>(builder).InstancePerMatchingLifetimeScope("shell");
        }


        private class ShellContainerRegistrations : IShellContainerRegistrations
        {
            public Action<ContainerBuilder> Registrations { get; set; }
        }
    }
}
