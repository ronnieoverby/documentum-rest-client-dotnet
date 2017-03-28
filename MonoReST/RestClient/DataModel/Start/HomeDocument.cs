using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Emc.Documentum.Rest.Net;
using System.Runtime.InteropServices;

namespace Emc.Documentum.Rest.DataModel
{
     /// <summary>
     /// Home document resource model, which is the entry point of the entire Documentum REST Services
     /// </summary>
    [DataContract(Name = "services", Namespace = "http://identifiers.emc.com/vocab/documentum")]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public partial class HomeDocument
    {
        /// <summary>
        /// Root resources under home document
        /// </summary>
        [DataMember(Name = "resources")]
        public Resources Resources { 
            get; 
            set; 
        }

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

    /// <summary>
    /// Resources model within home document
    /// </summary>
    [DataContract(Name = "resources", Namespace = "http://identifiers.emc.com/vocab/documentum")]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public class Resources
    {
        /// <summary>
        /// Repositories entry
        /// </summary>
        [DataMember(Name = "http://identifiers.emc.com/linkrel/repositories")]
        public Resource Repositories { get; set; }

        /// <summary>
        /// Product info entry
        /// </summary>
        [DataMember(Name = "about")]
        public Resource About { get; set; }
    }

    /// <summary>
    ///  Resource model within home document
    /// </summary>
    [DataContract(Name = "resource", Namespace = "http://identifiers.emc.com/vocab/documentum")]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public class Resource
    {
        /// <summary>
        /// Href
        /// </summary>
        [DataMember(Name = "href")]
        public string Href { get; set; }

        /// <summary>
        /// Hints
        /// </summary>
        [DataMember(Name = "hints")]
        public Hints Hints { get; set; }

        /// <summary>
        /// To string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(
                "Resource{{href: {0}, hints_allow: {1}, hints_representations: {2}}}",
                this.Href,
                string.Join(", ", this.Hints.Allow.ToArray()),
                string.Join(", ", this.Hints.Representations.ToArray()));
            return builder.ToString();
        }

        
    }

    /// <summary>
    /// Hints model within home document
    /// </summary>
    [DataContract(Name = "hints", Namespace = "http://identifiers.emc.com/vocab/documentum")]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public class Hints
    {
        /// <summary>
        /// Allowed methods
        /// </summary>
        [DataMember(Name = "allow")]
        public List<string> Allow { get; set; }

        /// <summary>
        /// Supported media types
        /// </summary>
        [DataMember(Name = "representations")]
        public List<string> Representations { get; set; }
    }
}
