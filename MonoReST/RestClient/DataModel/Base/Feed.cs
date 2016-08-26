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
    /// Feed model
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [DataContract(Name = "feed", Namespace = "http://www.w3.org/2005/Atom")]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public class Feed<T> : ExecLinkable
    {
        /// <summary>
        /// Feed id
        /// </summary>
        [DataMember(Name = "id")]
        public string Id { get; set; }

        /// <summary>
        /// Feed title
        /// </summary>
        [DataMember(Name = "title")]
        public string Title { get; set; }

        /// <summary>
        /// Feed updated
        /// </summary>
        [DataMember(Name = "updated")]
        public string Updated { get; set; }

        /// <summary>
        /// Total number of entries at server side
        /// </summary>
        [DataMember(Name = "total")]
        public int Total { get; set; }

        /// <summary>
        /// Number of items per page
        /// </summary>
        [DataMember(Name = "items-per-page")]
        public int ItemsPerPage { get; set; }

        /// <summary>
        /// Current page number
        /// </summary>
        [DataMember(Name = "page")]
        public int Page { get; set; }

        /// <summary>
        /// The number of page
        /// </summary>
        public double PageCount 
        { 
            get
            {
                double pageCount = -1;
                if (Total > 0)
                {
                    pageCount = Math.Ceiling((double) Total / ItemsPerPage);
                }
                return pageCount;
            }
        }

        private List<Author> _authors = new List<Author>();
      
        /// <summary>
        /// Feed authors
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

        public override void SetClient(RestController client)
        {
            base.SetClient(client);
            if (this.Entries != null)
            {
                foreach(Entry<T> entry in this.Entries)
                {
                    if (entry is Executable) (entry as Executable).SetClient(client);
                    if (entry.Content is Executable) (entry.Content as Executable).SetClient(client);
                }
            }
        }

        /// <summary>
        /// List of entries
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
            return entry;
        }

        /// <summary>
        /// Re-get current page feed
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
    }

    /// <summary>
    /// Entry point for Author
    /// </summary>
    [DataContract(Name = "author", Namespace = "http://www.w3.org/2005/Atom")]
    public class Author
    {
        /// <summary>
        /// Author name
        /// </summary>
        [DataMember(Name = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Author url
        /// </summary>
        [DataMember(Name = "url")]
        public string Url { get; set; }

        /// <summary>
        /// Author email
        /// </summary>
        [DataMember(Name = "email")]
        public string Email { get; set; }

        /// <summary>
        /// To string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            JsonDotnetJsonSerializer serializer = new JsonDotnetJsonSerializer();
            return serializer.Serialize(this);
        }
    }
}
