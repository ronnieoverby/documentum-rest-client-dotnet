using Emc.Documentum.Rest.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Emc.Documentum.Rest.DataModel.D2
{
    [DataContract(Name = "profile-configuration", Namespace = "http://www.w3.org/2005/Atom")]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public class ProfileConfiguration : ExecLinkable
    {
        [DataMember(Name = "object_name")]
        public String Name { get; set; }

        [DataMember(Name = "title")]
        public String Title { get; set; }

        [DataMember(Name = "r_object_id")]
        public String ObjectId { get; set; }

        [DataMember(Name = "user_group")]
        public String UserGroup { get; set; }

        [DataMember(Name = "linked_document_profile")]
        public String LinkedDocumentProfile { get; set; }

        [DataMember(Name = "creation_profile")]
        public bool CreationProfile { get; set; }

        [DataMember(Name = "import_profile")]
        public bool ImportProfile { get; set; }

        [DataMember(Name = "skip_edit_content")]
        public bool SkipEditContent { get; set; }

        [DataMember(Name = "inherit_properties")]
        public bool InheritProperties { get; set; }

        [DataMember(Name = "inherit_content")]
        public bool InheritContent { get; set; }

        [DataMember(Name = "inherit_vd_struture")]
        public bool InheritVDStructure { get; set; }

        [DataMember(Name = "dictionary_names")]
        public List<String> DictionaryNames;

        [DataMember(Name = "attribute_names")]
        public List<String> AttributeNames;
    }
}

