using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Emc.Documentum.Rest.Net;

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
        public void SetClient(RestController client)
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
    }
}
