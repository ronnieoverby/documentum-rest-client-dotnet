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
    /// Batch resource model
    /// </summary>
    [DataContract(Name = "batch", Namespace = "http://identifiers.emc.com/vocab/documentum")]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public class Batch
    {
        public Batch()
        {

        }
        public Batch(string description, bool transactional, bool sequential, string onError, bool returnRequest) 
        {
            Description = description;
            Transactional = transactional;
            Sequential = sequential;
            OnError = onError;
            ReturnRequest = returnRequest;
        }
        public Batch(string description, bool transactional, bool sequential, string onError, bool returnRequest, List<Operation> operations)
        {
            Description = description;
            Transactional = transactional;
            Sequential = sequential;
            OnError = onError;
            ReturnRequest = returnRequest;
            Operations = operations;
        }

        [DataMember(Name = "description")]
        public string Description {get; set;}

        [DataMember(Name = "transactional")]
        public Boolean Transactional { get; set; }

        [DataMember(Name = "sequential")]
        public Boolean Sequential { get; set; }

        [DataMember(Name = "on-error")]
        public string OnError { get; set; }

        [DataMember(Name = "return-request")]
        public Boolean ReturnRequest { get; set; }

        [DataMember(Name = "operations")]
        public List<Operation> Operations { get; set; }
        public Operation GetNewOperation()
        {
            return new Operation();
        }


    }

    /// <summary>
    /// Batch operation model
    /// </summary>
    [DataContract(Name = "operation", Namespace = "http://identifiers.emc.com/vocab/documentum")]
    public partial class Operation
    {
        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "description")]
        public string Description { get; set; }

        [DataMember(Name = "uri")]
        public string URI { get; set; }

        [DataMember(Name = "minor")]
        public string Minor { get; set; }

        private List<Header> _headers = new List<Header>();
        [DataMember(Name = "headers")]
        public List<Header> Headers
        {
            get
            {
                return _headers;
            }
        }

        public void addHeader(String name, String value) {
            Header header = new Header();
            header.Name = name;
            header.Value = value;
            Headers.Add(header);
        }
    }

    /// <summary>
    /// Batch request and response header model
    /// </summary>
    public partial class Header 
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "value")]
        public string Value { get; set; }
    }
}
