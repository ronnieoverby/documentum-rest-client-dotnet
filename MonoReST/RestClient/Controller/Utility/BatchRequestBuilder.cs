using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Emc.Documentum.Rest.DataModel;
using Emc.Documentum.Rest.Net;
using Emc.Documentum.Rest.Http.Utility;

namespace Emc.Documentum.Rest.Http.Utility
{
    /// <summary>
    /// A builder to build the batch request
    /// </summary>
    public class BatchRequestBuilder
    {
        private static readonly JsonDotnetJsonSerializer JSON_SERIALIZER = new JsonDotnetJsonSerializer();

        private string description;
        private BatchSpec batchSpec;
        private List<BatchOperation> operations = new List<BatchOperation>();

        /// <summary>
        /// Set batch description
        /// </summary>
        /// <param name="description"></param>
        /// <returns></returns>
        public BatchRequestBuilder Description(string description)
        {
            this.description = description;
            return this;
        }

        /// <summary>
        /// Set batch spec
        /// </summary>
        /// <param name="batchSpec"></param>
        /// <returns></returns>
        public BatchRequestBuilder BatchSpec(BatchSpec batchSpec)
        {
            this.batchSpec = batchSpec;
            return this;
        }

        /// <summary>
        /// Add a batch operation
        /// </summary>
        /// <param name="id"></param>
        /// <param name="sourceModel"></param>
        /// <param name="linkRel"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        public BatchRequestBuilder AddOperation(string id, Linkable sourceModel, string linkRel, string method)
        {
            string uri = GetUriFromLinkRelation(sourceModel, linkRel);
            this.AddOperation(id, uri, method);
            return this;
        }

        /// <summary>
        /// Add a batch operation
        /// </summary>
        /// <param name="id"></param>
        /// <param name="uri"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        public BatchRequestBuilder AddOperation(string id, string uri, string method)
        {
            this.AddOperation<PersistentObject>(id, "", uri, method, null, null);
            return this;
        }

        /// <summary>
        /// Add a batch operation
        /// </summary>
        /// <param name="id"></param>
        /// <param name="sourceModel"></param>
        /// <param name="linkRel"></param>
        /// <param name="method"></param>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public BatchRequestBuilder AddOperation<T>(string id, Linkable sourceModel, string linkRel, string method, T requestModel)
        {
            string uri = GetUriFromLinkRelation(sourceModel, linkRel);
            this.AddOperation(id, "", uri, method, null, requestModel);
            return this;
        }

        /// <summary>
        /// Add a batch operation
        /// </summary>
        /// <param name="id"></param>
        /// <param name="uri"></param>
        /// <param name="method"></param>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public BatchRequestBuilder AddOperation<T>(string id, string uri, string method, T requestModel)
        {
            this.AddOperation(id, "", uri, method, null, requestModel);
            return this;
        }

        /// <summary>
        /// Add a batch operation
        /// </summary>
        /// <param name="id"></param>
        /// <param name="description"></param>
        /// <param name="sourceModel"></param>
        /// <param name="linkRel"></param>
        /// <param name="method"></param>
        /// <param name="headers"></param>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public BatchRequestBuilder AddOperation<T>(string id, string description, Linkable sourceModel, string linkRel, string method, List<BatchOperationHeader> headers, T requestModel)
        {
            string uri = GetUriFromLinkRelation(sourceModel, linkRel);
            this.AddOperation(id, description, uri, method, headers, requestModel);
            return this;
        }

        /// <summary>
        /// Add a batch operation
        /// </summary>
        /// <param name="id"></param>
        /// <param name="description"></param>
        /// <param name="uri"></param>
        /// <param name="method"></param>
        /// <param name="headers"></param>
        /// <param name="requestModel"></param>
        /// <returns></returns>
        public BatchRequestBuilder AddOperation<T>(string id, string description, string uri, string method, List<BatchOperationHeader> headers, T requestModel)
        {
            List<BatchOperationHeader> updatedheaders = headers == null ? new List<BatchOperationHeader>() : new List<BatchOperationHeader>(headers);
            updatedheaders.Add(new BatchOperationHeader { Name = "Content-Type", Value = "application/vnd.emc.documentum+json" });
            updatedheaders.Add(new BatchOperationHeader { Name = "Accept", Value = "application/*+json" });

            string entity = null;
            if (requestModel != null)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    JSON_SERIALIZER.WriteObject<T>(ms, requestModel);
                    entity = System.Text.Encoding.Default.GetString(ms.ToArray());
                }
            }

            BatchOperation operation = new BatchOperation
            {
                Id = id,
                Description = description,
                Request = new BatchOperationRequest { Uri = uri, Method = method, Headers = updatedheaders, Entity = entity }
            };
            operations.Add(operation);
            return this;
        }

        /// <summary>
        /// Build the batch request
        /// </summary>
        /// <returns></returns>
        public Batch Build()
        {
            return new Batch
            {
                Description = description,
                Transactional = batchSpec.Transactional,
                Sequential = batchSpec.Sequential,
                OnError = batchSpec.FailOnError ? "FAIL" : "CONTINUE",
                ReturnRequest = batchSpec.ReturnRequest,
                Operations = operations
            };
        }

        private static string GetUriFromLinkRelation(Linkable sourceModel, string linkRel)
        {
            string uri = LinkRelations.FindLinkAsString(sourceModel.Links, linkRel);
            if (uri != null && uri.Contains("{"))
            {
                uri = uri.Substring(0, uri.IndexOf("{"));
            }
            return uri;
        }

        private void Validate()
        {
            if (operations == null || operations.Count == 0)
            {
                throw new Exception("Invalid batch request: Batch operations cannot be null or empty.");
            }
            foreach(BatchOperation operation in operations)
            {
                if (String.IsNullOrEmpty(operation.Id))
                {
                    throw new Exception("Invalid batch request: Each batch operation must have a non-empty Id.");
                }
                if (operation.Request == null)
                {
                    throw new Exception(String.Format("Invalid batch request: Batch operation {0} must have a non-empty Request.", operation.Id));
                }
                if (String.IsNullOrEmpty(operation.Request.Method))
                {
                    throw new Exception(String.Format("Invalid batch request: Batch operation {0} must have a non-empty Method.", operation.Id));
                }
                if (String.IsNullOrEmpty(operation.Request.Uri))
                {
                    throw new Exception(String.Format("Invalid batch request: Batch operation {0} must have a non-empty Uri.", operation.Id));
                }
            }
        }
    }

    public class BatchSpec
    {
        public bool Transactional { get; set; }
        public bool Sequential { get; set; }
        public bool FailOnError { get; set; }
        public bool ReturnRequest { get; set; }
    }
}
