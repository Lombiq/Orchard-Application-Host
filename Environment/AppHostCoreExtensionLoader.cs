using System.Linq;
using Orchard.Environment;
using Orchard.Environment.Extensions.Loaders;
using Orchard.Environment.Extensions.Models;
using Orchard.FileSystems.Dependencies;

namespace Lombiq.OrchardAppHost.Environment
{
    /// <summary>
    /// <see cref="Orchard.Environment.Extensions.Loaders.IExtensionLoader"/> implementation for loading Core extensions 
    /// from arbitrary folders.
    /// </summary>
    /// <remarks>
    /// Only overriding <see cref="CoreExtensionLoader"/> to be able to load extensions with the Location not just "~/Core".
    /// </remarks>
    public class AppHostCoreExtensionLoader : CoreExtensionLoader
    {
        private readonly IExtensionPathsProvider _extensionPathsProvider;


        public AppHostCoreExtensionLoader(
            IDependenciesFolder dependenciesFolder,
            IAssemblyLoader assemblyLoader,
            IExtensionPathsProvider extensionPathsProvider)
            : base(dependenciesFolder, assemblyLoader)
        {
            _extensionPathsProvider = extensionPathsProvider;
        }


        public override ExtensionProbeEntry Probe(ExtensionDescriptor descriptor)
        {
            if (Disabled)
                return null;

            if (!string.IsNullOrEmpty(descriptor.Location) &&
                _extensionPathsProvider.GetExtensionPaths().CoreModuleFolderPaths.Any(path => path.Contains(descriptor.Location)))
            {
                return new ExtensionProbeEntry
                {
                    Descriptor = descriptor,
                    Loader = this,
                    Priority = 100, // Higher priority because assemblies in ~/bin always take precedence
                    VirtualPath = "~/Core/" + descriptor.Id,
                    VirtualPathDependencies = Enumerable.Empty<string>(),
                };
            }
            return null;
        }
    }
}
