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
using Emc.Documentum.Rest.DataModel.D2;

namespace Emc.Documentum.Rest.DataModel
{
    /// <summary>
    /// Folder resource model
    /// </summary>
    [DataContract(Name = "folder", Namespace = "http://identifiers.emc.com/vocab/documentum")]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public partial class Folder : PersistentObject
    { 
    }
}
