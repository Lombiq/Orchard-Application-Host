using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orchard.Environment.Configuration;
using Orchard.Tasks;

namespace Lombiq.OrchardAppHost.Environment.Tasks
{
    /// <summary>
    /// Runs async background tasks.
    /// </summary>
    /// <remarks>
    /// Also see: https://orchard.codeplex.com/workitem/20383
    /// </remarks>
    public class AsyncBackgroundService : IBackgroundTask
    {
        private readonly IOrchardAppHost _orchardAppHost;
        private readonly ShellSettings _shellSettings;


        public AsyncBackgroundService(IOrchardAppHost orchardAppHost, ShellSettings shellSettings)
        {
            _orchardAppHost = orchardAppHost;
            _shellSettings = shellSettings;
        }
        
    
        public async void Sweep()
        {
            await _orchardAppHost.Run(async scope =>
                {
                    var tasks = scope.Resolve<IEnumerable<IAsyncBackgroundTask>>();
                    if (!tasks.Any()) return;
                    // Not wrapping tasks in a transaction since this could also be run from a transient host.
                    await Task.WhenAll(tasks.Select(task => task.Sweep()));
                }, _shellSettings.Name);
        }
    }
}
