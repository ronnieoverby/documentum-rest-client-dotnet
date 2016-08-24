using Emc.Documentum.Rest.Net;
using Emc.Documentum.Rest.Http.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Emc.Documentum.Rest.DataModel.D2;

namespace Emc.Documentum.Rest.DataModel
{
    /// <summary>
    /// Entry point for Folder 
    /// </summary>
    [DataContract(Name = "folder", Namespace = "http://identifiers.emc.com/vocab/documentum")]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public class Folder : PersistentObject
    {
        /// <summary>
        /// Whether the folder can be updated
        /// </summary>
        /// <returns>Returns Boolean value</returns>
        public new bool CanUpdate()
        {
            return LinkRelations.FindLinkAsString(this.getFullFolderLinks(), LinkRelations.EDIT.Rel) != null;
        }

        /// <summary>
        /// Whether the folder can be deleted
        /// </summary>
        /// <returns>Returns Boolean value</returns>
        public new bool CanDelete()
        {
            return LinkRelations.FindLinkAsString(this.getFullFolderLinks(), LinkRelations.DELETE.Rel) != null;
        }

        /// <summary>
        /// Whether the folder has parent folders
        /// </summary>
        /// <returns>Returns Boolean value</returns>
        public bool HasParent()
        {
            return LinkRelations.FindLinkAsString(this.getFullFolderLinks(), LinkRelations.PARENT.Rel) != null;
        }

        /// <summary>
        /// Get child folders from this folder
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="options"></param>
        /// <returns>Returns Rest Feed for the object</returns>
        public Feed<T> GetFolders<T>(FeedGetOptions options)
        {
            return Client.GetFeed<T>(
                this.getFullFolderLinks(),
                LinkRelations.FOLDERS.Rel,
                options);
        }

        /// <summary>
        /// Get child documents from this folder
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="options"></param>
        /// <returns>Returns Rest Feed for object</returns>
        public Feed<T> GetDocuments<T>(FeedGetOptions options)
        {
            return Client.GetFeed<T>(
                this.getFullFolderLinks(),
                LinkRelations.DOCUMENTS.Rel,
                options);
        }

        /// <summary>
        /// Get the cabinet resource where this folder locates at
        /// </summary>
        /// <param name="options"></param>
        /// <returns>Returns Cabinet object</returns>
        public new Cabinet GetCabinet(SingleGetOptions options)
        {
            return Client.GetSingleton<Cabinet>(
                this.getFullFolderLinks(),
                LinkRelations.CABINET.Rel,
                options);
        }

        /// <summary>
        /// Get the parent folder of this folder
        /// </summary>
        /// <param name="options"></param>
        /// <returns>Returns Folder object</returns>
        public new Folder GetParentFolder(SingleGetOptions options)
        {
            return Client.GetSingleton<Folder>(
                this.getFullFolderLinks(),
                LinkRelations.PARENT.Rel,
                options);
        }

        /// <summary>
        /// Create a folder resource under this folder
        /// </summary>
        /// <param name="newObj"></param>
        /// <param name="options"></param>
        /// <returns>Returns Folder object</returns>
        public Folder CreateSubFolder(Folder newObj, GenericOptions options)
        {
            if (newObj.Client == null) newObj.SetClient(this.Client);
            return Client.Post<Folder>(
                this.getFullFolderLinks(),
                LinkRelations.FOLDERS.Rel,
                newObj,
                options);
        }

        /// <summary>
        /// Create a document resource under this folder
        /// </summary>
        /// <param name="newObj"></param>
        /// <param name="options"></param>
        /// <returns>Returns RestDocument object</returns>
        public RestDocument CreateSubDocument(RestDocument newObj, GenericOptions options)
        {
            if (newObj.Client == null) newObj.SetClient(this.Client);

            return Client.Post<RestDocument>(
                this.getFullFolderLinks(),
                LinkRelations.DOCUMENTS.Rel,
                newObj,
                options);
        }

        /// <summary>
        /// Create a PersistentObject resource under this folder
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="newObj"></param>
        /// <param name="options"></param>
        /// <returns>Returns Type object</returns>
        public T CreateSubObject<T>(T newObj, GenericOptions options) where T : PersistentObject
        {
            return Client.Post<T>(
                this.getFullFolderLinks(),
                LinkRelations.OBJECTS.Rel,
                newObj,
                options);
        }

        /// <summary>
        /// Import a contentful document with content stream under this folder
        /// </summary>
        /// <param name="newObj"></param>
        /// <param name="otherPartStream"></param>
        /// <param name="otherPartMime"></param>
        /// <param name="options"></param>
        /// <returns>Returns RestDocument object</returns>
        public RestDocument ImportDocumentWithContent(RestDocument newObj, Stream otherPartStream, string otherPartMime, GenericOptions options)
        {
            Dictionary<Stream, string> otherParts = new Dictionary<Stream, string>();
            otherParts.Add(otherPartStream, otherPartMime);
            return Client.Post<RestDocument>(
                this.Links,
                LinkRelations.DOCUMENTS.Rel,
                newObj,
                otherParts,
                options);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="repositoryUri"></param>
        /// <param name="newObj"></param>
        /// <param name="otherPartStream"></param>
        /// <param name="otherPartMime"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public EmailPackage ImportEmail(RestDocument newObj, Stream otherPartStream, string otherPartMime, GenericOptions options)
        {
            Dictionary<Stream, string> otherParts = new Dictionary<Stream, string>();
            otherParts.Add(otherPartStream, otherPartMime);
            options.SetQuery("folderId", this.getAttributeValue("r_object_id"));
            Feed<RestDocument> feed = Client.Post<RestDocument, Feed<RestDocument>>(
                Client.RepositoryBaseUri + LinkRelations.EMAILIMPORT,
                newObj,
                otherParts,
                options);
            return ObjectUtil.getFeedAsEmailPackage(feed);

        }

        /// <summary>
        /// Link a PersistentObject resource to this folder
        /// </summary>
        /// <param name="newObj"></param>
        /// <param name="options"></param>
        /// <returns>Returns Folder link </returns>
        public FolderLink LinkFrom(RestDocument newObj, GenericOptions options)
        {
            return Client.Post<RestDocument, FolderLink>(
                this.getFullFolderLinks(),
                LinkRelations.CHILD_LINKS.Rel,
                newObj,
                options);
        }

        /// <summary>
        /// If a folder is a raw object, this method can be called to fetch the folder with its links
        /// </summary>
        /// <returns>Returns List</returns>
        private List<Link> getFullFolderLinks()
        {
            if (this.Links.Count == 1 && this.Links[0].Title.Equals("self"))
            {
                Repository repo = new Repository();
                repo.SetClient(Client);
                Folder fullFolder = repo.getObjectById<Folder>(this.getAttributeValue("r_object_id").ToString()); //getFolderByQualification("dm_folder where r_object_id = '"
                    //+ this.getAttributeValue("r_object_id") + "'", new FeedGetOptions() { Inline = true, Links = true });
                return fullFolder.Links;
            }
            else return this.Links;
        }
    }
}
