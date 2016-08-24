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
    [DataContract(Name = "feed", Namespace = "http://www.w3.org/2005/Atom")]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public class SearchFeed<T> : Linkable, Executable
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
        

        private List<SearchEntry<T>> _entries = new List<SearchEntry<T>>();
        [DataMember(Name = "entries")]
        public List<SearchEntry<T>> Entries
        {
            get
            {
                if (_entries == null)
                {
                    _entries = new List<SearchEntry<T>>();
                }
                return _entries;
            }
            set
            {
                _entries = value;
            }
        }

        public override string ToString()
        {
            JsonDotnetJsonSerializer serializer = new JsonDotnetJsonSerializer();
            return serializer.Serialize(this);
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
        /// Get current page SearchFeed
        /// </summary>
        /// <returns></returns>
        public  SearchFeed<T> CurrentPage()
        {
            return Client.Self<SearchFeed<T>>(this.Links);
        }

        /// <summary>
        /// Get next page SearchFeed
        /// </summary>
        /// <returns></returns>
        public SearchFeed<T> NextPage()
        {
            return Client.GetSearchFeed<T>(
                this.Links,
                Emc.Documentum.Rest.Http.Utility.LinkRelations.PAGING_NEXT.Rel,
                null);
        }

        /// <summary>
        /// Get previous page SearchFeed
        /// </summary>
        /// <returns></returns>
        public  SearchFeed<T> PreviousPage()
        {
            return Client.GetSearchFeed<T>(
                this.Links,
                Emc.Documentum.Rest.Http.Utility.LinkRelations.PAGING_PREV.Rel,
                null);
        }

        /// <summary>
        /// Get first page SearchFeed
        /// </summary>
        /// <returns></returns>
        public  SearchFeed<T> FirstPage()
        {
            return Client.GetSearchFeed<T>(
                Links,
                Emc.Documentum.Rest.Http.Utility.LinkRelations.PAGING_FIRST.Rel,
                null);
        }

        /// <summary>
        /// Get last page SearchFeed
        /// </summary>
        /// <returns></returns>
        public SearchFeed<T> LastPage()
        {
            return Client.GetSearchFeed<T>(
                this.Links,
                Emc.Documentum.Rest.Http.Utility.LinkRelations.PAGING_LAST.Rel,
                null);
        }
    }

}
