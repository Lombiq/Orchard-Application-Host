using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lombiq.OrchardAppHost.Helpers
{
    internal static class PathHelpers
    {
        public static string MapPath(string virtualPath)
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
    }
}
