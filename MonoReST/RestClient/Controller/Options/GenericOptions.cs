using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Emc.Documentum.Rest.Net
{
    /// <summary>
    /// key value pairs of URI query parameters
    /// </summary>
    public class GenericOptions
    {
        public Dictionary<string, object> pa = new Dictionary<string, object>();

        public GenericOptions()
        {

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
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
        /// 
        /// </summary>
        /// <returns></returns>
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
