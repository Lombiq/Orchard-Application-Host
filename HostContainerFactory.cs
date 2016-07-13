using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using Autofac;
using Autofac.Builder;
using Lombiq.OrchardAppHost.Configuration;
using Lombiq.OrchardAppHost.Environment;
using Orchard.Caching;
using Orchard.Environment;
using Orchard.Environment.Extensions.Folders;
using Orchard.Environment.Extensions.Loaders;
using Orchard.FileSystems.AppData;
using Orchard.FileSystems.VirtualPath;
using Orchard.FileSystems.WebSite;
using Orchard.Localization.Services;
using Orchard.Logging;

namespace Lombiq.OrchardAppHost
{
    /// <summary>
    /// Builds the Autofac container with all the default registrations.
    /// </summary>
    internal static class HostContainerFactory
    {
        public static IContainer CreateHostContainer(IOrchardAppHost appHost, AppHostSettings settings, AppHostRegistrations registrations)
        {
            return OrchardStarter.CreateHostContainer(builder =>
            {
                builder.RegisterType<AppHostEnvironment>().As<IHostEnvironment>().SingleInstance();
                builder.RegisterType<ApplicationEnvironment>().As<IApplicationEnvironment>().SingleInstance();
                builder.RegisterType<CultureDateTimeFormatProvider>().As<IDateTimeFormatProvider>().SingleInstance();

                // Needed also for shells, separately.
                RegisterAppDataFolderRoot(builder, settings.AppDataFolderPath).SingleInstance();

                RegisterVolatileProvider<AppHostVirtualPathMonitor, IVirtualPathMonitor>(builder);
                RegisterVolatileProvider<AppHostVirtualPathProvider, IVirtualPathProvider>(builder);
                RegisterVolatileProvider<AppHostWebSiteFolder, IWebSiteFolder>(builder);

                var shellRegistrations = new ShellContainerRegistrations
                {
                    Registrations = shellBuilder =>
                    {
                        // Despite imported assemblies being handled these registrations are necessary, because they are needed too early.
                        // Adding them as normal services, even with OrchardSuppressDependency, wouldn't work.

                        RegisterAppDataFolderRoot(shellBuilder, settings.AppDataFolderPath).InstancePerMatchingLifetimeScope("shell");

                        RegisterVolatileProviderForShell<AppHostVirtualPathMonitor, IVirtualPathMonitor>(shellBuilder);
                        RegisterVolatileProviderForShell<AppHostVirtualPathProvider, IVirtualPathProvider>(shellBuilder);
                        RegisterVolatileProviderForShell<AppHostWebSiteFolder, IWebSiteFolder>(shellBuilder);

                        if (registrations.ShellRegistrations != null)
                        {
                            registrations.ShellRegistrations(shellBuilder);
                        }
                    }
                };
                builder.RegisterInstance(shellRegistrations).As<IShellContainerRegistrations>();

                // Handling imported assemblies.
                if (settings.ImportedExtensions != null && settings.ImportedExtensions.Any())
                {
                    builder.RegisterType<ImportedExtensionsProvider>().As<IExtensionFolders, IExtensionLoader>().SingleInstance()
                        .WithParameter(new NamedParameter("extensions", settings.ImportedExtensions));
                }

                // Configuring extension loading.
                builder.RegisterType<ExtensionPathsProvider>().As<IExtensionPathsProvider>().SingleInstance()
                    .WithParameter(new NamedParameter("hostSettings", settings));
                builder.RegisterType<AppHostExtensionFolders>().As<IExtensionFolders>().SingleInstance();
                builder.RegisterType<AppHostCoreExtensionLoader>().As<IExtensionLoader>().SingleInstance();
                builder.RegisterType<AppHostRawThemeExtensionLoader>().As<IExtensionLoader>().SingleInstance();

                if (settings.DisableConfiguratonCaches)
                {
                    builder.RegisterModule<ConfigurationCacheDisablingModule>();
                }

                if (settings.DisableExtensionMonitoring)
                {
                    builder.RegisterModule<ExtensionMonitoringDisablingModule>();
                }

                // Either we register MVC singletons or we need at least a new IOrchardShell implementation.
                builder.Register(ctx => RouteTable.Routes).SingleInstance();
                builder.Register(ctx => ModelBinders.Binders).SingleInstance();
                builder.Register(ctx => ViewEngines.Engines).SingleInstance();

                builder.RegisterType<LoggerService>().As<ILoggerService>().SingleInstance();

                builder.RegisterInstance(appHost).As<IOrchardAppHost>().ExternallyOwned();

                if (registrations.HostRegistrations != null)
                {
                    registrations.HostRegistrations(builder);
                }
            });
        }

        private static IRegistrationBuilder<AppHostAppDataFolderRoot, ConcreteReflectionActivatorData, SingleRegistrationStyle> RegisterAppDataFolderRoot(ContainerBuilder builder, string appDataFolderPath)
        {
            var appDataRootRegistration = builder.RegisterType<AppHostAppDataFolderRoot>().As<IAppDataFolderRoot>();
            if (!string.IsNullOrEmpty(appDataFolderPath))
            {
                appDataRootRegistration.WithParameter(new NamedParameter("rootPath", appDataFolderPath));
            }
            return appDataRootRegistration;
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
