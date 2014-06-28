using System;
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
