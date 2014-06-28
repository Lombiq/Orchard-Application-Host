using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Lombiq.OrchardAppHost.Helpers;
using Orchard.Environment;

namespace Lombiq.OrchardAppHost.Services
{
    public delegate void AppDomainRestartRequestedEventHandler();

    public class AppHostEnvironment : IHostEnvironment
    {
        public event AppDomainRestartRequestedEventHandler AppDomainRestartRequested;


        public bool IsFullTrust
        {
            get { return AppDomain.CurrentDomain.IsHomogenous && AppDomain.CurrentDomain.IsFullyTrusted; }
        }

        public string MapPath(string virtualPath)
        {
            return PathHelpers.MapPath(virtualPath);
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
