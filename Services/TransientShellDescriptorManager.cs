using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Orchard;
using Orchard.Environment.Descriptor;
using Orchard.Environment.Descriptor.Models;
using Orchard.Localization;

namespace Lombiq.OrchardAppHost.Services
{
    /// <summary>
    /// An <see cref="Orchard.Environment.Descriptor.IShellDescriptorManager"/> implementation that doesn't persist shell descriptors.
    /// </summary>
    /// <remarks>
    /// Must be abstract otherwise it would get auto-registered and override the default implementation all the time.
    /// </remarks>
    public abstract class TransientShellDescriptorManager : IShellDescriptorManager, ISingletonDependency
    {
        ReaderWriterLockSlim locker = new ReaderWriterLockSlim();

        private ShellDescriptor _shellDescriptor = null;
        private ShellDescriptor ShellDescriptor
        {
            get
            {
                locker.EnterReadLock();

                try
                {
                    return _shellDescriptor;
                }
                finally
                {
                    locker.ExitReadLock();
                }
            }
            set
            {
                locker.EnterWriteLock();

                try
                {
                    _shellDescriptor = value;
                }
                finally
                {
                    locker.ExitWriteLock();
                }
            }
        }

        public Localizer T { get; set; }


        protected TransientShellDescriptorManager()
        {
            T = NullLocalizer.Instance;
        }


        public ShellDescriptor GetShellDescriptor()
        {
            return ShellDescriptor;
        }

        public void UpdateShellDescriptor(int priorSerialNumber, IEnumerable<ShellFeature> enabledFeatures, IEnumerable<ShellParameter> parameters)
        {
            var priorDescriptor = ShellDescriptor;
            var serialNumber = priorDescriptor == null ? 0 : priorDescriptor.SerialNumber;
            if (priorSerialNumber != serialNumber)
                throw new InvalidOperationException(T("Invalid serial number for shell descriptor").ToString());

            if (enabledFeatures == null) enabledFeatures = Enumerable.Empty<ShellFeature>();
            if (parameters == null) parameters = Enumerable.Empty<ShellParameter>();

            ShellDescriptor = new ShellDescriptor
                {
                    SerialNumber = ++serialNumber,
                    Features = enabledFeatures,
                    Parameters = parameters
                };
        }
    }
}
