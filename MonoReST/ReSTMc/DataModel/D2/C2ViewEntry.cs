using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Emc.Documentum.Rest.DataModel.D2
{

    public class C2ViewEntry : Linkable
    {
        [DataMember(Name = "id", IsRequired = false)]
        public String Id { get; set; }

        [DataMember(Name = "content", IsRequired = false)]
        public C2View C2View { get; set; }



    }
}
