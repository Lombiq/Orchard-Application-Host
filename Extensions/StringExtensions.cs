using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
    internal static class StringExtensions
    {
        public static string ToAssemblyShortName(this string s)
        {
            return s.Split(',')[0];
        }
    }
}
