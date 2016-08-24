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
        public static readonly string PARAM_VIEW = "view";
        public static readonly string PARAM_INLINE = "inline";
        public static readonly string PARAM_LINKS = "links";

        public SingleGetOptions() : base()
        {
            // Set any default values here.
            Inline = true;
            Links = true;
        }
        /// <summary>
        /// Specifies the object properties to retrieve. This parameter 
        /// works only when inline is set to true so if set to true
        /// this method will also set inline to true;
        /// </summary>
        public String View
        {
            get {
                return pa[PARAM_VIEW].ToString();
            }
            set 
            {
                // If a view is specified, inline must be true
                if(!String.IsNullOrEmpty(View))
                {
                    Inline = true;
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
        public Boolean Links
        {
            get
            {
                return (Boolean)pa[PARAM_LINKS];
            }
            set {
                if (pa.ContainsKey(PARAM_LINKS))
                {
                    pa[PARAM_LINKS] = value;
                }
                else pa.Add(PARAM_LINKS, value);
                if (value) Inline = true;
            }
        }

        public Boolean Inline
        {
            get
            {
                return (Boolean)pa[PARAM_INLINE];
            }
            set
            {
                if (!pa.ContainsKey(PARAM_INLINE))
                {
                    pa.Add(PARAM_INLINE, value);
                }
                else { pa[PARAM_INLINE] = value; }
            }
        }
    }
}
