using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Lombiq.OrchardAppHost.Configuration;
using Orchard.Environment.Extensions;
using Orchard.Environment.Extensions.Folders;
using Orchard.Environment.Extensions.Loaders;
using Orchard.Environment.Extensions.Models;
using Orchard.FileSystems.Dependencies;
using Orchard.FileSystems.VirtualPath;

namespace Lombiq.OrchardAppHost.Services
{
    /// <summary>
    /// Deals with extensions loaded on the fly when configuring the OrchardAppHost.
    /// </summary>
    public class ImportedExtensionsProvider : ExtensionLoaderBase, IExtensionFolders
    {
        private readonly IEnumerable<Assembly> _extensions;
        private readonly IVirtualPathProvider _virtualPathProvider;

        public override int Order { get { return 10; } }


        public ImportedExtensionsProvider(
            IDependenciesFolder dependenciesFolder,
            IEnumerable<Assembly> extensions,
            IVirtualPathProvider virtualPathProvider)
            : base(dependenciesFolder)
        {
            _extensions = extensions;
            _virtualPathProvider = virtualPathProvider;
        }


        public IEnumerable<ExtensionDescriptor> AvailableExtensions()
        {
            return _extensions.
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
            var path = _virtualPathProvider.Combine(descriptor.Location, descriptor.Id, ".dll");

            return new ExtensionProbeEntry
            {
                Descriptor = descriptor,
                Loader = this,
                VirtualPath = path,
                VirtualPathDependencies = new[] { path },
            };
        }

        protected override ExtensionEntry LoadWorker(ExtensionDescriptor descriptor)
        {
            var assembly = _extensions.SingleOrDefault(a => a.FullName == descriptor.Id);

            if (assembly == null) return null;

            return new ExtensionEntry
            {
                Descriptor = descriptor,
                Assembly = assembly,
                ExportedTypes = assembly.GetExportedTypes()
            };
        }
    }
}
