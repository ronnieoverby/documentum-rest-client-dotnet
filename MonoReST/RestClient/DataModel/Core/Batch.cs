using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    /// Batch resource model
    /// </summary>
    [DataContract(Name = "batch", Namespace = "http://identifiers.emc.com/vocab/documentum")]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public partial class Batch
    {
        [DataMember(Name = "description")]
        public string Description { get; set; }

        [DataMember(Name = "state")]
        public string State { get; set; }

        [DataMember(Name = "substate")]
        public string Substate { get; set; }

        [DataMember(Name = "owner")]
        public string Owner { get; set; }

        [DataMember(Name = "submitted")]
        public string Submitted { get; set; }

        [DataMember(Name = "started")]
        public string Started { get; set; }

        [DataMember(Name = "finished")]
        public string Finished { get; set; }

        [DataMember(Name = "transactional")]
        public Boolean Transactional { get; set; }

        [DataMember(Name = "sequential")]
        public Boolean Sequential { get; set; }

        [DataMember(Name = "on-error")]
        public string OnError { get; set; }

        [DataMember(Name = "return-request")]
        public Boolean ReturnRequest { get; set; }

        [DataMember(Name = "operations")]
        public List<BatchOperation> Operations 
        {
            get 
            { 
                if (_operations == null)
                {
                    _operations = new List<BatchOperation>();
                }
                return _operations;  
            }
            set { _operations = value; }
        }

        private List<BatchOperation> _operations = new List<BatchOperation>();
    }

    /// <summary>
    /// Batch operation model
    /// </summary>
    [DataContract(Name = "operation", Namespace = "http://identifiers.emc.com/vocab/documentum")]
    public partial class BatchOperation
    {
        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "description")]
        public string Description { get; set; }

        [DataMember(Name = "state")]
        public string State { get; set; }

        [DataMember(Name = "started")]
        public string Started { get; set; }

        [DataMember(Name = "finished")]
        public string Finished { get; set; }

        [DataMember(Name = "request")]
        public BatchOperationRequest Request { get; set; }

        [DataMember(Name = "response")]
        public BatchOperationResponse Response { get; set; }
    }

    /// <summary>
    /// Batch operation request model
    /// </summary>
    [DataContract(Name = "request", Namespace = "http://identifiers.emc.com/vocab/documentum")]
    public partial class BatchOperationRequest : BatchOperationItem
    {
        [DataMember(Name = "method")]
        public string Method { get; set; }

        [DataMember(Name = "uri")]
        public string Uri { get; set; }
    }

    /// <summary>
    /// Batch operation response model
    /// </summary>
    [DataContract(Name = "response", Namespace = "http://identifiers.emc.com/vocab/documentum")]
    public partial class BatchOperationResponse : BatchOperationItem
    {
        [DataMember(Name = "status")]
        public Int32 Status { get; set; }
    }

    /// <summary>
    /// Batch operation basic model
    /// </summary>
    [DataContract(Name = "response", Namespace = "http://identifiers.emc.com/vocab/documentum")]
    public partial class BatchOperationItem
    {
        [DataMember(Name = "headers")]
        public List<BatchOperationHeader> Headers { get; set; }

        [DataMember(Name = "entity")]
        public string Entity { get; set; }
    }

    /// <summary>
    /// Batch operation header model
    /// </summary>
    [DataContract(Name = "header", Namespace = "http://identifiers.emc.com/vocab/documentum")]
    public partial class BatchOperationHeader 
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "value")]
        public string Value { get; set; }
    }
}
