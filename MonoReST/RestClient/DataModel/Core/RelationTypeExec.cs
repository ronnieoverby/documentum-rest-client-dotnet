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
    public partial class RelationType
    {
        /// <summary>
        /// Get all relations for this relation type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="options"></param>
        /// <returns></returns>
        public Feed<T> GetRelations<T>(FeedGetOptions options) where T : Relation
        {
            return Client.GetFeed<T>(
                GetFullLinks(),
                LinkRelations.RELATIONS.Rel,
                options);
        }
    }
}
