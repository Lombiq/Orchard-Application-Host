using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Orchard.Caching;
using Orchard.Commands;
using Orchard.Data;
using Orchard.Environment;
using Orchard.Environment.Configuration;
using Orchard.FileSystems.VirtualPath;
using Orchard.Mvc;
using Orchard.Exceptions;
using Orchard.Logging;
using Orchard.Environment.State;
using Orchard.FileSystems.AppData;
using Orchard;
using Orchard.Environment.Extensions.Folders;
using Lombiq.OrchardAppHost.Configuration;
using Orchard.FileSystems.WebSite;
using System.Web.Compilation;
using Lombiq.OrchardAppHost.Services;
using Autofac.Builder;
using Orchard.Environment.Extensions.Loaders;
using System.Reflection;
using System.Web.Routing;
using System.Web.Mvc;

namespace Lombiq.OrchardAppHost
{
    public class OrchardAppHost : IOrchardAppHost
    {
        private readonly AppHostSettings _settings;
        private readonly Action<ContainerBuilder> _appRegistrations;
        private readonly Action<ContainerBuilder> _shellRegistrations;
        private IContainer _hostContainer = null;


        // Having an overload without "registrations" enables calling it without the caller having a reference to Autofac.
        public OrchardAppHost(AppHostSettings settings)
            : this(settings, null, null)
        {
            _settings = settings;
        }

        public OrchardAppHost(
            AppHostSettings settings,
            Action<ContainerBuilder> appRegistrations,
            Action<ContainerBuilder> shellRegistrations)
        {
            _appRegistrations = appRegistrations;
            _shellRegistrations = shellRegistrations;
        }


        public void Startup()
        {
            // Is this needed?
            //var clientBuildManager = new ClientBuildManager("/", @"E:\Projects\Munka\Lombiq\Orchard Dev Hg\src\Orchard.Web");
            //clientBuildManager.CompileApplicationDependencies();

            if (_settings.ImportedExtensionAssemblies == null)
            {
                _settings.ImportedExtensionAssemblies = Enumerable.Empty<Assembly>();
            }
            _settings.ImportedExtensionAssemblies = _settings.ImportedExtensionAssemblies.Union(new[] { this.GetType().Assembly });

            _hostContainer = CreateHostContainer();
            _hostContainer.Resolve<IOrchardHost>().Initialize();
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
                // can have unwanted sideeffects in the future.
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

                var assembliesImported = _settings.ImportedExtensionAssemblies != null && _settings.ImportedExtensionAssemblies.Any();

                var shellRegistrations = new ShellContainerRegistrations
                {
                    Registrations = shellBuilder =>
                    {
                        // Despite imported assemblies being handled these registrations are necessary, because they are needed too early.
                        shellBuilder.RegisterType<AppHostAppDataFolderRoot>().As<IAppDataFolderRoot>().InstancePerMatchingLifetimeScope("shell");
                        RegisterVolatileProvider<AppHostVirtualPathMonitor, IVirtualPathMonitor>(shellBuilder);
                        RegisterVolatileProvider<AppHostVirtualPathProvider, IVirtualPathProvider>(shellBuilder);
                        RegisterVolatileProvider<AppHostWebSiteFolder, IWebSiteFolder>(shellBuilder);

                        if (assembliesImported)
                        {
                            shellBuilder.RegisterModule<ShellDescriptorManagerModule>();
                        }

                        if (_shellRegistrations != null)
                        {
                            _shellRegistrations(shellBuilder);
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
                if (assembliesImported)
                {
                    builder.RegisterType<ImportedAssembliesAccessor>().As<IImportedAssembliesAccessor>().SingleInstance()
                        .WithParameter(new NamedParameter("assemblies", _settings.ImportedExtensionAssemblies));

                    builder.RegisterType<ImportedAssembliesExtensionProvider>().As<IExtensionFolders>().As<IExtensionLoader>().SingleInstance();
                }

                builder.RegisterType<AppHostCoreExtensionLoader>().As<IExtensionLoader>().SingleInstance();
                builder.RegisterType<AppHostRawThemeExtensionLoader>().As<IExtensionLoader>().SingleInstance();

                // Either we register MVC singletons or we need at least a new IOrchardShell implementation.
                builder.Register(ctx => RouteTable.Routes).SingleInstance();
                builder.Register(ctx => ModelBinders.Binders).SingleInstance();
                builder.Register(ctx => ViewEngines.Engines).SingleInstance();

                builder.RegisterType<OrchardLog4netFactory>().As<Castle.Core.Logging.ILoggerFactory>().InstancePerLifetimeScope();

                if (_appRegistrations != null)
                {
                    _appRegistrations(builder);
                }
            });
        }


        private static IRegistrationBuilder<TRegister, ConcreteReflectionActivatorData, SingleRegistrationStyle> RegisterVolatileProvider<TRegister, TService>(ContainerBuilder builder)
            where TService : IVolatileProvider
        {
            return builder.RegisterType<TRegister>()
                .As<TService>()
                .As<IVolatileProvider>()
                .SingleInstance();
        }

        private static IRegistrationBuilder<TRegister, ConcreteReflectionActivatorData, SingleRegistrationStyle> RegisterVolatileProviderForShell<TRegister, TService>(ContainerBuilder builder)
            where TService : IVolatileProvider
        {
            return RegisterVolatileProvider<TRegister, TService>(builder).InstancePerMatchingLifetimeScope("shell"); ;
        }


        private class ShellContainerRegistrations : IShellContainerRegistrations
        {
            public Action<ContainerBuilder> Registrations { get; set; }
        }
    }
}
