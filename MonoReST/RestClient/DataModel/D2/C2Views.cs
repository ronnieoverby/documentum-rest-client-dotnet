using Emc.Documentum.Rest.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Emc.Documentum.Rest.DataModel.D2
{
    [DataContract(Name = "c2-views", Namespace = "http://identifiers.emc.com/vocab/documentum")]
    public class C2Views : Linkable, Executable
    {
        [DataMember(Name = "id")]
        public String id { get; set; }

        [DataMember(Name = "title")]
        public String title { get; set; }

        /// <summary>
        /// Authors of the feed entry
        /// </summary>
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

        [DataMember(Name = "updated", IsRequired = false)]
        public String Updated { get; set; }

        [DataMember(Name = "page", IsRequired = false)]
        public int Page { get; set; }

        [DataMember(Name = "total", IsRequired = false)]
        public int Total { get; set; }


        /// <summary>
        /// Entry for List of entries
        /// </summary>
        private List<C2ViewEntry> _entries = new List<C2ViewEntry>();
        [DataMember(Name = "entries")]
        public List<C2ViewEntry> Entries
        {
            get
            {
                if (_entries == null)
                {
                    _entries = new List<C2ViewEntry>();
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

    }
}
