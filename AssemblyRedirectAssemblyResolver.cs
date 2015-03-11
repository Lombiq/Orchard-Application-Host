using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Lombiq.OrchardAppHost
{
    /// <summary>
    /// Implements assembly redirections instead using a static configuration file.
    /// </summary>
    internal static class AssemblyRedirectAssemblyResolver
    {
        public static Assembly ResolveAssembly(object sender, ResolveEventArgs args)
        {
            var assemblyShortName = args.Name.Split(',')[0];

            switch (assemblyShortName)
            {
                case "Autofac":
                    return typeof(Autofac.ContainerBuilder).Assembly;
                case "NHibernate":
                    return typeof(NHibernate.ADOException).Assembly;
                default:
                    return null;
            }
        }
    }
}
