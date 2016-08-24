using System.ServiceModel;
using System.ServiceModel.Web;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Text;
using Emc.Documentum.Rest.Net;

namespace Emc.Documentum.Rest.DataModel
{

    [DataContract(Name = "documentum-rest-services-product-info", Namespace = "http://identifiers.emc.com/vocab/documentum")]  
    public class ProductInfo : Linkable
    {
        [DataMember(Name = "properties")]
        public ProductInfoProperties Properties { get; set; }

        public override string ToString()
        {
            JsonDotnetJsonSerializer serializer = new JsonDotnetJsonSerializer();
            return serializer.Serialize(this);
        }
    }

    [DataContract(Name = "properties", Namespace = "http://identifiers.emc.com/vocab/documentum")]
    public class ProductInfoProperties
    {
        [DataMember(Name = "product")]
        public string Product { get; set; }

        [DataMember(Name = "product_version")]
        public string ProductVersion { get; set; }

        [DataMember(Name = "major")]
        public string Major { get; set; }

        [DataMember(Name = "minor")]
        public string Minor { get; set; }

        [DataMember(Name = "build_number")]
        public string BuildNumber { get; set; }

        [DataMember(Name = "revision_number")]
        public string RevisionNumber { get; set; }
    }
}
