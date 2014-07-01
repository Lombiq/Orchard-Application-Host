
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
