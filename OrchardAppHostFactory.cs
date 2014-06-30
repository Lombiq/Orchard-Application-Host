using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lombiq.OrchardAppHost.Configuration;
using Lombiq.OrchardAppHost.Services;
using Orchard.Environment.Configuration;
using Autofac;
using Orchard.Environment.Descriptor.Models;
using Orchard.Environment.Descriptor;
using Lombiq.OrchardAppHost.Environment;
using Orchard.Environment.State;

namespace Lombiq.OrchardAppHost
{
    public static class OrchardAppHostFactory
    {
        /// <summary>
        /// Creates and starts an App Host.
        /// </summary>
        public static IOrchardAppHost StartHost()
        {
            return StartHost(null, null);
        }

        /// <summary>
        /// Creates and starts an App Host.
        /// </summary>
        /// <param name="settings">Settings for the App Host.</param>
        public static IOrchardAppHost StartHost(AppHostSettings settings)
        {
            return StartHost(settings, null);
        }

        /// <summary>
        /// Creates and starts an App Host.
        /// </summary>
        /// <param name="settings">Settings for the App Host.</param>
        /// <param name="registrations">Dependency registrations for the App Host.</param>
        public static IOrchardAppHost StartHost(AppHostSettings settings, AppHostRegistrations registrations)
        {
            var host = new OrchardAppHost(settings, registrations);
            host.Startup();
            return host;
        }

        /// <summary>
        /// Creates and starts a persistence-less App Host that doesn't have a database connection.
        /// </summary>
        /// <param name="settings">Settings for the App Host.</param>
        /// <param name="registrations">Dependency registrations for the App Host.</param>
        /// <param name="enabledStartupFeatures">Names of features to enable already when the shell starts.</param>
        public static IOrchardAppHost StartTransientHost(AppHostSettings settings, AppHostRegistrations registrations, IEnumerable<ShellFeature> enabledStartupFeatures)
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

                shellRegistrations(builder);
            };

            return StartHost(settings, registrations);
        }
    }
}