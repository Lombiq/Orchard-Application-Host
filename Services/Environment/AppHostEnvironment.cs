using System;
using System.Linq;
using System.Reflection;
using Orchard.Environment;
using Orchard.FileSystems.VirtualPath;

namespace Lombiq.OrchardAppHost.Services.Environment
{
    public delegate void AppDomainRestartRequestedEventHandler();

    public class AppHostEnvironment : IHostEnvironment
    {
        private readonly IVirtualPathProvider _virtualPathProvider;

        public event AppDomainRestartRequestedEventHandler AppDomainRestartRequested;


        public AppHostEnvironment(IVirtualPathProvider virtualPathProvider)
        {
            _virtualPathProvider = virtualPathProvider;
        }
        

        public bool IsFullTrust
        {
            get { return AppDomain.CurrentDomain.IsHomogenous && AppDomain.CurrentDomain.IsFullyTrusted; }
        }

        public string MapPath(string virtualPath)
        {
            return _virtualPathProvider.MapPath(virtualPath);
        }

        public bool IsAssemblyLoaded(string name)
        {
            return AppDomain.CurrentDomain.GetAssemblies().Any(assembly => new AssemblyName(assembly.FullName).Name == name);
        }

        public void RestartAppDomain()
        {
            if (AppDomainRestartRequested != null)
            {
                AppDomainRestartRequested();
            }
        }
    }
}
