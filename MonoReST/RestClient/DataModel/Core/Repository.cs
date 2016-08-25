using Emc.Documentum.Rest.Net;
using Emc.Documentum.Rest.Http.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Net.Http;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices;

namespace Emc.Documentum.Rest.DataModel
{
    /// <summary>
    /// Repository resource model
    /// </summary>
    [DataContract(Name = "repository", Namespace = "http://identifiers.emc.com/vocab/documentum")]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public partial class Repository : ExecLinkable
    {
        /// <summary>
        /// Repository id
        /// </summary>
        [DataMember(Name = "id")]
        public string Id { get; set; }

        /// <summary>
        /// Repository name
        /// </summary>
        [DataMember(Name = "name")]
        public string Name { get; set; }

        /// <summary>
        /// Repository description
        /// </summary>
        [DataMember(Name = "description")]
        public string Description { get; set; }

        /// <summary>
        /// List of content servers for the repository
        /// </summary>
        [DataMember(Name = "servers")]
        public List<Server> Servers { get; set; }
    }

    /// <summary>
    /// Information about the content server(s) available
    /// </summary>
    [DataContract(Name = "server", Namespace = "http://identifiers.emc.com/vocab/documentum")]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public class Server
    {
        /// <summary>
        /// The docbroker used to resolve the repository
        /// </summary>
        [DataMember(Name = "docbroker")]
        public string Docbroker { get; set; }

        /// <summary>
        /// The name of the repository that is available for use
        /// </summary>
        [DataMember(Name = "name")]
        public string Name { get; set; }

        /// <summary>
        /// The hostname of the server the repository connection is made to
        /// </summary>
        [DataMember(Name = "host")]
        public string Host { get; set; }

        /// <summary>
        /// Content server version information
        /// </summary>
        [DataMember(Name = "version")]
        public string Version { get; set; }
    }


}
