using System;
using System.Threading.Tasks;
using Orchard;

namespace Lombiq.OrchardAppHost
{
    /// <summary>
    /// Describes a service for running an Orchard Application Host.
    /// </summary>
    public interface IOrchardAppHost : IDisposable
    {
        /// <summary>
        /// Initilizes the Orchard Application Host. This method should be called before the Host can be used.
        /// </summary>
        Task Startup();

        /// <summary>
        /// Runs a process inside the Orchard Application Host.
        /// </summary>
        /// <param name="process">The process to run inside the Orchard Application Host.</param>
        /// <param name="shellName">Name of the Orchard shell to run the process in.</param>
        Task Run(Func<IWorkContextScope, Task> process, string shellName);
    }
}
