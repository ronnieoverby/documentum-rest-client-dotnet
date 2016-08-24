using Emc.Documentum.Rest.Net;
using Emc.Documentum.Rest.Http.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Emc.Documentum.Rest.DataModel
{
    /// <summary>
    /// The model class for the dm_cabinet object, inherits from dm_folder but it is the highest level folder
    /// in a respository.
    /// </summary>
    [DataContract(Name = "cabinet", Namespace = "http://identifiers.emc.com/vocab/documentum")]
    public class Cabinet : Folder
    {
/***********************************************************
 * All of the below was required with the DataContractSerilizer, but the Json.Net serializer is more logical
 * and flexible
*/
        ///// <summary>
        ///// Whether this cabinet can be updated
        ///// </summary>
        ///// <returns>Returns Boolean value</returns>
        //public new bool CanUpdate()
        //{
        //    return LinkUtil.FindLinkAsString(this.Links, LinkUtil.EDIT.Rel) != null;
        //}

        ///// <summary>
        ///// Whether this cabinet can be deleted
        ///// </summary>
        ///// <returns>Returns Boolean value</returns>
        //public new bool CanDelete()
        //{
        //    return LinkUtil.FindLinkAsString(this.Links, LinkUtil.DELETE.Rel) != null;
        //}

        ///// <summary>
        ///// Get child folders from this cabinet
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="options"></param>
        ///// <returns>Returns Feed based on options</returns>
        //public Feed<T> GetFolders<T>(FeedGetOptions options)
        //{
        //    return Client.GetFeed<T>(
        //        this.Links,
        //        LinkUtil.FOLDERS.Rel,
        //        options);
        //}

        ///// <summary>
        ///// Get child documents from this cabinet
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="options"></param>
        ///// <returns>Returns the feed</returns>
        //public Feed<T> GetDocuments<T>(FeedGetOptions options)
        //{
        //    return Client.GetFeed<T>(
        //        this.Links,
        //        LinkUtil.DOCUMENTS.Rel,
        //        options);
        //}

        ///// <summary>
        ///// Create a sub folder resource under this folder
        ///// </summary>
        ///// <param name="newObj"></param>
        ///// <param name="options"></param>
        ///// <returns>Returns folder</returns>
        //public Folder CreateSubFolder(Folder newObj, GenericOptions options)
        //{
        //    return Client.Post<Folder>(
        //        this.Links,
        //        LinkUtil.FOLDERS.Rel,
        //        newObj,
        //        options);
        //}

        ///// <summary>
        ///// Create a document resource under this cabinet
        ///// </summary>
        ///// <param name="newObj"></param>
        ///// <param name="options"></param>
        ///// <returns>Returns RestDocument object</returns>
        //public RestDocument CreateSubDocument(RestDocument newObj, GenericOptions options)
        //{
        //    return this.Client.Post<RestDocument>(
        //        this.Links,
        //        LinkUtil.DOCUMENTS.Rel,
        //        newObj,
        //        options);
        //}

        ///// <summary>
        ///// Import a contentful document with content stream under this cabinet
        ///// </summary>
        ///// <param name="newObj"></param>
        ///// <param name="otherPartStream"></param>
        ///// <param name="otherPartMime"></param>
        ///// <param name="options"></param>
        ///// <returns>Returns RestDocument object</returns>
        //public RestDocument ImportDocumentWithContent(RestDocument newObj, Stream otherPartStream, string otherPartMime, GenericOptions options)
        //{
        //    Dictionary<Stream, string> otherParts = new Dictionary<Stream, string>();
        //    otherParts.Add(otherPartStream, otherPartMime);
        //    return Client.Post<RestDocument>(
        //        this.Links,
        //        LinkUtil.DOCUMENTS.Rel,
        //        newObj,
        //        otherParts,
        //        options);
        //}
    }
}
