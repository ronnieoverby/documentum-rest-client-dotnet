using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Emc.Documentum.Rest.DataModel.D2
{
    [DataContract(Name = "d2-configuration", Namespace = "http://identifiers.emc.com/vocab/documentum")]
    public class D2Configuration
    {
        [DataMember(Name = "start_version")]
        public Double StartVersion { get; set; }

        [DataMember(Name = "folder_id")]
        public String FolderId { get; set; }

        [DataMember(Name = "default_value_template")]
        public String DefaultValueTemplate { get; set; }

        [DataMember(Name = "inheritance_config")]
        public String InheritanceConfig { get; set; }

        [DataMember(Name = "inherited_id")]
        public String InheritedId { get; set; }

        [DataMember(Name = "inherit_content")]
        public bool InheritContent { get; set; }

        [DataMember(Name = "inherit_properties")]
        public bool InheritProperties { get; set; }

        [DataMember(Name = "lifecycle")]
        public String LifeCycle { get; set; }

        [DataMember(Name = "properties_string")]
        public String PropertiesString { get; set; }

        [DataMember(Name = "properties_xml")]
        public String PropertiesXML { get; set; }
    }
}
