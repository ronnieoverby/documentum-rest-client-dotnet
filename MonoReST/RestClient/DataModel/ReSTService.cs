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
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    [DataContract(Name = "services", Namespace = "http://identifiers.emc.com/vocab/documentum")]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public class RestService : Executable
    {
        [DataMember(Name = "resources")]
        public Resources Resources { 
            get; 
            set; 
        }

        public override string ToString()
        {
            JsonDotnetJsonSerializer serializer = new JsonDotnetJsonSerializer();
            return serializer.Serialize(this);
        }

        private RestController _client;
        public void SetClient(RestController client)
        {
            _client = client;
        }

        public RestController Client
        {
            get { return _client; }
            set { this._client = value; }
        }

        /// <summary>
        /// Get product info resource
        /// </summary>
        /// <returns></returns>
        public ProductInfo GetProductInfo()
        {
            string productInfoUri = this.Resources.About.Href;
            return Client.Get<ProductInfo>(productInfoUri, null);
        }

        /// <summary>
        /// Get repositories feed requires Options can be null
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="options"></param>
        /// <returns></returns>
        public Feed<T> GetRepositories<T>(FeedGetOptions options)
        {
            string repositoriesUri = this.Resources.Repositories.Href;
            Feed<T> feed = Client.Get<Feed<T>>(repositoriesUri, options == null ? null : options.ToQueryList());
            feed.Client = Client;
            return feed;
        }

        public Repository GetRepository(string repositoryName)
        {
            Repository repository = Client.Get<Repository>(this.Resources.Repositories.Href + "/" + repositoryName, null);
            if (repository != null)
            {
                Client.RepositoryBaseUri = repository.getRepositoryUri();
                repository.Client = Client;
            }
            return repository;
        }
    }

    [DataContract(Name = "resources", Namespace = "http://identifiers.emc.com/vocab/documentum")]
    public class Resources
    {
        [DataMember(Name = "http://identifiers.emc.com/linkrel/repositories")]
        public Resource Repositories { get; set; }

        [DataMember(Name = "about")]
        public Resource About { get; set; }
    }

    [DataContract(Name = "resources", Namespace = "http://identifiers.emc.com/vocab/documentum")]
    public class Resource
    {
        [DataMember(Name = "href")]
        public string Href { get; set; }

        [DataMember(Name = "hints")]
        public Hints Hints { get; set; }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat(
                "Resource{{href: {0}, hints_allow: {1}, hints_representations: {2}}}",
                this.Href,
                string.Join(", ", this.Hints.Allow),
                string.Join(", ", this.Hints.Representations));
            return builder.ToString();
        }

        
    }

    [DataContract(Name = "hints", Namespace = "http://identifiers.emc.com/vocab/documentum")]
    public class Hints
    {
        [DataMember(Name = "allow")]
        public List<string> Allow { get; set; }

        [DataMember(Name = "representations")]
        public List<string> Representations { get; set; }
    }


}
