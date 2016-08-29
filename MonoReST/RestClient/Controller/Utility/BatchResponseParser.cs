using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Emc.Documentum.Rest.DataModel;
using Emc.Documentum.Rest.Net;
namespace Emc.Documentum.Rest.Http.Utility
{
    /// <summary>
    /// A batch operation resonse parser
    /// </summary>
    public class BatchResponseParser
    {
        private static readonly JsonDotnetJsonSerializer JSON_SERIALIZER = new JsonDotnetJsonSerializer();

        /// <summary>
        /// Parse error
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        public Error ParseError(BatchOperationResponse response)
        {
            if (response.Status >= 400)
            {
                using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(response.Entity)))
                {
                    return JSON_SERIALIZER.ReadObject<Error>(stream);
                }
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Parse response body as a resource model
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="response"></param>
        /// <returns></returns>
        public T ParseObject<T>(BatchOperationResponse response)
        {
            if (response.Status >= 200 && response.Status < 300)
            {
                using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(response.Entity)))
                {
                    return JSON_SERIALIZER.ReadObject<T>(stream);
                }
            }
            else
            {
                return default(T);
            }
        }
    }
}
