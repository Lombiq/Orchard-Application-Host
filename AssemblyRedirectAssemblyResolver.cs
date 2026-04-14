using Iesi.Collections.Generic;
using Newtonsoft.Json;
using System;
using System.Reflection;

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
                case "Iesi.Collections":
                    return typeof(ReadOnlySet<>).Assembly;
                case "Newtonsoft.Json":
                    return typeof(JsonSerializer).Assembly;
                default:
                    return null;
            }
        }
    }
}
