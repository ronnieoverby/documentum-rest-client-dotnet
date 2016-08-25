using System.ServiceModel;
using System.ServiceModel.Web;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Text;
using Emc.Documentum.Rest.Net;
using System.Runtime.InteropServices;

namespace Emc.Documentum.Rest.DataModel
{
    /// <summary>
    /// Product info resource model
    /// </summary>
    [DataContract(Name = "documentum-rest-services-product-info", Namespace = "http://identifiers.emc.com/vocab/documentum")]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public class ProductInfo : Linkable
    {
        /// <summary>
        /// Properties of product info
        /// </summary>
        [DataMember(Name = "properties")]
        public ProductInfoProperties Properties { get; set; }
    }

    /// <summary>
    /// Product info properties
    /// </summary>
    [DataContract(Name = "properties", Namespace = "http://identifiers.emc.com/vocab/documentum")]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public class ProductInfoProperties
    {
        /// <summary>
        /// Product name
        /// </summary>
        [DataMember(Name = "product")]
        public string Product { get; set; }

        /// <summary>
        /// Product version
        /// </summary>
        [DataMember(Name = "product_version")]
        public string ProductVersion { get; set; }

        /// <summary>
        /// Product major version
        /// </summary>
        [DataMember(Name = "major")]
        public string Major { get; set; }

        /// <summary>
        /// Product minor version
        /// </summary>
        [DataMember(Name = "minor")]
        public string Minor { get; set; }

        /// <summary>
        /// Product build number
        /// </summary>
        [DataMember(Name = "build_number")]
        public string BuildNumber { get; set; }

        /// <summary>
        /// Product revision number
        /// </summary>
        [DataMember(Name = "revision_number")]
        public string RevisionNumber { get; set; }
    }
}
