using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emc.Documentum.Rest.DataModel
{
    /// <summary>
    /// Version history type
    /// </summary>
    public enum HistoryType
    {
        /// <summary>
        /// All versions
        /// </summary>
        FULLVERSIONTREE,
        /// <summary>
        /// Current version
        /// </summary>
        THISDOCUMENTONLY
    }
}
