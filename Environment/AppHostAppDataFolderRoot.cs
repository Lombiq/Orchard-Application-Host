using Orchard.FileSystems.AppData;
using Orchard.FileSystems.VirtualPath;

namespace Lombiq.OrchardAppHost.Environment
{
    /// <summary>
    /// <see cref="IAppDataFolderRoot"/> implementation for the Orchard App Host that doesn't make use of
    /// <see cref="System.Web.Hosting.HostingEnvironment"/> (since that would just work for web applications).
    /// </summary>
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
