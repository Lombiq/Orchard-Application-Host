using System;
using System.Threading.Tasks;
using Orchard;

namespace Lombiq.OrchardAppHost
{
    /// <summary>
    /// Describes a service for running an Orchard Application Host. You can use such a host to run code inside the 
    /// context of an Orchard application (and Orchard shells).
    /// </summary>
    public interface IOrchardAppHost : IDisposable
    {
        /// <summary>
        /// Initilizes the Orchard Application Host. This method should be called once before the Host can be used.
        /// </summary>
        Task Startup();

        /// <summary>
        /// Runs a process inside the Orchard Application Host. The method is thread-safe.
        /// </summary>
        /// <param name="process">
        /// The process to run inside the Orchard Application Host. This can be any code that will behave just as inside
        /// an Orchard application and will be able to use Orchard's services.
        /// </param>
        /// <param name="shellName">
        /// Name of the Orchard shell to run the process in. You can run multiple Orchard shells inside the same
        /// Application Host instance.
        /// </param>
        Task Run(Func<IWorkContextScope, Task> process, string shellName);
    }
}
