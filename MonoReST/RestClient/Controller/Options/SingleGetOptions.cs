using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emc.Documentum.Rest.Net;

namespace Emc.Documentum.Rest.Net
{
    /// <summary>
    /// Single resource related query parameters
    /// </summary>
    public class SingleGetOptions : GenericOptions
    {
        /// <summary>
        /// Query parameter 'view'
        /// </summary>
        public static readonly string PARAM_VIEW = "view";
        /// <summary>
        /// Query parameter 'links'
        /// </summary>
        public static readonly string PARAM_LINKS = "links";

        /// <summary>
        /// Default constructor
        /// </summary>
        public SingleGetOptions() : base()
        {
            // Set any default values here.
            Links = true;
        }
        /// <summary>
        /// Specifies the object properties to retrieve. This parameter 
        /// works only when inline is set to true so if set to true
        /// this method will also set inline to true;
        /// </summary>
        public virtual String View
        {
            get {
                return pa.ContainsKey(PARAM_VIEW) ? pa[PARAM_VIEW].ToString() : ":default";
            }
            set 
            {
                // If a view is specified, inline must be true
                if(!String.IsNullOrEmpty(View))
                {
                    if (pa.ContainsKey(PARAM_VIEW))
                    {
                        pa[PARAM_VIEW] = value;
                    }
                    else pa.Add(PARAM_VIEW, value);
                } else
                {
                    pa.Remove(PARAM_VIEW);
                }
            }
        }

        /// <summary>
        /// Determines whether or not to return link relations in the object 
        /// representation This parameter works only when inline is set to true.
        /// true - return link relations
        /// false - do not return link relations
        /// Default: true
        /// </summary>
        public virtual Boolean Links
        {
            get
            {
                return pa.ContainsKey(PARAM_LINKS) ? (Boolean)pa[PARAM_LINKS] : true;
            }
            set {
                if (pa.ContainsKey(PARAM_LINKS))
                {
                    pa[PARAM_LINKS] = value;
                }
                else pa.Add(PARAM_LINKS, value);
            }
        }
    }
}
