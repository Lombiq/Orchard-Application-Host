using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;

namespace Lombiq.OrchardAppHost.Configuration
{
    /// <summary>
    /// Autofac dependency registrations for the Orchard Host.
    /// </summary>
    public class AppHostRegistrations
    {
        public Action<ContainerBuilder> AppRegistrations { get; set; }
        public Action<ContainerBuilder> ShellRegistrations { get; set; }
    }
}
