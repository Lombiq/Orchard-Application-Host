using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Orchard.FileSystems.AppData;

namespace Lombiq.OrchardAppHost.Services
{
    public class AppHostAppDataFolderRoot : IAppDataFolderRoot
    {
        public string RootPath
        {
            get { return "~/App_Data"; }
        }

        public string RootFolder
        {
            get { return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data"); }
        }
    }
}
