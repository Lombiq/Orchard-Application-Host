using System.Collections.Generic;
using System.Reflection;
using Orchard.Environment.Configuration;
using System.Linq;
using Lombiq.OrchardAppHost.Configuration;

namespace Lombiq.OrchardAppHost.Services.Extensions
{
    /// <summary>
    /// Gets assemblies imported to be registered as extensions when configuring the OrchardAppHost.
    /// </summary>
    public interface IImportedExtensionsAccessor
    {
        IEnumerable<Assembly> GetImportedExtensions();
    }


    public class ImportedExtensionsAccessor : IImportedExtensionsAccessor
    {
        private readonly ShellSettings _shellSettings;
        private readonly IEnumerable<ShellExtensions> _extensions;


        public ImportedExtensionsAccessor(ShellSettings shellSettings, IEnumerable<ShellExtensions> extensions)
        {
            _shellSettings = shellSettings;
            _extensions = extensions;
        }


        public IEnumerable<Assembly> GetImportedExtensions()
        {
            return _extensions.Where(extensions => extensions.ShellName == _shellSettings.Name).SelectMany(extensions => extensions.Extensions);
        }
    }
}
