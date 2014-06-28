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
        public IEnumerable<Assembly> ImportedExtensions { get; set; }
    }
}
