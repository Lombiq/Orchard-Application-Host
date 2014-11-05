using System.IO;
using Orchard.Environment.Extensions;
using Orchard.FileSystems.VirtualPath;
using Orchard.FileSystems.WebSite;

namespace Lombiq.OrchardAppHost.Environment
{
    /// <summary>
    /// <see cref="IWebSiteFolder"/> implementation for the Orchard App Host that doesn't make use of
    /// <see cref="System.Web.Hosting.HostingEnvironment"/> (since that would just work for web applications).
    /// </summary>
    [OrchardSuppressDependency("Orchard.FileSystems.WebSite.WebSiteFolder")]
    public class AppHostWebSiteFolder : WebSiteFolder, IWebSiteFolder
    {
        private readonly IVirtualPathProvider _virtualPathProvider;


        public AppHostWebSiteFolder(IVirtualPathMonitor virtualPathMonitor, IVirtualPathProvider virtualPathProvider)
            : base(virtualPathMonitor, virtualPathProvider)
        {
            _virtualPathProvider = virtualPathProvider;
        }
        
    
        string IWebSiteFolder.ReadFile(string virtualPath)
        {
            return ((IWebSiteFolder)this).ReadFile(virtualPath, false);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
        string IWebSiteFolder.ReadFile(string virtualPath, bool actualContent)
        {
            if (!_virtualPathProvider.FileExists(virtualPath))
            {
                return null;
            }

            if (actualContent)
            {
                var physicalPath = _virtualPathProvider.MapPath(virtualPath);
                using (var stream = File.Open(physicalPath, FileMode.Open, FileAccess.Read))
                {
                    using (var reader = new StreamReader(stream))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
            else
            {
                using (var stream = _virtualPathProvider.OpenFile(virtualPath))
                {
                    using (var reader = new StreamReader(stream))
                    {
                        return reader.ReadToEnd();
                    }
                }
            }
        }

        void IWebSiteFolder.CopyFileTo(string virtualPath, Stream destination)
        {
            ((IWebSiteFolder)this).CopyFileTo(virtualPath, destination, false/*actualContent*/);
        }

        void IWebSiteFolder.CopyFileTo(string virtualPath, Stream destination, bool actualContent)
        {
            if (actualContent)
            {
                // This is an unfortunate side-effect of the dynamic compilation work.
                // Orchard has a custom virtual path provider which adds "<@Assembly xxx@>"
                // directives to WebForm view files. There are cases when this side effect
                // is not expected by the consumer of the WebSiteFolder API.
                // The workaround here is to go directly to the file system.
                var physicalPath = _virtualPathProvider.MapPath(virtualPath);
                using (var stream = File.Open(physicalPath, FileMode.Open, FileAccess.Read))
                {
                    stream.CopyTo(destination);
                }
            }
            else
            {
                using (var stream = _virtualPathProvider.OpenFile(virtualPath))
                {
                    stream.CopyTo(destination);
                }
            }
        }
    }
}
