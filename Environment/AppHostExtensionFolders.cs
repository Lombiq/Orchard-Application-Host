using System.Collections.Generic;
using System.Linq;
using Orchard.Environment.Extensions.Folders;
using Orchard.Environment.Extensions.Loaders;
using Orchard.Environment.Extensions.Models;

namespace Lombiq.OrchardAppHost.Environment
{
    /// <summary>
    /// <see cref="IExtensionFolders"/> implementation for harvesting extensions from configured Modules, Core and Themes 
    /// folders.
    /// </summary>
    /// <remarks>
    /// Does what <see cref="ModuleFolders"/>, <see cref="CoreModuleFolders"/> and <see cref="ThemeFolders"/> does.
    /// </remarks>
    public class AppHostExtensionFolders : IExtensionFolders
    {
        private readonly IExtensionPathsProvider _extensionPathsProvider;
        private readonly IExtensionHarvester _extensionHarvester;
        private readonly IEnumerable<IExtensionLoader> _extensionLoaders;


        public AppHostExtensionFolders(
            IExtensionPathsProvider extensionPathsProvider, 
            IExtensionHarvester extensionHarvester,
            IEnumerable<IExtensionLoader> extensionLoaders)
        {
            _extensionPathsProvider = extensionPathsProvider;
            _extensionHarvester = extensionHarvester;
            _extensionLoaders = extensionLoaders;
        }


        public IEnumerable<ExtensionDescriptor> AvailableExtensions()
        {
            var extensionPaths = _extensionPathsProvider.GetExtensionPaths();

            // Removing imported extensions. If an imported extension also has a Module.txt then this will prevent 
            // double-loading it.
            var importedExtensions = _extensionLoaders
                .OfType<ImportedExtensionsProvider>()
                .Single()
                .AvailableExtensions()
                .ToDictionary(extension => extension.Id);

            return
                _extensionHarvester
                .HarvestExtensions(extensionPaths.ModuleFolderPaths, DefaultExtensionTypes.Module, "Module.txt", false)
                .Union(_extensionHarvester.HarvestExtensions(extensionPaths.CoreModuleFolderPaths, DefaultExtensionTypes.Module, "Module.txt", false))
                .Union(_extensionHarvester.HarvestExtensions(extensionPaths.ThemeFolderPaths, DefaultExtensionTypes.Theme, "Theme.txt", false))
                .Where(extension => !importedExtensions.ContainsKey(extension.Id));
        }
    }
}
