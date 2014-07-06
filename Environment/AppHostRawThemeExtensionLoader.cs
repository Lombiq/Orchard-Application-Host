using System.Linq;
using Orchard.Environment.Extensions.Loaders;
using Orchard.Environment.Extensions.Models;
using Orchard.FileSystems.Dependencies;
using Orchard.FileSystems.VirtualPath;

namespace Lombiq.OrchardAppHost.Environment
{
    /// <summary>
    /// <see cref="Orchard.Environment.Extensions.Loaders.IExtensionLoader"/> implementation for loading raw themes (i.e. themes
    /// that don't have their own project but are rather just folders under the default Themes theme project) from an arbitrary 
    /// folder with the name called Core.
    /// </summary>
    /// <remarks>
    /// Only overriding <see cref="RawThemeExtensionLoader"/> to be able to load extensions with the Location not just "~/Themes".
    /// </remarks>
    public class AppHostRawThemeExtensionLoader : RawThemeExtensionLoader
    {
        private readonly IVirtualPathProvider _virtualPathProvider;
        private readonly IExtensionPathsProvider _extensionPathsProvider;


        public AppHostRawThemeExtensionLoader(
            IDependenciesFolder dependenciesFolder,
            IVirtualPathProvider virtualPathProvider,
            IExtensionPathsProvider extensionPathsProvider)
            : base(dependenciesFolder, virtualPathProvider)
        {
            _virtualPathProvider = virtualPathProvider;
            _extensionPathsProvider = extensionPathsProvider;
        }


        public override ExtensionProbeEntry Probe(ExtensionDescriptor descriptor)
        {
            if (Disabled)
                return null;

            if (!string.IsNullOrEmpty(descriptor.Location) && 
                _extensionPathsProvider.GetExtensionPaths().ThemeFolderPaths.Any(path => path.Contains(descriptor.Location)))
            {
                string projectPath = _virtualPathProvider.Combine(descriptor.Location, descriptor.Id,
                                           descriptor.Id + ".csproj");

                // ignore themes including a .csproj in this loader
                if (_virtualPathProvider.FileExists(projectPath))
                {
                    return null;
                }

                var assemblyPath = _virtualPathProvider.Combine(descriptor.Location, descriptor.Id, "bin",
                                                descriptor.Id + ".dll");

                // ignore themes with /bin in this loader
                if (_virtualPathProvider.FileExists(assemblyPath))
                    return null;

                return new ExtensionProbeEntry
                {
                    Descriptor = descriptor,
                    Loader = this,
                    VirtualPath = "~/Theme/" + descriptor.Id,
                    VirtualPathDependencies = Enumerable.Empty<string>(),
                };
            }
            return null;
        }
    }
}
