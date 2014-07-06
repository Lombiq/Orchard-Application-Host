using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lombiq.OrchardAppHost.Configuration;
using Orchard.FileSystems.VirtualPath;

namespace Lombiq.OrchardAppHost.Environment
{
    public interface IExtensionPaths
    {
        IEnumerable<string> ModuleFolderPaths { get; }
        IEnumerable<string> CoreModuleFolderPaths { get; }
        IEnumerable<string> ThemeFolderPaths { get; }
    }


    public interface IExtensionPathsProvider
    {
        IExtensionPaths GetExtensionPaths();
    }


    public class ExtensionPathsProvider : IExtensionPathsProvider
    {
        private readonly AppHostSettings _hostSettings;
        private readonly IVirtualPathProvider _virtualPathProvider;

        private readonly object _locker = new object();
        private IExtensionPaths _paths = null;


        public ExtensionPathsProvider(AppHostSettings hostSettings, IVirtualPathProvider virtualPathProvider)
        {
            _hostSettings = hostSettings;
            _virtualPathProvider = virtualPathProvider;
        }


        public IExtensionPaths GetExtensionPaths()
        {
            lock (_locker)
            {
                if (_paths == null)
                {
                    _paths = new ExtensionPaths
                    {
                        ModuleFolderPaths = _hostSettings.ModuleFolderPaths.Select(path => _virtualPathProvider.MapPath(path)),
                        CoreModuleFolderPaths = _hostSettings.CoreModuleFolderPaths.Select(path => _virtualPathProvider.MapPath(path)),
                        ThemeFolderPaths = _hostSettings.ThemeFolderPaths.Select(path => _virtualPathProvider.MapPath(path))
                    };
                }

                return _paths;
            }
        }


        private class ExtensionPaths : IExtensionPaths
        {
            public IEnumerable<string> ModuleFolderPaths { get; set; }
            public IEnumerable<string> CoreModuleFolderPaths { get; set; }
            public IEnumerable<string> ThemeFolderPaths { get; set; }
        }
    }
}
