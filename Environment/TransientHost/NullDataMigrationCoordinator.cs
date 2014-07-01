using Orchard.Environment;
using Orchard.Environment.Extensions;
using Orchard.Environment.Extensions.Models;

namespace Lombiq.OrchardAppHost.Services.TransientHost
{
    /// <summary>
    /// Prevents data migrations from being run during feature state changes for transient hosts, since transient hosts don't have persistence
    /// configured.
    /// </summary>
    [OrchardFeature("Lombiq.OrchardAppHost.TransientHost")]
    [OrchardSuppressDependency("Orchard.Data.Migration.DataMigrationCoordinator")]
    public class NullDataMigrationCoordinator : IFeatureEventHandler
    {
        public void Installing(Feature feature)
        {
        }

        public void Installed(Feature feature)
        {
        }

        public void Enabling(Feature feature)
        {
        }

        public void Enabled(Feature feature)
        {
        }

        public void Disabling(Feature feature)
        {
        }

        public void Disabled(Feature feature)
        {
        }

        public void Uninstalling(Feature feature)
        {
        }

        public void Uninstalled(Feature feature)
        {
        }
    }
}
