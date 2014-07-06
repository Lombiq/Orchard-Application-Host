using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Autofac.Core;
using Orchard.Environment.Extensions.Folders;

namespace Lombiq.OrchardAppHost.Environment
{
    /// <summary>
    /// When NHibernate session and shell descriptor caches are disabled when configuring the Orchard App Host this module
    /// applies the configuration to the respective implementations.
    /// </summary>
    /// <remarks>Internal so it isn't automatically registered, just when needed.</remarks>
    internal class ExtensionMonitoringDisablingModule : Module
    {
        protected override void AttachToComponentRegistration(IComponentRegistry componentRegistry, IComponentRegistration registration)
        {
            if (registration.Activator.LimitType.IsAssignableTo<ExtensionHarvester>())
            {
                registration.Activating += (sender, e) =>
                {
                    ((ExtensionHarvester)e.Instance).DisableMonitoring = true;
                };
            }
        }
    }
}
