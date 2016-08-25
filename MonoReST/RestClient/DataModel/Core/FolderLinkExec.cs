using Emc.Documentum.Rest.Net;
using Emc.Documentum.Rest.Http.Utility;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Emc.Documentum.Rest.DataModel
{
    public partial class FolderLink
    {
        /// <summary>
        /// Move current folder link's target folder to a new folder location
        /// </summary>
        /// <param name="newObj"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public FolderLink MoveTo(Folder newObj, GenericOptions options)
        {
            return Client.Put<Folder, FolderLink>(
                this.Links,
                LinkRelations.SELF.Rel,
                newObj,
                options);
        }

        /// <summary>
        /// Remove the folder link
        /// </summary>
        public void Remove()
        {
            Client.Delete(
                this.Links,
                LinkRelations.SELF.Rel,
                null);
        }
    }
}
