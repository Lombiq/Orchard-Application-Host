using System;
using System.Collections.Generic;
using System.IO;
using Orchard.FileSystems.VirtualPath;

namespace Lombiq.OrchardAppHost.Environment
{
    /// <summary>
    /// <see cref="IVirtualPathProvider"/> implementation for the Orchard App Host that doesn't make use of
    /// <see cref="System.Web.Hosting.HostingEnvironment"/> (since that would just work for web applications).
    /// </summary>
    public class AppHostVirtualPathProvider : DefaultVirtualPathProvider, IVirtualPathProvider
    {
        public override string MapPath(string virtualPath)
        {
            if (virtualPath.StartsWith("~"))
            {
                virtualPath = virtualPath.Substring(1);
            }
            if (virtualPath.StartsWith("/"))
            {
                virtualPath = virtualPath.Substring(1);
            }
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, virtualPath);
        }

        public override bool FileExists(string virtualPath)
        {
            return File.Exists(MapPath(virtualPath));
        }

        public override Stream OpenFile(string virtualPath)
        {
            return File.OpenRead(MapPath(virtualPath));
        }

        string IVirtualPathProvider.GetFileHash(string virtualPath)
        {
            return ((IVirtualPathProvider)this).GetFileHash(virtualPath, new[] { virtualPath });
        }

        string IVirtualPathProvider.GetFileHash(string virtualPath, IEnumerable<string> dependencies)
        {
            // There seems to be no easy implementation for this.
            throw new NotImplementedException();
        }

        public override void DeleteFile(string virtualPath)
        {
            File.Delete(MapPath(virtualPath));
        }

        public override bool DirectoryExists(string virtualPath)
        {
            return Directory.Exists(MapPath(virtualPath));
        }

        public override IEnumerable<string> ListFiles(string path)
        {
            return Directory.EnumerateFiles(MapPath(path));
        }

        public override IEnumerable<string> ListDirectories(string path)
        {
            return Directory.EnumerateDirectories(MapPath(path));
        }
    }
}
