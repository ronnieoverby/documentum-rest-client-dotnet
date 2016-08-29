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
    public partial class Batch
    {
        public Batch()
        {

        }
        public Batch(string description, bool transactional, bool sequential, bool failOnError, bool returnRequest)
            : this(description, transactional, sequential, failOnError, returnRequest, null) { }

        public Batch(string description, bool transactional, bool sequential, bool failOnError, bool returnRequest, List<BatchOperation> operations)
        {
            Description = description;
            Transactional = transactional;
            Sequential = sequential;
            OnError = failOnError ? "FAIL" : "CONTINUE";
            ReturnRequest = returnRequest;
            Operations = operations;
        }

        public static BatchRequestBuilder CreateFromBuilder()
        {
            return new BatchRequestBuilder();
        }
    }

    public partial class BatchOperationResponse
    {
        public bool HasError()
        {
            return this.Status >= 400;
        }

        public T GetObject<T>()
        {
            return new BatchResponseParser().ParseObject<T>(this);
        }

        public Error GetError()
        {
            return new BatchResponseParser().ParseError(this);
        }
    }
}
