using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emc.Documentum.Rest.Net
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class AbstractJsonSerializer
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="input"></param>
        /// <returns></returns>
        public abstract T ReadObject<T>(Stream input);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="output"></param>
        /// <param name="obj"></param>
        public abstract void WriteObject<T>(Stream output, T obj);
    }
}
