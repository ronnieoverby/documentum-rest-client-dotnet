using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Emc.Documentum.Rest.Net;
using Emc.Documentum.Rest.Http.Utility;

namespace Emc.Documentum.Rest.DataModel
{
    public partial class DmType
    {
        /// <summary>
        /// Get sub types of this type
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public Feed<DmType> GetSubTypes(FeedGetOptions options) 
        {
            return Client.GetFeed<DmType>(
                GetFullLinks(),
                LinkRelations.TYPES.Rel,
                options);
        }

        /// <summary>
        /// Get parent type for this type
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public DmType GetParentType(SingleGetOptions options)
        {
            if (LinkRelations.FindLinkAsString(GetFullLinks(), LinkRelations.PARENT.Rel) == null)
            {
                return null;
            }
            else
            {
                return Client.GetSingleton<DmType>(
                    GetFullLinks(),
                    LinkRelations.PARENT.Rel,
                    options);
            }
        }

    }
}
