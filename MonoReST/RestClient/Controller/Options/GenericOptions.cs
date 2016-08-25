using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Emc.Documentum.Rest.Net
{
    /// <summary>
    /// Key value pairs of URI query parameters
    /// </summary>
    public class GenericOptions
    {
        protected Dictionary<string, object> pa = new Dictionary<string, object>();

        /// <summary>
        /// Constructor
        /// </summary>
        public GenericOptions()
        {

        }

        /// <summary>
        /// Sets query parameter like links=true
        /// </summary>
        /// <param name="name">Parameter name</param>
        /// <param name="value">Parameter value</param>
        public void SetQuery(String name, object value)
        {
            if(!pa.ContainsKey(name))
            {
                pa.Add(name, value);
            } else
            {
                // TODO: Add warning log message that query param already existed.
            }
            
        }

        /// <summary>
        /// Check whether the specified parameter is included
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool ContainsParam(string name)
        {
            return pa.ContainsKey(name);
        }

        /// <summary>
        /// Returns the query parameters as a list of pairs
        /// </summary>
        /// <returns>The list of query parameters</returns>
        public List<KeyValuePair<string,object>> ToQueryList()
        {
            List<KeyValuePair<string, object>> kvp = new List<KeyValuePair<string, object>>();   
            foreach(String key in pa.Keys)
            {
                kvp.Add(new KeyValuePair<string, object>(key, pa[key]));
            }
            return kvp;
        }
    }
}
