using System;
using System.Linq;
using System.Reflection;
using Orchard.Environment;
using Orchard.FileSystems.VirtualPath;

namespace Lombiq.OrchardAppHost.Environment
{
    public delegate void AppDomainRestartRequestedEventHandler();


    /// <summary>
    /// <see cref="IHostEnvironment"/> implementation for the Orchard App Host that doesn't make use of
    /// <see cref="System.Web.Hosting.HostingEnvironment"/> (since that would just work for web applications).
    /// </summary>
    public class AppHostEnvironment : IHostEnvironment
    {
        private readonly IVirtualPathProvider _virtualPathProvider;

        /// <summary>
        /// Fired when RestartAppDomain() is called. Can be used to handle the app domain restart externally.
        /// It seems that currently an app domain restart is not needed, the App Host works properly.
        /// </summary>
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
