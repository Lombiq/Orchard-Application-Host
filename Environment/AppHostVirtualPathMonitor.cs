using System;
using System.IO;
using Orchard.Caching;
using Orchard.FileSystems.VirtualPath;

namespace Lombiq.OrchardAppHost.Environment
{
    /// <summary>
    /// <see cref="IVirtualPathMonitor"/> implementation for the Orchard App Host that doesn't make use of
    /// <see cref="System.Web.Hosting.HostingEnvironment"/> (since that would just work for web applications).
    /// </summary>
    /// <remarks>
    /// Copied from <see cref="Orchard.Commands.CommandHostVirtualPathMonitor"/> because it's internal.
    /// </remarks>
    public class AppHostVirtualPathMonitor : IVirtualPathMonitor
    {
        private readonly IVirtualPathProvider _virtualPathProvider;


        public AppHostVirtualPathMonitor(IVirtualPathProvider virtualPathProvider)
        {
            _virtualPathProvider = virtualPathProvider;
        }


        public IVolatileToken WhenPathChanges(string virtualPath)
        {
            var filename = _virtualPathProvider.MapPath(virtualPath);
            if (File.Exists(filename))
            {
                return new FileToken(filename);
            }
            if (Directory.Exists(filename))
            {
                return new DirectoryToken(filename);
            }
            return new EmptyVolativeToken(filename);
        }


        public class EmptyVolativeToken : IVolatileToken
        {
            private readonly string _filename;

            public EmptyVolativeToken(string filename)
            {
                _filename = filename;
            }

            public bool IsCurrent
            {
                get
                {
                    if (Directory.Exists(_filename))
                    {
                        return false;
                    }
                    if (File.Exists(_filename))
                    {
                        return false;
                    }
                    return true;
                }
            }
        }

        public class FileToken : IVolatileToken
        {
            private readonly string _filename;
            private readonly DateTime _lastWriteTimeUtc;

            public FileToken(string filename)
            {
                _filename = filename;
                _lastWriteTimeUtc = File.GetLastWriteTimeUtc(filename);
            }

            public bool IsCurrent
            {
                get
                {
                    try
                    {
                        if (_lastWriteTimeUtc != File.GetLastWriteTimeUtc(_filename))
                            return false;
                    }
                    catch
                    {
                        return false;
                    }
                    return true;
                }
            }
        }

        public class DirectoryToken : IVolatileToken
        {
            private readonly string _filename;
            private readonly DateTime _lastWriteTimeUtc;

            public DirectoryToken(string filename)
            {
                _filename = filename;
                _lastWriteTimeUtc = Directory.GetLastWriteTimeUtc(filename);
            }

            public bool IsCurrent
            {
                get
                {
                    try
                    {
                        if (_lastWriteTimeUtc != Directory.GetLastWriteTimeUtc(_filename))
                            return false;
                    }
                    catch
                    {
                        return false;
                    }
                    return true;
                }
            }
        }
    }
}
