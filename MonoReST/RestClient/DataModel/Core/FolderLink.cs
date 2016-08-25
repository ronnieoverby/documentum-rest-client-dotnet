using Emc.Documentum.Rest.Net;
using Emc.Documentum.Rest.Http.Utility;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Emc.Documentum.Rest.DataModel
{
    /// <summary>
    /// Folder link resource model
    /// </summary>
    [DataContract(Name = "folder-link", Namespace = "http://identifiers.emc.com/vocab/documentum")]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public partial class FolderLink : ExecLinkable
    {
        /// <summary>
        /// href
        /// </summary>
        [DataMember(Name = "href")]
        public string Href { get; set; }

        /// <summary>
        /// parent-id
        /// </summary>
        [DataMember(Name = "parent-id")]
        public string ParentId { get; set; }

        /// <summary>
        /// child-id
        /// </summary>
        [DataMember(Name = "child-id")]
        public string ChildId { get; set; }
    }
}
