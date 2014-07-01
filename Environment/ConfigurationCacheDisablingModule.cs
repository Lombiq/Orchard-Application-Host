using Autofac;
using Autofac.Core;
using Orchard.Data;
using Orchard.Environment.Descriptor;

namespace Lombiq.OrchardAppHost.Environment
{
    /// <summary>
    /// When NHibernate session and shell descriptor caches are disabled when configuring the Orchard App Host this module
    /// applies the configuration to the respective implementations.
    /// </summary>
    public class ConfigurationCacheDisablingModule : Module
    {
        protected override void AttachToComponentRegistration(IComponentRegistry componentRegistry, IComponentRegistration registration)
        {
            if (registration.Activator.LimitType.IsAssignableTo<SessionConfigurationCache>())
            {
                registration.Activating += (sender, e) =>
                {
                    ((SessionConfigurationCache)e.Instance).Disabled = true;
                };
            }

            if (registration.Activator.LimitType.IsAssignableTo<ShellDescriptorCache>())
            {
                registration.Activating += (sender, e) =>
                {
                    ((ShellDescriptorCache)e.Instance).Disabled = true;
                };
            }
        }
    }
}
