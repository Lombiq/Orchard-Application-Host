using System.Collections.Generic;
using System.Reflection;

namespace Lombiq.OrchardAppHost.Configuration
{
    public class AppHostSettings
    {
        public string AppDataFolderPath { get; set; }
        public IEnumerable<string> ModuleFolderPaths { get; set; }
        public IEnumerable<string> CoreModuleFolderPaths { get; set; }
        public IEnumerable<string> ThemeFolderPaths { get; set; }
        public IEnumerable<ShellExtensions> ImportedExtensions { get; set; }
        public bool DisableConfiguratonCaches { get; set; }
    }


    public class ShellExtensions
    {
        public string ShellName { get; set; }
        public IEnumerable<Assembly> Extensions { get; set; }
    }
}
