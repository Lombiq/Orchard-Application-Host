using Orchard.Environment;
using Orchard.Environment.Extensions;

namespace Lombiq.OrchardAppHost.Services.TransientHost
{
    /// <summary>
    /// Prevents data migrations from being run during shell startup for transient hosts, since transient hosts don't 
    /// have persistence configured.
    /// </summary>
    [OrchardFeature("Lombiq.OrchardAppHost.TransientHost")]
    [OrchardSuppressDependency("Orchard.Data.Migration.AutomaticDataMigrations")]
    public class NullAutomaticDataMigrations : IOrchardShellEvents
    {
        public void Activated()
        {
        }

        public void Terminating()
        {
        }
    }
}
