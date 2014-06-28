using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orchard;
using Orchard.Environment.Configuration;

namespace Lombiq.OrchardAppHost
{
    public interface IOrchardAppHost : IDisposable
    {
        void Startup();
        void Run(string shellName, Action<IWorkContextScope> kernel);
    }


    public static class OrchardAppHostExtensions
    {
        public static void Run(this IOrchardAppHost appHost, Action<IWorkContextScope> kernel)
        {
            appHost.Run(ShellSettings.DefaultName, kernel);
        }
    }
}
