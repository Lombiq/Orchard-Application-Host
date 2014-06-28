using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orchard.Environment.Descriptor;
using Orchard.Environment.Descriptor.Models;

namespace Lombiq.OrchardAppHost.Services
{
    // Should be internal so it isn't registerd automatically but just through ShellDescriptorManagerModule
    internal class ImportedExtensionsEnablingShellDescriptorManager : IShellDescriptorManager
    {
        private readonly IShellDescriptorManager _decorated;
        private readonly IImportedAssembliesAccessor _assembliesAccessor;


        public ImportedExtensionsEnablingShellDescriptorManager(
            IShellDescriptorManager decorated,
            IImportedAssembliesAccessor assembliesAccessor)
        {
            _decorated = decorated;
            _assembliesAccessor = assembliesAccessor;
        }
        
    
        public ShellDescriptor GetShellDescriptor()
        {
            var shellDescriptor = _decorated.GetShellDescriptor();
            shellDescriptor.Features = shellDescriptor.Features.Union(_assembliesAccessor.GetImportedAssemblies().Select(assembly => new ShellFeature { Name = assembly.FullName }));
            return shellDescriptor;
        }

        public void UpdateShellDescriptor(int priorSerialNumber, IEnumerable<ShellFeature> enabledFeatures, IEnumerable<ShellParameter> parameters)
        {
            _decorated.UpdateShellDescriptor(priorSerialNumber, enabledFeatures, parameters);
        }
    }
}
