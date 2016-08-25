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
    public partial class User
    {
        /// <summary>
        /// Get the home cabinet (default folder) resource of the user
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public Folder GetHomeCabinet(SingleGetOptions options)
        {
            return Client.GetSingleton<Folder>(this.Links, LinkRelations.DEFAULT_FOLDER.Rel, options);
        }

        /// <summary>
        /// Get the groups feed of which this user is a member
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
    }
}
