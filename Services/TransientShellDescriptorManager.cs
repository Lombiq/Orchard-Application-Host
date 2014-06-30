using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Orchard;
using Orchard.Environment.Configuration;
using Orchard.Environment.Descriptor;
using Orchard.Environment.Descriptor.Models;
using Orchard.Environment.Extensions;
using Orchard.Localization;

namespace Lombiq.OrchardAppHost.Services
{
    /// <summary>
    /// An <see cref="Orchard.Environment.Descriptor.IShellDescriptorManager"/> implementation that doesn't persist shell descriptors.
    /// </summary>
    /// <remarks>
    /// Must be abstract otherwise it would get auto-registered and override the default implementation all the time.
    /// </remarks>
    [OrchardFeature("Lombiq.OrchardAppHost.TransientHost")]
    public class TransientShellDescriptorManager : IShellDescriptorManager
    {
        private const string StoreKey = "Lombiq.OrchardAppHost.TransientShellDescriptorManager.ShellDescriptor";

        private readonly ITransientStore _transientStore;
        private readonly IShellDescriptorManagerEventHandler _events;
        private readonly ShellSettings _shellSettings;
        private readonly IDefaultTransientShellDescriptorProvider _defaultTransientShellDescriptorProvider;

        private ShellDescriptor ShellDescriptor
        {
            get { return _transientStore.Get<ShellDescriptor>(StoreKey); }
            set { _transientStore.Set(StoreKey, value); }
        }


        public TransientShellDescriptorManager(
            ITransientStore transientStore,
            IShellDescriptorManagerEventHandler events,
            ShellSettings shellSettings,
            IDefaultTransientShellDescriptorProvider defaultTransientShellDescriptorProvider)
        {
            _transientStore = transientStore;
            _events = events;
            _shellSettings = shellSettings;
            _defaultTransientShellDescriptorProvider = defaultTransientShellDescriptorProvider;
        }


        public ShellDescriptor GetShellDescriptor()
        {
            var shellDescriptor = ShellDescriptor;

            if (shellDescriptor == null)
            {
                var defaultShellDescriptor = _defaultTransientShellDescriptorProvider.GetDefaultShellDescriptor();
                if (defaultShellDescriptor != null)
                {
                    shellDescriptor = ShellDescriptor = defaultShellDescriptor; 
                }
            }

            return shellDescriptor;
        }

        public void UpdateShellDescriptor(int priorSerialNumber, IEnumerable<ShellFeature> enabledFeatures, IEnumerable<ShellParameter> parameters)
        {
            var priorDescriptor = ShellDescriptor;
            var serialNumber = priorDescriptor == null ? 0 : priorDescriptor.SerialNumber;
            if (priorSerialNumber != serialNumber)
                throw new InvalidOperationException("Invalid serial number for shell descriptor");

            if (enabledFeatures == null) enabledFeatures = Enumerable.Empty<ShellFeature>();
            if (parameters == null) parameters = Enumerable.Empty<ShellParameter>();

            var shellDescriptor = new ShellDescriptor
                {
                    SerialNumber = ++serialNumber,
                    Features = enabledFeatures,
                    Parameters = parameters
                };
            ShellDescriptor = shellDescriptor;

            _events.Changed(shellDescriptor, _shellSettings.Name);
        }
    }
}
