using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Net.Http.Headers;
using Emc.Documentum.Rest.Http.Utility;
using System.Runtime.Serialization;
using System.Runtime.InteropServices;

namespace Emc.Documentum.Rest.DataModel
{
    /// <summary>
    /// Batch capabilities resource model
    /// </summary>
    [DataContract(Name = "batch-capabilities", Namespace = "http://identifiers.emc.com/vocab/documentum")]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public class BatchCapabilities : ExecLinkable
    {
        /// <summary>
        /// Transaction capability. Available values are: transactional, non-transactional, both.
        /// </summary>
        [DataMember(Name = "transactions")]
        public string Transactions { get; set; }

        /// <summary>
        /// Sequence capability. Available values are: sequential, non-sequential, both.
        /// </summary>
        [DataMember(Name = "sequence")]
        public string Sequence { get; set; }

        /// <summary>
        /// Error handling capability. Available values are: fail-on-error, continue-on-error, both.
        /// </summary>
        [DataMember(Name = "on-error")]
        public string OnError { get; set; }

        /// <summary>
        /// List of batchable resources
        /// </summary>
        [DataMember(Name = "batchable-resources")]
        public List<string> BatchableResources { get; set; }

        /// <summary>
        /// List of non batchable resources
        /// </summary>
        [DataMember(Name = "non-batchable-resources")]
        public List<string> NonBatchableResources { get; set; }
    }
}
