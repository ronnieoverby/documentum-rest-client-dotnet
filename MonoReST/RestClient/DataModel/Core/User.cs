using Emc.Documentum.Rest.Net;
using Emc.Documentum.Rest.Http.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;


namespace Emc.Documentum.Rest.DataModel
{
    /// <summary>
    /// User resource model
    /// </summary>
    [DataContract(Name = "user", Namespace = "http://identifiers.emc.com/vocab/documentum")]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public partial class User : PersistentObject
    {
    }
}
