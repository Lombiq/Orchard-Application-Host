
namespace System.Reflection
{
    public static class AssemblyExtensions
    {
        /// <summary>
        /// Returns the short name of an <see cref="Assembly"/> (i.e. the same, human readable anem, without version 
        /// number or thumbprints that is also the project name).
        /// </summary>
        /// <param name="assembly">A reference to the <see cref="Assembly"/>.</param>
        public static string ShortName(this Assembly assembly)
        {
            return assembly.FullName.ToAssemblyShortName();
        }
    }
}
