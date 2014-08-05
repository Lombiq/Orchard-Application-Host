using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Lombiq.OrchardAppHost.Configuration;
using Lombiq.OrchardAppHost.Services.TransientHost;
using Orchard.ContentManagement.MetaData;
using Orchard.Environment.Configuration;
using Orchard.Environment.Descriptor;
using Orchard.Environment.Descriptor.Models;
using Orchard.Environment.State;
using Orchard.Settings;

namespace Lombiq.OrchardAppHost
{
    public static class OrchardAppHostFactory
    {
        /// <summary>
        /// Creates and starts an App Host.
        /// </summary>
        public static Task<IOrchardAppHost> StartHost()
        {
            return StartHost(null, null);
        }

        /// <summary>
        /// Creates and starts an App Host.
        /// </summary>
        /// <param name="settings">Settings for the App Host.</param>
        public static Task<IOrchardAppHost> StartHost(AppHostSettings settings)
        {
            return StartHost(settings, null);
        }

        /// <summary>
        /// Creates and starts an App Host.
        /// </summary>
        /// <param name="settings">Settings for the App Host.</param>
        /// <param name="registrations">Dependency registrations for the App Host.</param>
        public static async Task<IOrchardAppHost> StartHost(AppHostSettings settings, AppHostRegistrations registrations)
        {
            var host = new OrchardAppHost(settings, registrations);
            await host.Startup();
            return host;
        }

        /// <summary>
        /// Creates and starts a persistence-less App Host that doesn't have a database connection. A transient host is very light-weight and
        /// although its state is not persisted (the Orchard shell state; any persistence you do will work of course) it's kept until the
        /// Host is disposed. That means you can still enabled/disable features for example and the shell state will change as expected.
        /// </summary>
        /// <param name="settings">Settings for the App Host.</param>
        /// <param name="registrations">Dependency registrations for the App Host.</param>
        /// <param name="enabledStartupFeatures">Names of features to enable already when the shell starts.</param>
        public static Task<IOrchardAppHost> StartTransientHost(AppHostSettings settings, AppHostRegistrations registrations, IEnumerable<ShellFeature> enabledStartupFeatures)
        {
            if (registrations == null) registrations = new AppHostRegistrations();

            var appRegistrations = (registrations.HostRegistrations == null) ? builder => { } : registrations.HostRegistrations;
            registrations.HostRegistrations = builder =>
                {
                    var shellSettingsManager = new TransientShellSettingsManager();
                    shellSettingsManager.SaveSettings(new ShellSettings { Name = ShellSettings.DefaultName, State = TenantState.Running });
                    builder.RegisterInstance(shellSettingsManager).As<IShellSettingsManager>().SingleInstance();

                    builder.RegisterType<HostTransientStore>().As<IHostTransientStore>().SingleInstance();

                    appRegistrations(builder);
                };

            var shellRegistrations = (registrations.ShellRegistrations == null) ? builder => { } : registrations.ShellRegistrations;
            registrations.ShellRegistrations = builder =>
            {
                if (enabledStartupFeatures == null) enabledStartupFeatures = Enumerable.Empty<ShellFeature>();
                enabledStartupFeatures = enabledStartupFeatures.Union(new[]
                {
                    new ShellFeature { Name = "Orchard.Framework" },
                    new ShellFeature { Name = "Lombiq.OrchardAppHost.TransientHost" }
                });

                builder
                    .RegisterInstance(new DefaultTransientShellDescriptorProvider(new ShellDescriptor { Features = enabledStartupFeatures }))
                    .As<IDefaultTransientShellDescriptorProvider>();

                builder.RegisterType<TransientShellDescriptorManager>().As<IShellDescriptorManager>().SingleInstance();
                builder.RegisterType<TransientShellStateManager>().As<IShellStateManager>().SingleInstance();
                builder.RegisterType<TransientStore>().As<ITransientStore>().SingleInstance();

                // These below are only needed if Core extensions are not loaded (i.e. their path not set in AppHostSettings).
                // Needed too early in ShellContextFactory, since the minimum shell doesn't include any external feautures.
                builder.RegisterType<NullSiteService>().As<ISiteService>().SingleInstance();
                builder.RegisterType<NullContentDefinitionManager>().As<IContentDefinitionManager>().SingleInstance();

                shellRegistrations(builder);
            };

            return StartHost(settings, registrations);
        }
    }
}