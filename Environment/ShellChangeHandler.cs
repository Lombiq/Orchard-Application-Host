using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Orchard.Environment.Configuration;
using Orchard.Environment.Descriptor;
using Orchard.Environment.Descriptor.Models;

namespace Lombiq.OrchardAppHost.Environment
{
    public interface IChangedShellDescriptor
    {
        string TenantName { get; }
        ShellDescriptor ShellDescriptor { get; }
    }


    /// <summary>
    /// Service for detecting and registering shell changes so the affected shells can be restarted. This is necessary as 
    /// <see cref="Orchard.Environment.DefaultOrchardHost"/>' looses shells queued for restart on thread switches (what 
    /// happens in async code).
    /// </summary>
    public interface IShellChangeHandler
    {
        IEnumerable<IChangedShellDescriptor> GetChangedShellDescriptors();
        IEnumerable<ShellSettings> GetChangedShellSettings();
    }


    // Should be internal so it's not automatically registered in Autofac.
    internal class ShellChangeHandler : IShellChangeHandler, IShellDescriptorManagerEventHandler, IShellSettingsManagerEventHandler
    {
        private readonly ConcurrentDictionary<string, ShellDescriptor> _changedShellDescriptors = new ConcurrentDictionary<string, ShellDescriptor>();
        private readonly ConcurrentDictionary<string, ShellSettings> _changedShellSettings = new ConcurrentDictionary<string, ShellSettings>();


        public IEnumerable<IChangedShellDescriptor> GetChangedShellDescriptors()
        {
            return _changedShellDescriptors.Select(kvp => new ChangedShellDescriptor { TenantName = kvp.Key, ShellDescriptor = kvp.Value });
        }

        public IEnumerable<ShellSettings> GetChangedShellSettings()
        {
            return _changedShellSettings.Values;
        }

        void IShellDescriptorManagerEventHandler.Changed(ShellDescriptor descriptor, string tenant)
        {
            _changedShellDescriptors[tenant] = descriptor;
        }

        void IShellSettingsManagerEventHandler.Saved(ShellSettings settings)
        {
            _changedShellSettings[settings.Name] = settings;
        }


        private class ChangedShellDescriptor : IChangedShellDescriptor
        {
            public string TenantName { get; set; }
            public ShellDescriptor ShellDescriptor { get; set; }
        }
    }
}
