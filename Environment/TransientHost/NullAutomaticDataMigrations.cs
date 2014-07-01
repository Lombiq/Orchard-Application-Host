using Orchard.Environment;
using Orchard.Environment.Extensions;

namespace Lombiq.OrchardAppHost.Services.TransientHost
{
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
