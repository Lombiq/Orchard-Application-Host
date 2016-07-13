using System.Threading.Tasks;
using Orchard;

namespace Lombiq.OrchardAppHost.Environment.Tasks
{
    /// <summary>
    /// Describes a service that runs periodically in the background in an async way.
    /// </summary>
    public interface IAsyncBackgroundTask : IDependency
    {
        /// <summary>
        /// Called periodically (by default every minute) in the background. Be aware that such calls are NOT wrapped 
        /// in a transaction.
        /// </summary>
        Task Sweep();
    }
}
