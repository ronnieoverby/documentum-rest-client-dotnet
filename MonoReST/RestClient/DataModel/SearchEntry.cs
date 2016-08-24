using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Emc.Documentum.Rest.Net;
using System.Runtime.InteropServices;

namespace Emc.Documentum.Rest.DataModel
{
    [DataContract(Name = "entry", Namespace = "http://www.w3.org/2005/Atom")]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public class SearchEntry<T> : Entry<T>
    {
        /// <summary>
        /// The search score
        /// </summary>
        [DataMember(Name = "score")]
        public float Score { get; set; }

        /// <summary>
        /// The terms that match, can be used to highlight in the summary
        /// </summary>
        [DataMember(Name = "terms")]
        public string[] Terms { get; set; }

        public override string ToString()
        {
            JsonDotnetJsonSerializer serializer = new JsonDotnetJsonSerializer();
            return serializer.Serialize(this);
        }
    
    }
}
