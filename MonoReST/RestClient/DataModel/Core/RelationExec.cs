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
    public partial class Relation
    {
        /// <summary>
        /// Get relation type for this relation
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="options"></param>
        /// <returns></returns>
        public RelationType GetRelationType(SingleGetOptions options)
        {
            return Client.GetSingleton<RelationType>(
                GetFullLinks(),
                LinkRelations.RELATION_TYPE.Rel,
                options);
        }

        /// <summary>
        /// Get relation type for this relation
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="options"></param>
        /// <returns></returns>
        public T GetParent<T>(SingleGetOptions options) where T : PersistentObject
        {
            // parent link relation is not returned by feed for performance 
            if (LinkRelations.FindLinkAsString(GetFullLinks(), LinkRelations.PARENT.Rel) == null)
            {
                this.Links = this.Fetch<Relation>().Links;
            }
            if (LinkRelations.FindLinkAsString(GetFullLinks(), LinkRelations.PARENT.Rel) == null)
            {
                return null;
            }
            else
            {
                return Client.GetSingleton<T>(
                    GetFullLinks(),
                    LinkRelations.PARENT.Rel,
                    options);
            }
        }

        /// <summary>
        /// Get relation type for this relation
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="options"></param>
        /// <returns></returns>
        public T GetChild<T>(SingleGetOptions options) where T : PersistentObject
        {
            // child link relation is not returned by feed for performance 
            if (LinkRelations.FindLinkAsString(GetFullLinks(), LinkRelations.CHILD.Rel) == null)
            {
                this.Links = this.Fetch<Relation>().Links;
            }
            if (LinkRelations.FindLinkAsString(GetFullLinks(), LinkRelations.CHILD.Rel) == null)
            {
                return null;
            }
            else
            {
                return Client.GetSingleton<T>(
                    GetFullLinks(),
                    LinkRelations.CHILD.Rel,
                    options);
            }
        }
    }
}
