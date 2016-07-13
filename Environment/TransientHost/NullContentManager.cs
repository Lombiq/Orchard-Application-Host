using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Orchard.ContentManagement;
using Orchard.ContentManagement.MetaData.Models;
using Orchard.Environment.Extensions;
using Orchard.Indexing;

namespace Lombiq.OrchardAppHost.Environment.TransientHost
{
    /// <summary>
    /// Needed because BackgroundService calls ContentManager.Clear().
    /// </summary>
    [OrchardFeature("Lombiq.OrchardAppHost.TransientHost")]
    [OrchardSuppressDependency("Orchard.ContentManagement.DefaultContentManager")]
    public class NullContentManager : IContentManager
    {
        public IEnumerable<ContentTypeDefinition> GetContentTypeDefinitions()
        {
            throw new NotImplementedException();
        }

        public ContentItem New(string contentType)
        {
            throw new NotImplementedException();
        }

        public void Create(ContentItem contentItem)
        {
            throw new NotImplementedException();
        }

        public void Create(ContentItem contentItem, VersionOptions options)
        {
            throw new NotImplementedException();
        }

        public ContentItem Clone(ContentItem contentItem)
        {
            throw new NotImplementedException();
        }

        public ContentItem Restore(ContentItem contentItem, VersionOptions options)
        {
            throw new NotImplementedException();
        }

        public ContentItem Get(int id)
        {
            throw new NotImplementedException();
        }

        public ContentItem Get(int id, VersionOptions options)
        {
            throw new NotImplementedException();
        }

        public ContentItem Get(int id, VersionOptions options, QueryHints hints)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ContentItem> GetAllVersions(int id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> GetMany<T>(IEnumerable<int> ids, VersionOptions options, QueryHints hints) where T : class, IContent
        {
            throw new NotImplementedException();
        }

        public IEnumerable<T> GetManyByVersionId<T>(IEnumerable<int> versionRecordIds, QueryHints hints) where T : class, IContent
        {
            throw new NotImplementedException();
        }

        public IEnumerable<ContentItem> GetManyByVersionId(IEnumerable<int> versionRecordIds, QueryHints hints)
        {
            throw new NotImplementedException();
        }

        public void Publish(ContentItem contentItem)
        {
            throw new NotImplementedException();
        }

        public void Unpublish(ContentItem contentItem)
        {
            throw new NotImplementedException();
        }

        public void Remove(ContentItem contentItem)
        {
            throw new NotImplementedException();
        }

        public void Destroy(ContentItem contentItem)
        {
            throw new NotImplementedException();
        }

        public void Index(ContentItem contentItem, IDocumentIndex documentIndex)
        {
            throw new NotImplementedException();
        }

        public XElement Export(ContentItem contentItem)
        {
            throw new NotImplementedException();
        }

        public void Import(XElement element, ImportContentSession importContentSession)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
        }

        public IContentQuery<ContentItem> Query()
        {
            throw new NotImplementedException();
        }

        public IHqlQuery HqlQuery()
        {
            throw new NotImplementedException();
        }

        public ContentItemMetadata GetItemMetadata(IContent contentItem)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<GroupInfo> GetEditorGroupInfos(IContent contentItem)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<GroupInfo> GetDisplayGroupInfos(IContent contentItem)
        {
            throw new NotImplementedException();
        }

        public GroupInfo GetEditorGroupInfo(IContent contentItem, string groupInfoId)
        {
            throw new NotImplementedException();
        }

        public GroupInfo GetDisplayGroupInfo(IContent contentItem, string groupInfoId)
        {
            throw new NotImplementedException();
        }

        public ContentItem ResolveIdentity(ContentIdentity contentIdentity)
        {
            throw new NotImplementedException();
        }

        public dynamic BuildDisplay(IContent content, string displayType = "", string groupId = "")
        {
            throw new NotImplementedException();
        }

        public dynamic BuildEditor(IContent content, string groupId = "")
        {
            throw new NotImplementedException();
        }

        public dynamic UpdateEditor(IContent content, IUpdateModel updater, string groupId = "")
        {
            throw new NotImplementedException();
        }

        public void CompleteImport(XElement element, ImportContentSession importContentSession)
        {
            throw new NotImplementedException();
        }
    }
}
