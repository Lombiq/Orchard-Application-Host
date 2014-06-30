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
        public static IOrchardAppHost StartHost()
        {
            return StartHost(null, null);
        }

        public static IOrchardAppHost StartHost(AppHostSettings settings)
        {
            return StartHost(settings, null);
        }

        public static IOrchardAppHost StartHost(AppHostSettings settings, AppHostRegistrations registrations)
        {
            var host = new OrchardAppHost(settings, registrations);
            host.Startup();
            return host;
        }

        /// <summary>
        /// Creates and starts a persistence-less host that doesn't have a database connection.
        /// </summary>
        public static IOrchardAppHost StartTransientHost(AppHostSettings settings, AppHostRegistrations registrations, IEnumerable<ShellFeature> enabledFeatures)
        {
            if (registrations == null) registrations = new AppHostRegistrations();

            var appRegistrations = (registrations.AppRegistrations == null) ? builder => { } : registrations.AppRegistrations;
            registrations.AppRegistrations = builder =>
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
                if (enabledFeatures == null) enabledFeatures = Enumerable.Empty<ShellFeature>();
                enabledFeatures = enabledFeatures.Union(new[]
                {
                    new ShellFeature { Name = "Orchard.Framework" },
                    new ShellFeature { Name = "Lombiq.OrchardAppHost.TransientHost" }
                });

                builder
                    .RegisterInstance(new DefaultTransientShellDescriptorProvider(new ShellDescriptor { Features = enabledFeatures }))
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