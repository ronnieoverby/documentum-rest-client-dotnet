using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Emc.Documentum.Rest.Net;
using Emc.Documentum.Rest.Http.Utility;

namespace Emc.Documentum.Rest.DataModel
{
    /// <summary>
    /// Abstract model with client and links
    /// </summary>
    public abstract class ExecLinkable : Linkable, Executable
    {
        internal RestController _client;

        /// <summary>
        /// Sets REST client
        /// </summary>
        /// <param name="client"></param>
        public virtual void SetClient(RestController client)
        {
            _client = client;
        }

        /// <summary>
        /// Rest controler client 
        /// </summary>
        public RestController Client
        {
            get { return _client; }
            set { this._client = value; }
        }

        /// <summary>
        /// If a persistent object is a raw object, this method can be called to fetch the object again with its links
        /// </summary>
        /// <returns>Returns List</returns>
        protected List<Link> GetFullLinks()
        {
            if (this.Links.Count == 1 && this.Links[0].Title.Equals(LinkRelations.SELF.Rel))
            {
                SingleGetOptions options = new SingleGetOptions { Links = true };
                PersistentObject refreshed = Client.GetSingleton<PersistentObject>(this.Links, LinkRelations.SELF.Rel, options);
                this.Links = refreshed.Links;
            }
            return this.Links;
        }
    }
}
