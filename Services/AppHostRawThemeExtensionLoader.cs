using System.Linq;
using Orchard.Environment.Extensions.Loaders;
using Orchard.Environment.Extensions.Models;
using Orchard.FileSystems.Dependencies;
using Orchard.FileSystems.VirtualPath;

namespace Lombiq.OrchardAppHost.Services
{
    // Only overriding RawThemeExtensionLoader to be able to load extensions with the Location not just "~/Themes".
    public class AppHostRawThemeExtensionLoader : RawThemeExtensionLoader
    {
        private readonly IVirtualPathProvider _virtualPathProvider;


        public AppHostRawThemeExtensionLoader(IDependenciesFolder dependenciesFolder, IVirtualPathProvider virtualPathProvider)
            : base(dependenciesFolder, virtualPathProvider)
        {
            _virtualPathProvider = virtualPathProvider;
        }


        public override ExtensionProbeEntry Probe(ExtensionDescriptor descriptor)
        {
            if (Disabled)
                return null;

            if (descriptor.Location.Contains("/Themes") || descriptor.Location.Contains(@"\Themes"))
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
