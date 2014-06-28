﻿using System.Collections.Generic;
using System.Reflection;

namespace Lombiq.OrchardAppHost.Services
{
    /// <summary>
    /// Gets assemblies imported to be registered as extensions when configuring the OrchardAppHost.
    /// </summary>
    public interface IImportedAssembliesAccessor
    {
        IEnumerable<Assembly> GetImportedAssemblies();
    }


    public class ImportedAssembliesAccessor : IImportedAssembliesAccessor
    {
        private readonly IEnumerable<Assembly> _assemblies;


        public ImportedAssembliesAccessor(IEnumerable<Assembly> assemblies)
        {
            _assemblies = assemblies;
        }
        
    
        public IEnumerable<Assembly> GetImportedAssemblies()
        {
            return _assemblies;
        }
    }
}
