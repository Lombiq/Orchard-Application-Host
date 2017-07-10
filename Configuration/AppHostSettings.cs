using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using log4net.Repository;
using Orchard.Environment.Configuration;

namespace Lombiq.OrchardAppHost.Configuration
{
    public delegate void Log4NetConfigurator(ILoggerRepository loggerRepository);


    public class AppHostSettings
    {
        /// <summary>
        /// Gets or sets the path for the App_Data folder.
        /// </summary>
        public string AppDataFolderPath { get; set; } = "~/App_Data";

        /// <summary>
        /// Gets or sets paths where modules will be harvested from.
        /// </summary>
        public IEnumerable<string> ModuleFolderPaths { get; set; } = Enumerable.Empty<string>();

        /// <summary>
        /// Gets or sets paths where Core modules will be harvested from.
        /// </summary>
        public IEnumerable<string> CoreModuleFolderPaths { get; set; } = Enumerable.Empty<string>();

        /// <summary>
        /// Gets or sets paths where themes will be harvested from.
        /// </summary>
        public IEnumerable<string> ThemeFolderPaths { get; set; } = Enumerable.Empty<string>();

        /// <summary>
        /// These assemblies will be usable as extensions inside Orchard.
        /// </summary>
        public IEnumerable<Assembly> ImportedExtensions { get; set; } = Enumerable.Empty<Assembly>();

        /// <summary>
        /// You can use this to enable features on a shell by default.
        /// </summary>
        public IEnumerable<DefaultShellFeatureState> DefaultShellFeatureStates { get; set; } = Enumerable.Empty<DefaultShellFeatureState>();

        /// <summary>
        /// If set to true, configuration caches (e.g. SessionConfigurationCache, ShellDescriptorCache) will be disabled.
        /// Disable caches if the application runs on multiple instances with a common database.
        /// </summary>
        public bool DisableConfiguratonCaches { get; set; } = true;

        /// <summary>
        /// If set to true, the monitoring of extension folders (for newly installed or changed extensions) will be 
        /// disabled. Disabling extension monitoring saves memory but you won't be able to add new extensions in runtime.
        /// </summary>
        public bool DisableExtensionMonitoring { get; set; } = true;

        /// <summary>
        /// Delegate for altering the default Orchard Log4Net configuration.
        /// </summary>
        public Log4NetConfigurator Log4NetConfigurator { get; set; } = repository => { };
    }


    public class DefaultShellFeatureState
    {
        /// <summary>
        /// The name of the shell in question.
        /// </summary>
        public string ShellName { get; set; } = ShellSettings.DefaultName;

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
