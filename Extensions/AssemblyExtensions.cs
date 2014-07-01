
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
