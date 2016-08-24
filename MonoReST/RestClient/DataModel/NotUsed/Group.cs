using Emc.Documentum.Rest.Net;
using Emc.Documentum.Rest.Http.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Emc.Documentum.Rest.DataModel
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    [DataContract(Name = "group", Namespace = "http://identifiers.emc.com/vocab/documentum")]
    public partial class Group : PersistentObject
    {
        /// <summary>
        /// Get groups feed of which this group is a member
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="options"></param>
        /// <returns></returns>
        public Feed<T> GetParentGroups<T>(FeedGetOptions options)
        {
            return Client.GetFeed<T>(
                this.Links,
                LinkRelations.PARENT.Rel,
                options);
        }

        /// <summary>
        /// Get groups feed which are members of this group
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="options"></param>
        /// <returns></returns>
        public Feed<T> GetSubGroups<T>(FeedGetOptions options)
        {
            return Client.GetFeed<T>(
                this.Links,
                LinkRelations.GROUPS.Rel,
                options);
        }

        /// <summary>
        /// Get users feed which are members of this group
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="options"></param>
        /// <returns></returns>
        public Feed<T> GetGroupUsers<T>(FeedGetOptions options)
        {
            return Client.GetFeed<T>(
                this.Links,
                LinkRelations.USERS.Rel,
                options);
        }
    }
}
