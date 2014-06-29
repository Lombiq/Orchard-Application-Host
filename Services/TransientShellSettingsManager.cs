using System.Collections.Concurrent;
using System.Collections.Generic;
using Orchard.Environment.Configuration;
using Orchard.Environment.Extensions;

namespace Lombiq.OrchardAppHost.Services
{
    /// <summary>
    /// An <see cref="Orchard.Environment.Configuration.IShellSettingsManager"/> implementation that doesn't persist shell settings.
    /// </summary>
    [OrchardFeature("Lombiq.OrchardAppHost.TransientHost")]
    public class TransientShellSettingsManager : IShellSettingsManager
    {
        private readonly ConcurrentDictionary<string, ShellSettings> _shellSettings = new ConcurrentDictionary<string,ShellSettings>();


        public IEnumerable<ShellSettings> LoadSettings()
        {
            return _shellSettings.Values;
        }

        public void SaveSettings(ShellSettings settings)
        {
            _shellSettings.AddOrUpdate(settings.Name, settings, (key, existing) => settings);
        }
    }
}
