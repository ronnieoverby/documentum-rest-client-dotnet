using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Newtonsoft.Json.Linq;
using Emc.Documentum.Rest.Net;
using Emc.Documentum.Rest.Http.Utility;
using System.Runtime.InteropServices;

namespace Emc.Documentum.Rest.DataModel
{
    public partial class PersistentObject
    {
        /// <summary>
        /// Whether the object can be updated
        /// </summary>
        /// <returns>Returns Boolean value</returns>
        public bool CanUpdate()
        {
            return LinkRelations.FindLinkAsString(GetFullLinks(), LinkRelations.EDIT.Rel) != null;
        }

        /// <summary>
        /// Whether the object can be deleted
        /// </summary>
        /// <returns></returns>
        public bool CanDelete()
        {
            return LinkRelations.FindLinkAsString(GetFullLinks(), LinkRelations.DELETE.Rel) != null;
        }

        /// <summary>
        /// Get the cabinet resource where this object locates at 
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public Cabinet GetCabinet(SingleGetOptions options)
        {
            return Client.GetSingleton<Cabinet>(
                GetFullLinks(),
                LinkRelations.CABINET.Rel,
                options);
        }

        /// <summary>
        /// Get the parent folder resource of this object
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public Folder GetParentFolder(SingleGetOptions options)
        {
            return Client.GetSingleton<Folder>(
                GetFullLinks(),
                LinkRelations.PARENT.Rel,
                options);
        }

        /// <summary>
        /// Get folder links feed of this object
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public Feed<FolderLink> GetFolderLinks(FeedGetOptions options)
        {
            return Client.GetFeed<FolderLink>(
                GetFullLinks(),
                LinkRelations.PARENT_LINKS.Rel,
                options);
        }

        /// <summary>
        /// Link this object to a new folder
        /// </summary>
        /// <param name="newObj"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public FolderLink LinkToFolder(Folder newObj, GenericOptions options)
        {
           
            return Client.Post<Folder, FolderLink>(
                GetFullLinks(),
                LinkRelations.PARENT_LINKS.Rel,
                newObj,
                options);
        }

        /// <summary>
        /// Get current persistent object resource
        /// </summary>
        /// <returns></returns>
        public T Fetch<T>() where T : PersistentObject
        {
            return Client.Self<T>(this.Links);
        }

        /// <summary>
        /// Paritially update the persistent object resource
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        private T Save<T>(T obj) where T : PersistentObject
        {
            PersistentObject tp = obj as PersistentObject;
          
            T updated = Client.Post<T>(SelfLink(), obj);
            updated.Client = Client;
            return updated;
        }

        /// <summary>
        /// Save object after properties update
        /// </summary>
        public void Save() 
        {
            
            Dictionary<string, object> tempProp = _properties;
            _properties = ChangedProperties;
            Client.Post(SelfLink(), this);
            ChangedProperties = new Dictionary<string, object>();
            _properties = tempProp;
        }

        /// <summary>
        /// Completely update the persistent object resource
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public T CompleteUpdate<T>(T obj) where T : PersistentObject
        {
            T updated = Client.Put<T>(SelfLink(), obj);
            updated.Client = Client;
            return updated;
        }

        /// <summary>
        /// Delete the persistent object resource
        /// </summary>
        /// <param name="options"></param>
        public void Delete(GenericOptions options)
        {
            Client.Delete(SelfLink(), options == null ? null : options.ToQueryList());
        }

        /// <summary>
        /// Get a reference object representation of this resource with a href link
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T CreateHrefObject<T>() where T : PersistentObject
        {
            T hrefObj = (T)Activator.CreateInstance(typeof(T));
            hrefObj.Href = SelfLink();
            hrefObj.SetClient(this.Client);
            return hrefObj;
        }

       /// <summary>
       /// Get a copy of an object
       /// </summary>
       /// <typeparam name="T"></typeparam>
       /// <returns></returns>
        public T CreateCopyObject<T>() where T : PersistentObject
        {
            T hrefObj = (T)Activator.CreateInstance(typeof(T));
            hrefObj.Href = SelfLink();
            hrefObj.SetClient(this.Client);
            return hrefObj;
        }

       /// <summary>
       /// Self link to the item 
       /// </summary>
       /// <returns></returns>
        protected string SelfLink()
        {
            string self = LinkRelations.FindLinkAsString(this.Links, LinkRelations.EDIT.Rel);
            if (self == null)
            {
                self = LinkRelations.FindLinkAsString(this.Links, LinkRelations.SELF.Rel);
            }
            return self;
        }

        /// <summary>
        /// If a persistent object is a raw object, this method can be called to fetch the folder with its links
        /// </summary>
        /// <returns>Returns List</returns>
        protected List<Link> GetFullLinks()
        {
            if (this.Links.Count == 1 && this.Links[0].Title.Equals(LinkRelations.SELF.Rel))
            {
                SingleGetOptions options = new SingleGetOptions { Links = true };
                PersistentObject refreshed = Client.GetSingleton<PersistentObject>(this.Links, LinkRelations.SELF.Rel, options);
                this.Links = refreshed.Links;
            }
            return this.Links;
        }
    }
}
