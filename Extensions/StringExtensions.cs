
namespace System
{
    internal static class StringExtensions
    {
        /// <summary>
        /// Returns the short name of an <see cref="Assembly"/> (i.e. the same, human readable anem, without version number or thumbprints
        /// that is also the project name).
        /// </summary>
        public static string ToAssemblyShortName(this string s)
        {
            return s.Split(',')[0];
        }
    }
}
