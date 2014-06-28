using Orchard.FileSystems.AppData;
using Orchard.FileSystems.VirtualPath;

namespace Lombiq.OrchardAppHost.Services
{
    public class AppHostAppDataFolderRoot : IAppDataFolderRoot
    {
        private readonly IVirtualPathProvider _virtualPathProvider;
        private readonly string _rootPath;


        public AppHostAppDataFolderRoot(IVirtualPathProvider virtualPathProvider, string rootPath = "~/App_Data")
        {
            _virtualPathProvider = virtualPathProvider;
            _rootPath = rootPath;
        }
    

        public string RootPath
        {
            get { return _rootPath; }
        }

        public string RootFolder
        {
            get { return _virtualPathProvider.MapPath(RootPath); }
        }
    }
}
