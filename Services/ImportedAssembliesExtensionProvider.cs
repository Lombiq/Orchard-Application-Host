using System;
using System.Collections.Generic;
using System.IO;
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
    /// <summary>
    /// Deals with extensions loaded on the fly when configuring the OrchardAppHost.
    /// </summary>
    public class ImportedAssembliesExtensionProvider : ExtensionLoaderBase, IExtensionFolders
    {
        private readonly IImportedAssembliesAccessor _assembliesAccessor;

        public override int Order { get { return 10; } }


        public ImportedAssembliesExtensionProvider(
            IDependenciesFolder dependenciesFolder,
            IImportedAssembliesAccessor assembliesAccessor)
            : base(dependenciesFolder)
        {
            _assembliesAccessor = assembliesAccessor;
        }


        public IEnumerable<ExtensionDescriptor> AvailableExtensions()
        {
            return _assembliesAccessor.GetImportedAssemblies().
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
            var path = Path.Combine(descriptor.Location, descriptor.Id, ".dll");

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
            var assembly = _assembliesAccessor.GetImportedAssemblies().SingleOrDefault(a => a.FullName == descriptor.Id);

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
