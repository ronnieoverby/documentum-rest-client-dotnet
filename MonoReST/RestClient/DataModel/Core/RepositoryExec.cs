using Emc.Documentum.Rest.Net;
using Emc.Documentum.Rest.Http.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Net.Http;
using System.IO;
using System.IO.Compression;

namespace Emc.Documentum.Rest.DataModel
{
    public partial class Repository 
    {
        /// <summary>
        /// Get current login user resource
        /// </summary>
        /// <param name="options"></param>
        /// <returns>Feed</returns>
        public User GetCurrentUser(SingleGetOptions options)
        {
            return Client.GetSingleton<User>(
                this.Links,
                LinkRelations.CURRENT_USER.Rel,
                options);
        }


        /// <summary>
        /// Get cabinets feed in this repository
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="options"></param>
        /// <returns>Feed</returns>
        public Feed<T> GetCabinets<T>(FeedGetOptions options)
        {
            return Client.GetFeed<T>(
                this.Links,
                Emc.Documentum.Rest.Http.Utility.LinkRelations.CABINETS.Rel,
                options);
        }

        /// <summary>
        /// Get users feed in this repository
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="options"></param>
        /// <returns>Feed</returns>
        public Feed<T> GetUsers<T>(FeedGetOptions options)
        {
            return Client.GetFeed<T>(
                this.Links,
                Emc.Documentum.Rest.Http.Utility.LinkRelations.USERS.Rel,
                options);
        }

        /// <summary>
        /// Get groups in this repository
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="options"></param>
        /// <returns>Feed</returns>
        public Feed<T> GetGroups<T>(FeedGetOptions options)
        {
            return Client.GetFeed<T>(
                this.Links,
                Emc.Documentum.Rest.Http.Utility.LinkRelations.GROUPS.Rel,
                options);
        }

        /// <summary>
        ///  Get checked out PersistentObjects in this repository
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="options"></param>
        /// <returns>Feed</returns>
        public Feed<T> GetCheckedOutObjects<T>(FeedGetOptions options)
        {
            return Client.GetFeed<T>(
                this.Links,
                Emc.Documentum.Rest.Http.Utility.LinkRelations.CHECKED_OUT_OBJECTS.Rel,
                options);
        }

        /// <summary>
        /// Execute a DQL query in this repository
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dql"></param>
        /// <param name="options"></param>
        /// <returns>Feed</returns>
        public Feed<T> ExecuteDQL<T>(string dql, FeedGetOptions options)
        {
            decimal count = 0;
            double pageCount = 0;

            if (options == null) options = new FeedGetOptions();
            string dqlUri = LinkRelations.FindLinkAsString(this.Links, LinkRelations.DQL.Rel);
            string dqlUriWithoutTemplateParams = dqlUri.Substring(0, dqlUri.IndexOf("{"));

            /******************** BEGIN GET TOTAL IF SPECIFIED *****************************/
            if (options != null && options.IncludeTotal)
            {
                String countDql = "select count(*) as total " + dql.Substring(dql.IndexOf("from"));
                List<KeyValuePair<string, object>> cl = options.ToQueryList();
                cl.Add(new KeyValuePair<string, object>("dql", countDql));
                Feed<PersistentObject> countFeed = Client.Get<Feed<PersistentObject>>(dqlUriWithoutTemplateParams, cl);
                List<Entry<PersistentObject>> res = (List<Entry<PersistentObject>>)countFeed.Entries;

                foreach (Entry<PersistentObject> obj in res) // There will only be one result
                {
                    count = decimal.Parse(obj.Content.GetPropertyValue("total").ToString());
                }
            }
            /********************* END GET TOTAL IF SPECIFIED ******************************/

            // Now execute the real query
            List<KeyValuePair<string, object>> pa = options.ToQueryList();
            pa.Add(new KeyValuePair<string, object>("dql", dql));
            Feed<T> feed = this.Client.Get<Feed<T>>(dqlUriWithoutTemplateParams, pa);
            if (feed != null)
            {
                feed.Client = Client;
            }

            if (count > 0)
            {
                feed.Total = (int)count;
                int itemsPerPage = (options.ItemsPerPage > 0) ? options.ItemsPerPage : feed.Total;
                pageCount = Math.Ceiling((double)count / itemsPerPage);
                feed.PageCount = pageCount;
            }
            return feed;
        }


        /// <summary>
        ///  Executes a Full Text Query against this repository
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="search"></param>
        /// <returns>Feed</returns>
        public Feed<T> ExecuteSearch<T>(SearchOptions search)
        {
            decimal count = 0;
            double pageCount = 0;

            if (search == null) search = new SearchOptions();
            string searchUri = LinkRelations.FindLinkAsString(this.Links, LinkRelations.SEARCH.Rel);
            searchUri = searchUri.Substring(0, searchUri.IndexOf("{"));
            List<KeyValuePair<string, object>> pa = search.ToQueryList();
            Feed<T> feed = this.Client.Get<Feed<T>>(searchUri, pa);
            if (feed != null)
            {
                feed.Client = Client;
            }
            count = feed == null ? 0 : feed.Total;

            if (count > 0)
            {
                int itemsPerPage = (search.ItemsPerPage > 0) ? search.ItemsPerPage : feed.Total;
                pageCount = Math.Ceiling((double)count / itemsPerPage);
                feed.PageCount = pageCount;
            }
            long tStart = DateTime.Now.Ticks;
            // If raw search is not specified, get the full object for each item returned on the page
            if (!search.Raw && feed != null)
            {
                foreach (Entry<T> entry in feed.Entries)
                {
                    entry.Content = getObjectByQualification<T>(String.Format("dm_sysobject where r_object_id='{0}'", entry.Id.ToString()), null);
                }
            }

            return feed;

        }

        /// <summary>
        /// This gets a folder object from the feed then fetches the full folder object. 
        /// </summary>
        /// <param name="dql"></param>
        /// <param name="options"></param>
        /// <returns>Folder</returns>
        public Folder getFolderByQualification(string dql, FeedGetOptions options)
        {
            dql = "select * from " + dql;
            string dqlUri = LinkRelations.FindLinkAsString(this.Links, LinkRelations.DQL.Rel);
            string dqlUriWithoutTemplateParams = dqlUri.Substring(0, dqlUri.IndexOf("{"));
            List<KeyValuePair<string, object>> pa = options == null ? new FeedGetOptions().ToQueryList() : options.ToQueryList();
            pa.Add(new KeyValuePair<string, object>("dql", dql));
            Feed<Folder> feed = this.Client.Get<Feed<Folder>>(dqlUriWithoutTemplateParams, pa);

            List<Entry<Folder>> folders = feed == null ? new List<Entry<Folder>>() : feed.Entries;
            if (folders.Count == 0)
            {
                return null;
            }
            else
            {
                string folderId = folders[0].Content.GetPropertyValue("r_object_id").ToString();
                Folder folder = getObjectById<Folder>(folderId);
                //Folder folder =  _client.Get<Folder>(folders[0].Content.Links[0].Href);
                folder.SetClient(this.Client);
                return folder;
            }

        }

        /// <summary>
        /// Get a document by a Dql qualification 
        /// </summary>
        /// <param name="dql"></param>
        /// <param name="options"></param>
        /// <returns>RestDocument</returns>
        public Document getDocumentByQualification(string dql, FeedGetOptions options)
        {
            dql = "select * from " + dql;
            string dqlUri = LinkRelations.FindLinkAsString(this.Links, LinkRelations.DQL.Rel);
            string dqlUriWithoutTemplateParams = dqlUri.Substring(0, dqlUri.IndexOf("{"));
            List<KeyValuePair<string, object>> pa = options == null ? new FeedGetOptions().ToQueryList() : options.ToQueryList();
            pa.Add(new KeyValuePair<string, object>("dql", dql));
            Feed<Document> feed = this.Client.Get<Feed<Document>>(dqlUriWithoutTemplateParams, pa);

            List<Entry<Document>> docs = (List<Entry<Document>>)feed.Entries;
            if (docs.Count == 0)
            {
                return null;
            }
            else
            {
                Document doc = _client.Get<Document>(docs[0].Content.Links[0].Href, null);
                if (doc != null) doc.SetClient(this.Client);
                return doc;
            }

        }

        public T getObjectById<T>(string objectId) where T : PersistentObject
        {
            SingleGetOptions options = new SingleGetOptions { Links = true };
            return getObjectById<T>(objectId, options);
        }

        public T getObjectById<T>(string objectId, SingleGetOptions options) where T : PersistentObject
        {
            return null;
            //todo
            // Client.GetObjectById<T>(objectId, options);
        }

        public PersistentObject getObjectByQualification(string dql, FeedGetOptions options)
        {
            return getObjectByQualification<PersistentObject>(dql, options);
        }

        public T getObjectByQualification<T>(string dql, FeedGetOptions options)
        {
            dql = "select * from " + dql;
            string dqlUri = LinkRelations.FindLinkAsString(this.Links, LinkRelations.DQL.Rel);
            string dqlUriWithoutTemplateParams = dqlUri.Substring(0, dqlUri.IndexOf("{"));
            List<KeyValuePair<string, object>> pa = options == null ? new FeedGetOptions().ToQueryList() : options.ToQueryList();
            pa.Add(new KeyValuePair<string, object>("dql", dql));
            Feed<PersistentObject> feed = this.Client.Get<Feed<PersistentObject>>(dqlUriWithoutTemplateParams, pa);

            List<Entry<PersistentObject>> objects = (List<Entry<PersistentObject>>)feed.Entries;
            if (objects.Count == 0)
            {
                return default(T);
            }
            else
            {
                T obj = _client.Get<T>(objects[0].Content.Links[0].Href, null);
                if (obj != null) (obj as Executable).SetClient(Client);
                return obj;
            }

        }

        private string _cabinetType = "dm_cabinet";
        /// <summary>
        /// When creating a new cabinet object, this will be the object type used.
        /// </summary>
        private string CabinetType
        {
            get { return _cabinetType; }
            set { _cabinetType = value; }
        }

        private string _documentType = "dm_document";
        public string DocumentType
        {
            get { return _documentType; }
            set { _documentType = value; }
        }

        private string _folderType = "dm_folder";
        /// <summary>
        /// When creating a folder object, this will be the default type used.
        /// </summary>
        public string FolderType
        {
            get { return _folderType; }
            set { _folderType = value; }
        }

    } // End Repository Class
}
