using Emc.Documentum.Rest.Http.Utility;
using Emc.Documentum.Rest.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Emc.Documentum.Rest.DataModel.D2
{
    public class D2Document : RestDocument, Executable
    {
        /// <summary>
        /// This is used to set the D2 options when creating the D2 document during creation. This may be 
        /// moved off the D2Document model to somewhere else, but it is here for now until I get more 
        /// familiar with D2 Rest's model and upcoming changes in the next release.
        /// </summary>
        public D2Configuration Configuration { get; set; }


        public C2Views getC2Views()
        {

            return Client.GetSingleton<C2Views>(
                this.Links,
                LinkRelations.C2_VIEW.Rel,
                null);
        }

      
    }

}
