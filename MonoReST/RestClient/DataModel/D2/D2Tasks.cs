using Emc.Documentum.Rest.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Emc.Documentum.Rest.DataModel.D2
{
    [DataContract(Name = "task list", Namespace = "http://identifiers.emc.com/vocab/documentum")]
    public class D2Tasks : ExecLinkable
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

        [DataMember(Name = "content", IsRequired = false)]
        public ProfileConfigLinkContent Content { get; set; }



        /// <summary>
        /// Entry for List of entries
        /// </summary>
        private List<D2Task> _entries = new List<D2Task>();
        [DataMember(Name = "entries")]
        public List<D2Task> Entries
        {
            get
            {
                if (_entries == null)
                {
                    _entries = new List<D2Task>();
                }
                return _entries;
            }
            set
            {
                _entries = value;
            }
        }
    }
}
