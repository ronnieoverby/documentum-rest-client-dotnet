using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Emc.Documentum.Rest.DataModel
{
    public class C2View : Linkable
    {
        [DataMember(Name = "url", IsRequired = false)]
        public String Url { get; set; }

        [DataMember(Name = "view-type", IsRequired = false)]
        public String ViewType { get; set; }

    }
}
