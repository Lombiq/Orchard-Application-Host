using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
