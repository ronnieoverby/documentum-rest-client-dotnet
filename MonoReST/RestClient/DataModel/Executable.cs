using Emc.Documentum.Rest.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emc.Documentum.Rest.DataModel
{
    /// <summary>
    /// Implement this interface to attach a HTTP client instance with the data model
    /// Further REST requests could be invoked from the links on the executable data model
    /// </summary>
    public interface Executable
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        void SetClient(RestController client);
    }
}
