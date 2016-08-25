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
        ///  Executes a Full Text Query against this repository with simple search language
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="search"></param>
        /// <returns>Feed</returns>
        public Feed<T> ExecuteSimpleSearch<T>(SearchOptions search)
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
                    entry.Content = GetObjectByQualification<T>(String.Format("dm_sysobject where r_object_id='{0}'", entry.Id.ToString()), null);
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
        public Folder GetFolderByQualification(string dql, FeedGetOptions options)
        {
            return GetObjectByQualification<Folder>(dql, options);
        }

        /// <summary>
        /// Get a document by a Dql qualification 
        /// </summary>
        /// <param name="dql"></param>
        /// <param name="options"></param>
        /// <returns>RestDocument</returns>
        public Document GetDocumentByQualification(string dql, SingleGetOptions options)
        {
            return GetObjectByQualification<Document>(dql, options);
        }

        /// <summary>
        /// Get sysobject by ID.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectId"></param>
        /// <returns></returns>
        public T GetSysObjectById<T>(string objectId) where T : PersistentObject
        {
            SingleGetOptions options = new SingleGetOptions { Links = true };
            return GetSysObjectById<T>(objectId, options);
        }

        /// <summary>
        /// Get sysobject by object ID
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectId"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public T GetSysObjectById<T>(string objectId, SingleGetOptions options) where T : PersistentObject
        {
            return GetObjectByQualification<T>(String.Format("dm_sysobject where r_object_id='{0}'", objectId), options);
        }

        /// <summary>
        /// Get object by dql qualification
        /// </summary>
        /// <param name="dql"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public PersistentObject GetObjectByQualification(string dql, SingleGetOptions options)
        {
            return GetObjectByQualification<PersistentObject>(dql, options);
        }

        /// <summary>
        /// Get object by dql qualification
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dql"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public T GetObjectByQualification<T>(string dql, SingleGetOptions options)
        {
            dql = "select r_object_id from " + dql;
            Feed<PersistentObject> feed = ExecuteDQL<PersistentObject>(dql, new FeedGetOptions { Inline = false });
            if (feed.Entries == null || feed.Entries.Count == 0)
            {
                return default(T);
            }
            if (feed.Entries.Count > 1)
            {
                throw new Exception("The qualification '" + dql + "' has more than one object in repository.");
            }
            return Client.GetSingleton<T>(feed.Entries[0].Links, LinkRelations.EDIT.Rel, options);
        }

        private string _cabinetType = "dm_cabinet";
        /// <summary>
        /// When creating a new cabinet object, this will be the object type used. Defaults to 'dm_cabinet'.
        /// </summary>
        private string CabinetType
        {
            get { return _cabinetType; }
            set { _cabinetType = value; }
        }

        private string _documentType = "dm_document";
        /// <summary>
        /// When creating a new document object, this will be the object type used. Defaults to 'dm_document'.
        /// </summary>
        public string DocumentType
        {
            get { return _documentType; }
            set { _documentType = value; }
        }

        private string _folderType = "dm_folder";
        /// <summary>
        /// When creating a folder object, this will be the default type used. Defaults to 'dm_folder'.
        /// </summary>
        public string FolderType
        {
            get { return _folderType; }
            set { _folderType = value; }
        }

    } // End Repository Class
}
