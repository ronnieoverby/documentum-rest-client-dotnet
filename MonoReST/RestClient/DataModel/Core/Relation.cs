using Emc.Documentum.Rest.Net;
using Emc.Documentum.Rest.Http.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace Emc.Documentum.Rest.DataModel
{
    /// <summary>
    /// Relation resource model
    /// </summary>
    [DataContract(Name = "relation", Namespace = "http://identifiers.emc.com/vocab/documentum")]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public partial class Relation : PersistentObject
    {
    }
}
