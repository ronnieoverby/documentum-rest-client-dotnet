using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

using Emc.Documentum.Rest.Net;

namespace Emc.Documentum.Rest.DataModel
{

    /// <summary>
    /// Abstract client with links
    /// </summary>
    [DataContract]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public abstract class Linkable
    {
        private List<Link> _links = new List<Link>();
        [DataMember(Name = "links")]
        private List<Link> LinksForBinding
        {
            get
            {
                return Links == null || Links.Count == 0 ? null : Links;
            }
            set
            {
                Links = value;
            }
        }

        /// <summary>
        /// Links collection on a resource model
        /// </summary>
        public List<Link> Links
        {
            get
            {
                if (_links == null)
                {
                    _links = new List<Link>();
                }
                return _links;
            }
            set
            {
                _links = value;
            }
        }

        /// <summary>
        /// To JSON string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            JsonDotnetJsonSerializer serializer = new JsonDotnetJsonSerializer();
            return serializer.Serialize(this);
        }
    }
}
