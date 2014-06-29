using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Autofac.Core;
using Orchard.Data;
using Orchard.Environment.Descriptor;

namespace Lombiq.OrchardAppHost.Services.Environment
{
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
