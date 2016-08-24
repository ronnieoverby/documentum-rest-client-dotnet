using Emc.Documentum.Rest.Net;
using Emc.Documentum.Rest.Http.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace Emc.Documentum.Rest.DataModel
{
    /// <summary>
    /// Entry point to the datacontract
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [DataContract(Name = "feed", Namespace = "http://www.w3.org/2005/Atom")]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public class Feed<T> : Linkable, Executable
    {
        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "updated")]
        public string Updated { get; set; }

        [DataMember(Name = "total")]
        public int Total { get; set; }

        [DataMember(Name = "pageCount")]
        public double PageCount { get; set; }

        private List<Author> _authors = new List<Author>();
      
        /// <summary>
        /// List entry for Authors
        /// </summary>
        [DataMember(Name = "author")]
        public List<Author> Authors
        {
            get
            {
                if (_authors == null)
                {
                    _authors = new List<Author>();
                }
                return _authors;
            }
            set
            {
                _authors = value;
            }
        }
        /// <summary>
        /// Entry for List of entries
        /// </summary>
        private List<Entry<T>> _entries = new List<Entry<T>>();
        [DataMember(Name = "entries")]
        public List<Entry<T>> Entries
        {
            get
            {
                if (_entries == null)
                {
                    _entries = new List<Entry<T>>();
                }
                return _entries;
            }
            set
            {
                _entries = value;
            }
        }

        private RestController _client;
        public void SetClient(RestController client)
        {
            _client = client;
        }

        public RestController Client
        {
            get { return _client; }
            set { this._client = value; }
        }

        /// <summary>
        /// Find entry by atom entry title
        /// </summary>
        /// <typeparam name="R">Feed Type passed here</typeparam> 
        /// <param name="title"></param>
        /// <returns></returns>
        public R GetEntry<R>(string title) where R : Executable
        {
            string repositoryUri = AtomUtil.FindEntryHref(this, title);
            R obj = Client.Get<R>(repositoryUri, null);
            if (obj == null) return default(R);
            (obj as Executable).SetClient(Client);
            return obj;
        }

        /// <summary>
        /// Find inline entry content
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        public T FindInlineEntry(string title)
        {
            T entry = AtomUtil.FindInlineEntry(this, title);
            if (entry == null) return default(T);
            (entry as Executable).SetClient(this.Client);
            return entry;
        }

        /// <summary>
        /// Find entry by atom entry summary
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        public T FindInlineEntryBySummary(string title)
        {
            T entry = AtomUtil.FindInlineEntryBySummary(this, title);
            if (entry == null) return default(T); 
            (entry as Executable).SetClient(this.Client);
            return entry;
        }

        /// <summary>
        /// Get current page feed
        /// </summary>
        /// <returns></returns>
        public Feed<T> CurrentPage()
        {
            return Client.Self<Feed<T>>(this.Links);
        }

        /// <summary>
        /// Get next page feed
        /// </summary>
        /// <returns></returns>
        public Feed<T> NextPage()
        {
            return Client.GetFeed<T>(
                this.Links,
                Emc.Documentum.Rest.Http.Utility.LinkRelations.PAGING_NEXT.Rel,
                null);
        }

        /// <summary>
        /// Get previous page feed
        /// </summary>
        /// <returns></returns>
        public Feed<T> PreviousPage()
        {
            return Client.GetFeed<T>(
                this.Links,
                Emc.Documentum.Rest.Http.Utility.LinkRelations.PAGING_PREV.Rel,
                null);
        }

        /// <summary>
        /// Get first page feed
        /// </summary>
        /// <returns></returns>
        public Feed<T> FirstPage()
        {
            return Client.GetFeed<T>(
                this.Links,
                Emc.Documentum.Rest.Http.Utility.LinkRelations.PAGING_FIRST.Rel,
                null);
        }

        /// <summary>
        /// Get last page feed
        /// </summary>
        /// <returns></returns>
        public Feed<T> LastPage()
        {
            return Client.GetFeed<T>(
                this.Links,
                Emc.Documentum.Rest.Http.Utility.LinkRelations.PAGING_LAST.Rel,
                null);
        }

        public override string ToString()
        {
            JsonDotnetJsonSerializer serializer = new JsonDotnetJsonSerializer();
            return serializer.Serialize(this);
        }
    }

    /// <summary>
    /// Entryp point for Author
    /// </summary>
    [DataContract(Name = "author", Namespace = "http://www.w3.org/2005/Atom")]
    public class Author
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "url")]
        public string Url { get; set; }

        [DataMember(Name = "email")]
        public string Email { get; set; }

        public override string ToString()
        {
            JsonDotnetJsonSerializer serializer = new JsonDotnetJsonSerializer();
            return serializer.Serialize(this);
        }
    }
}
