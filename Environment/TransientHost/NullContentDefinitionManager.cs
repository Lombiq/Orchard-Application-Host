using System;
using System.Collections.Generic;
using Orchard.ContentManagement.MetaData;
using Orchard.ContentManagement.MetaData.Models;
using Orchard.Environment.Extensions;

namespace Lombiq.OrchardAppHost.Services.TransientHost
{
    /// <summary>
    /// Used in transient hosts to prevent exceptions caused by instantiating the default <see cref="IContentDefinitionManager"/>
    /// implementation due to transient hosts not having persistence configured.
    /// </summary>
    [OrchardFeature("Lombiq.OrchardAppHost.TransientHost")]
    public class NullContentDefinitionManager : IContentDefinitionManager
    {
        public IEnumerable<ContentTypeDefinition> ListTypeDefinitions()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ContentPartDefinition> ListPartDefinitions()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ContentFieldDefinition> ListFieldDefinitions()
        {
            throw new NotImplementedException();
        }

        public ContentTypeDefinition GetTypeDefinition(string name)
        {
            throw new NotImplementedException();
        }

        public ContentPartDefinition GetPartDefinition(string name)
        {
            throw new NotImplementedException();
        }

        public void DeleteTypeDefinition(string name)
        {
            throw new NotImplementedException();
        }

        public void DeletePartDefinition(string name)
        {
            throw new NotImplementedException();
        }

        public void StoreTypeDefinition(ContentTypeDefinition contentTypeDefinition)
        {
            throw new NotImplementedException();
        }

        public void StorePartDefinition(ContentPartDefinition contentPartDefinition)
        {
            throw new NotImplementedException();
        }
    }
}
