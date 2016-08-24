using Emc.Documentum.Rest.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Emc.Documentum.Rest.DataModel.D2
{
    [DataContract(Name = "search-configuration", Namespace = "http://identifiers.emc.com/vocab/documentum")]
    public class SearchConfigurations : Linkable, Executable
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
        public SearchConfigLinkContent Content { get; set; }

        public SearchConfiguration getSearchConfiguration(String uri)
        {
           SearchConfiguration sConfig = Client.Get<SearchConfiguration>(uri, null);
           sConfig.SetClient(Client);
           return sConfig;

        }

        /// <summary>
        /// Entry for List of entries
        /// </summary>
        private List<SearchConfigLink> _entries = new List<SearchConfigLink>();
        [DataMember(Name = "entries")]
        public List<SearchConfigLink> Entries
        {
            get
            {
                if (_entries == null)
                {
                    _entries = new List<SearchConfigLink>();
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

    public class SearchConfigLink
    {
        public String id { get; set; }
        public String title { get; set; }
        public String summary { get; set; }
        public SearchConfigLinkContent content { get; set; }
    }


    /// <summary>
    /// Used to broker the object to an atom feed content object
    /// </summary>
    [DataContract(Name = "content", Namespace = "http://www.w3.org/2005/Atom")]
    public class SearchConfigLinkContent
    {
        /// <summary>
        /// The rest link to the object
        /// </summary>
        [DataMember(Name = "src")]
        public string Src { get; set; }
        /// <summary>
        /// The content type of the object (xml/json, etc)
        /// </summary>
        [DataMember(Name = "type")]
        public string ContentType { get; set; }

        /// <summary>
        /// Convenience method to override to string to give a simple src/content json representation
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(
                "Content{{src: {0}, content-type: {1}}}",
                this.Src,
                this.ContentType);
            return builder.ToString();
        }
    }


}
