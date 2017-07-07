using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Orchard.Environment.Extensions;
using Orchard.Environment.Extensions.Folders;
using Orchard.Environment.Extensions.Loaders;
using Orchard.Environment.Extensions.Models;
using Orchard.FileSystems.Dependencies;
using Orchard.FileSystems.VirtualPath;

namespace Lombiq.OrchardAppHost.Environment
{
    /// <summary>
    /// Deals with extensions loaded on the fly when configuring the Orchard App Host.
    /// </summary>
    public class ImportedExtensionsProvider : ExtensionLoaderBase, IExtensionFolders
    {
        private readonly Dictionary<string, Extension> _extensionsByName;
        private readonly IVirtualPathProvider _virtualPathProvider;

        public override int Order { get { return 10; } }


        public ImportedExtensionsProvider(
            IDependenciesFolder dependenciesFolder,
            IEnumerable<Assembly> extensions,
            IVirtualPathProvider virtualPathProvider)
            : base(dependenciesFolder)
        {
            // It's not nice to do this in the ctor but this way we spare to implement locking that would be needed with 
            // lazy-loading and this class will be instantiated once anyway.
            _extensionsByName = extensions
                .ToDictionary
                (
                    assembly => assembly.ShortName(),
                    assembly => new Extension
                    {
                        Assembly = assembly,
                        Features = assembly
                                .GetExportedTypes()
                                .Where(type => type.GetCustomAttribute(typeof(OrchardFeatureAttribute)) != null)
                                .Select(type => ((OrchardFeatureAttribute)type.GetCustomAttribute(typeof(OrchardFeatureAttribute))).FeatureName)
                                .Union(new[] { assembly.ShortName() })
                                .Distinct()
                    }
                );

            _virtualPathProvider = virtualPathProvider;
        }


        public IEnumerable<ExtensionDescriptor> AvailableExtensions()
        {
            return _extensionsByName.
                Select(extensionByName =>
                {
                    var extensionDescriptor = new ExtensionDescriptor
                    {
                        Location = string.Empty,
                        Id = extensionByName.Key,
                        ExtensionType = DefaultExtensionTypes.Module,
                        Path = extensionByName.Key
                    };

                    var features = extensionByName.Value.Features.Select(feature =>
                        new FeatureDescriptor
                        {
                            Extension = extensionDescriptor,
                            Id = feature
                        });

                    extensionDescriptor.Features = features;

                    return extensionDescriptor;
                });
        }

        public override ExtensionProbeEntry Probe(ExtensionDescriptor descriptor)
        {
            if (!_extensionsByName.ContainsKey(descriptor.Id)) return null;

            var path = _virtualPathProvider.Combine(descriptor.Location, descriptor.Id + ".dll");

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
            if (!_extensionsByName.ContainsKey(descriptor.Id)) return null;

            var assembly = _extensionsByName[descriptor.Id].Assembly;

            return new ExtensionEntry
            {
                Descriptor = descriptor,
                Assembly = assembly,
                ExportedTypes = assembly.GetExportedTypes()
            };
        }


        private class Extension
        {
            public Assembly Assembly { get; set; }
            public IEnumerable<string> Features { get; set; }
        }
    }
}
