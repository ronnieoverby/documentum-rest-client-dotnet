using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Emc.Documentum.Rest.DataModel;
using Emc.Documentum.Rest.Net;

namespace Emc.Documentum.Rest.DataModel.D2
{
    [DataContract(Name = "search-configuration", Namespace = "http://www.w3.org/2005/Atom")]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public class SearchConfiguration : Linkable, Executable
    {
        [DataMember(Name = "object_name")]
        public String Name { get; set; }

        [DataMember(Name = "title")]
        public String Title { get; set; }

        [DataMember(Name = "applications")]
        public List<String> Applications { get; set; }

        [DataMember(Name = "search_properties")]
        public List<SearchProperty> SearchProperties;

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

        public override string ToString()
        {
            JsonDotnetJsonSerializer serializer = new JsonDotnetJsonSerializer();
            return serializer.Serialize(this);
        }
    }

    [DataContract(Name = "item", Namespace = "http://www.w3.org/2005/Atom")]
    public class SearchProperty
    {
        [DataMember(Name = "property_name")]
        public String PropertyName { get; set; }

        [DataMember(Name = "property_type")]
        public String PropertyType { get; set; }

        [DataMember(Name = "is_facet")]
        public bool IsFacet { get; set; }

        [DataMember(Name = "is_default_facet")]
        public bool IsDefaultFacet { get; set; }

        [DataMember(Name = "facet_position")]
        public int FacetPosition { get; set; }

        [DataMember(Name = "facet_indent")]
        public int FacetIndent { get; set; }

        [DataMember(Name = "facet_sort")]
        public String FacetSort { get; set; }

        [DataMember(Name = "alias_local_name")]
        public String AliasLocalName { get; set; }

        [DataMember(Name = "dictionary_name")]
        public String DictionaryName { get; set; }

        [DataMember(Name = "dql")]
        public String Dql { get; set; }

        public override string ToString()
        {
            JsonDotnetJsonSerializer serializer = new JsonDotnetJsonSerializer();
            return serializer.Serialize(this);
        }
    }
}
