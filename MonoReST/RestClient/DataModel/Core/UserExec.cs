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
        public Cabinet GetHomeCabinet(SingleGetOptions options)
        {
            return Client.GetSingleton<Cabinet>(GetFullLinks(), LinkRelations.DEFAULT_FOLDER.Rel, options);
        }

        /// <summary>
        /// Update a user
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="newUser"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public T Update<T>(T newUser, GenericOptions options) where T : User
        {
            return Client.Post<T>(
                GetFullLinks(),
                LinkRelations.EDIT.Rel,
                newUser,
                options);
        }

        /// <summary>
        ///  Delete a user       
        /// </summary>
        public void Delete()
        {
            Client.Delete(LinkRelations.FindLinkAsString(GetFullLinks(), LinkRelations.DELETE.Rel));
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
                GetFullLinks(),
                LinkRelations.PARENT.Rel,
                options);
        }
    }
}
