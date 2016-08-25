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
    public partial class HomeDocument : Executable
    {
        private RestController _client;

        /// <summary>
        /// Sets REST client
        /// </summary>
        /// <param name="client"></param>
        public void SetClient(RestController client)
        {
            _client = client;
        }

        /// <summary>
        /// Gets REST client
        /// </summary>
        public RestController Client
        {
            get { return _client; }
            set { this._client = value; }
        }

        /// <summary>
        /// Gets product info resource
        /// </summary>
        /// <returns></returns>
        public ProductInfo GetProductInfo()
        {
            string productInfoUri = this.Resources.About.Href;
            return Client.Get<ProductInfo>(productInfoUri, null);
        }

        /// <summary>
        /// Gets repositories feed requires Options can be null
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="options"></param>
        /// <returns></returns>
        public Feed<T> GetRepositories<T>(FeedGetOptions options) where T : Repository
        {
            string repositoriesUri = this.Resources.Repositories.Href;
            Feed<T> feed = Client.Get<Feed<T>>(repositoriesUri, options == null ? null : options.ToQueryList());
            feed.Client = Client;
            return feed;
        }

        /// <summary>
        /// Gets repository resource by name
        /// </summary>
        /// <param name="repositoryName"></param>
        /// <returns></returns>
        public Repository GetRepository(string repositoryName)
        {
            return GetRepository<Repository>(repositoryName);
        }
   
        /// <summary>
        /// Gets repository resource by name
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="repositoryName"></param>
        /// <returns></returns>
        public T GetRepository<T>(string repositoryName) where T : Repository
        {
            T repository = null;
            Feed<T> repositories = GetRepositories<T>(new FeedGetOptions { Inline = true });
            foreach (Entry<T> repo in repositories.Entries)
            {
                if (repo.Title.Equals(repositoryName))
                {
                    repository = repo.Content;
                    repository.Client = this.Client;
                    break;
                }
            }
            return repository;
        }
    }

}
