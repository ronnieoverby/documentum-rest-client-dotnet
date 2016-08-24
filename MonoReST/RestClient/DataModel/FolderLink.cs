using Emc.Documentum.Rest.Net;
using Emc.Documentum.Rest.Http.Utility;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Text;

namespace Emc.Documentum.Rest.DataModel
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    [DataContract(Name = "folder-link", Namespace = "http://identifiers.emc.com/vocab/documentum")]  
    public class FolderLink : Linkable, Executable
    {
        [DataMember(Name = "href")]
        public string Href { get; set; }

        [DataMember(Name = "parent-id")]
        public string ParentId { get; set; }

        [DataMember(Name = "child-id")]
        public string ChildId { get; set; }

        public override string ToString()
        {
            JsonDotnetJsonSerializer serializer = new JsonDotnetJsonSerializer();
            return serializer.Serialize(this);
        }

        private RestController _client;
        public void SetClient(RestController client)
        {
            _client = client;
        }

        public RestController Client
        {
            get { return _client; }
            set { this._client = value; }
        }

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
