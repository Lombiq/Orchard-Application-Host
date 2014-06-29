using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Reflection
{
    internal static class AssemblyExtensions
    {
        public static string ShortName(this Assembly assembly)
        {
            return assembly.FullName.ToAssemblyShortName();
        }
    }
}
