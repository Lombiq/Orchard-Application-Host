using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Lombiq.OrchardAppHost.Configuration
{
    public class AppHostSettings
    {
        /// <summary>
        /// Gets or sets the path for the App_Data folder.
        /// </summary>
        public string AppDataFolderPath { get; set; }

        /// <summary>
        /// Gets or sets paths where modules will be harvested from.
        /// </summary>
        public IEnumerable<string> ModuleFolderPaths { get; set; }

        /// <summary>
        /// Gets or sets paths where Core modules will be harvested from.
        /// </summary>
        public IEnumerable<string> CoreModuleFolderPaths { get; set; }

        /// <summary>
        /// Gets or sets paths where themes will be harvested from.
        /// </summary>
        public IEnumerable<string> ThemeFolderPaths { get; set; }

        /// <summary>
        /// These assemblies will be usable as extensions inside Orchard.
        /// </summary>
        public IEnumerable<Assembly> ImportedExtensions { get; set; }

        /// <summary>
        /// You can use this to enable features on a shell by default.
        /// </summary>
        public IEnumerable<DefaultShellFeatureState> DefaultShellFeatureStates { get; set; }

        /// <summary>
        /// If set configuration caches (e.g. SessionConfigurationCache, ShellDescriptorCache) will be disabled.
        /// Disable caches if the application runs on multiple instances with a common database.
        /// </summary>
        public bool DisableConfiguratonCaches { get; set; }


        public AppHostSettings()
        {
            AppDataFolderPath = "~/App_Data";
            ModuleFolderPaths = Enumerable.Empty<string>();
            CoreModuleFolderPaths = Enumerable.Empty<string>();
            ThemeFolderPaths = Enumerable.Empty<string>();
            ImportedExtensions = Enumerable.Empty<Assembly>();
            DefaultShellFeatureStates = Enumerable.Empty<DefaultShellFeatureState>();
        }
    }


    public class DefaultShellFeatureState
    {
        /// <summary>
        /// The name of the shell in question.
        /// </summary>
        public string ShellName { get; set; }

        /// <summary>
        /// IDs of features to enable by default.
        /// </summary>
        public IEnumerable<string> EnabledFeatures { get; set; }


        public DefaultShellFeatureState()
        {
            EnabledFeatures = Enumerable.Empty<string>();
        }
    }
}
