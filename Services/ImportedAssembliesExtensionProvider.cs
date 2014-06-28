using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Orchard.Environment.Extensions;
using Orchard.Environment.Extensions.Folders;
using Orchard.Environment.Extensions.Loaders;
using Orchard.Environment.Extensions.Models;
using Orchard.FileSystems.Dependencies;

namespace Lombiq.OrchardAppHost.Services
{
    public class ImportedAssembliesExtensionProvider : ExtensionLoaderBase, IExtensionFolders
    {
        private readonly IEnumerable<Assembly> _assemblies;

        public override int Order { get { return 10; } }


        public ImportedAssembliesExtensionProvider(
            IDependenciesFolder dependenciesFolder,
            IEnumerable<Assembly> assemblies)
            : base(dependenciesFolder)
        {
            _assemblies = assemblies;
        }


        public IEnumerable<ExtensionDescriptor> AvailableExtensions()
        {
            return _assemblies.
                Select(assembly =>
                    {
                        var extensionDescriptor = new ExtensionDescriptor
                        {
                            Location = string.Empty,
                            Id = assembly.FullName,
                            ExtensionType = DefaultExtensionTypes.Module
                        };

                        var features = new[] { new FeatureDescriptor
                        {
                            Extension = extensionDescriptor,
                            Id = extensionDescriptor.Id
                        }};

                        extensionDescriptor.Features = features;

                        return extensionDescriptor;
                    });
        }

        public override ExtensionProbeEntry Probe(ExtensionDescriptor descriptor)
        {
            throw new NotImplementedException();
        }

        protected override ExtensionEntry LoadWorker(ExtensionDescriptor descriptor)
        {
            throw new NotImplementedException();
        }
    }
}
