using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emc.Documentum.Rest.Net
{
    /// <summary>
    /// Abstract JSON message serializer 
    /// </summary>
    public abstract class AbstractJsonSerializer
    {
        /// <summary>
        /// Read resource data object from JSON message stream
        /// </summary>
        /// <typeparam name="T">Resource data model type</typeparam>
        /// <param name="input">JSON input stream</param>
        /// <returns>Resource data model object</returns>
        public abstract T ReadObject<T>(Stream input);

        /// <summary>
        /// Write resource data object to JSON message stream
        /// </summary>
        /// <typeparam name="T">Resource data model type</typeparam>
        /// <param name="output">JSON output stream</param>
        /// <param name="obj">Resource data model object</param>
        public abstract void WriteObject<T>(Stream output, T obj);
    }
}
