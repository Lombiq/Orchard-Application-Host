using System;
using Autofac;

namespace Lombiq.OrchardAppHost.Configuration
{
    /// <summary>
    /// Autofac dependency registrations for the Orchard Host.
    /// </summary>
    public class AppHostRegistrations
    {
        /// <summary>
        /// Gets or set dependency registrations for the whole host.
        /// </summary>
        public Action<ContainerBuilder> HostRegistrations { get; set; }

        /// <summary>
        /// Gets or set dependency registrations for each shell.
        /// </summary>
        public Action<ContainerBuilder> ShellRegistrations { get; set; }
    }
}
