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
    /// <summary>
    /// 
    /// </summary>
    [DataContract(Name = "document", Namespace = "http://identifiers.emc.com/vocab/documentum")]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public class RestDocument : PersistentObject, Executable
    {

        /// <summary>
        /// Get the primary content resource of this document
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public ContentMeta GetPrimaryContent(SingleGetOptions options)
        {
            options.SetQuery("media-url-policy", "all");
            return Client.GetSingleton<ContentMeta>(
                this.Links,
                LinkRelations.PRIMARY_CONTENT.Rel,
                options);
        }

        /// <summary>
        /// Gets a rendition of an object by format qualifier
        /// </summary>
        /// <param name="format"></param>
        /// <returns>Returns Content meta</returns>
        public ContentMeta GetRenditionByFormat(string format)
        {
            SingleGetOptions renditionOptions = new SingleGetOptions();
            renditionOptions.SetQuery("media-url-policy", "local");
            renditionOptions.SetQuery("format", format);
            return GetPrimaryContent(renditionOptions);
        }

        /// <summary>
        /// Get Rendition by modifier
        /// </summary>
        /// <param name="modifier"></param>
        /// <returns></returns>
        public ContentMeta getRenditionByModifier(string modifier)
        {
            return getRenditionByModifierAndFormat(modifier, null);
        }

        /// <summary>
        /// Get content of object (RestDocument)
        /// </summary>
        /// <returns></returns>
        public ContentMeta getContent()
        {
            SingleGetOptions contentOptions = new SingleGetOptions();
            contentOptions.SetQuery("media-url-policy", "local");

            ContentMeta primaryContentMeta = GetPrimaryContent(contentOptions);
            return primaryContentMeta;
        }

        /// <summary>
        /// Get rendition by Modifier and Format for RestDocument
        /// </summary>
        /// <param name="modifier"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public ContentMeta getRenditionByModifierAndFormat(string modifier, string format)
        {
            FeedGetOptions mediaOptions = new FeedGetOptions();
            mediaOptions.SetQuery("media-url-policy", "local");
            if (format != null) mediaOptions.SetQuery("format", "pdf");
            mediaOptions.SetQuery("modifier", "Test");

            Feed<ContentMeta> renditionMetas = GetContents<ContentMeta>(new FeedGetOptions { Inline = true });
            if (renditionMetas == null) return null;

            List<Entry<ContentMeta>> entries = renditionMetas.Entries;
            ContentMeta renditionMeta = null;
            foreach (Entry<ContentMeta> entry in entries)
            {
                ContentMeta rendition = entry.Content;
                if (rendition.getRepeatingString("page_modifier", 0).Equals("Test") || rendition.getRepeatingString("modifier", 0).Equals("Test"))
                {
                    renditionMeta = rendition;
                    renditionMeta.Client = Client;
                    break;
                }
            }
            string dosExtension = ObjectUtil.getDosExtensionFromFormat(format);
            string objectName = getAttributeValue("object_name").ToString();
            FileInfo fi = new FileInfo(getAttributeValue("object_name").ToString());
            renditionMeta.setAttributeValue("object_name", getAttributeValue("object_name"));
            string currentExtention = fi.Extension;

            if (currentExtention != null && !currentExtention.Trim().Equals(""))
            {
                string nameFormat = ObjectUtil.getDocumentumFormatForFile(fi.Extension);
                if (nameFormat.Equals(format))
                {
                    dosExtension = ""; // Name already has the currect extension in it
                }
            }
            renditionMeta.setAttributeValue("dos_extension", dosExtension);
            return renditionMeta;
        }

        ///// <summary>
        ///// Get the primary content resource of this document
        ///// </summary>
        ///// <param name="options"></param>
        ///// <returns></returns>
        //public ContentMeta GetRendition(SingleGetOptions options)
        //{
        //    options.SetQuery("media-url-policy", "all");
        //    return Client.GetSingleton<ContentMeta>(
        //        this.Links,
        //        LinkUtil.CONTENT_MEDIA.Rel,
        //        options);
        //}

        /// <summary>
        /// Get contents feed of this document
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="options"></param>
        /// <returns></returns>
        public Feed<T> GetContents<T>(FeedGetOptions options)
        {
            options.SetQuery("media-url-policy", "all");
            return Client.GetFeed<T>(
                this.Links,
                LinkRelations.CONTENTS.Rel,
                options);
        }

        /// <summary>
        /// Create a new content (rendition) for this document
        /// </summary>
        /// <param name="contentStream"></param>
        /// <param name="mimeType"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public ContentMeta CreateContent(Stream contentStream, string mimeType, GenericOptions options)
        {
            return Client.Post<ContentMeta>(
                this.Links,
                LinkRelations.CONTENTS.Rel,
                contentStream,
                mimeType,
                options);
        }

        /// <summary>
        /// Get version history of this document
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="options"></param>
        /// <returns></returns>
        public Feed<T> GetVersionHistory<T>(FeedGetOptions options)
        {
            return Client.GetFeed<T>(
                this.Links,
                LinkRelations.VERSIONS.Rel,
                options);
        }

        /// <summary>
        /// Get current version of this document
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public RestDocument GetCurrentVersion(SingleGetOptions options)
        {
            return Client.GetSingleton<RestDocument>(
                this.Links,
                LinkRelations.CURRENT_VERSION.Rel,
                options);
        }

        /// <summary>
        /// Get the predecessor version of this document
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public RestDocument GetPredessorVersion(SingleGetOptions options)
        {
            return Client.GetSingleton<RestDocument>(
                this.Links,
                LinkRelations.PREDECESSOR_VERSION.Rel,
                options);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public RestDocument View(GenericOptions options)
        {
            return Client.Post<RestDocument>(
                this.Links,
                LinkRelations.EDIT.Rel,
                null,
                options);
        }

        public string getCheckedOutBy()
        {
            return getLockOwner();
        }

        public string getLockOwner()
        {
            string checkedOutBy = "";
            if (IsCheckedOut())
            {
                if(getAttributeValue("r_object_type").ToString().StartsWith("Process")) {
                    checkedOutBy = getAttributeValue("Process_lock_owner").ToString();
                }
                if(checkedOutBy.Equals("")) {
                    checkedOutBy = getAttributeValue("r_lock_owner").ToString();
                }
            }
            return checkedOutBy;
        }

        /// <summary>
        ///  Checkout the document
        /// </summary>
        /// <returns></returns>
        public RestDocument Checkout()
        {
            if (IsCheckedOut())
            {
                return this;
            }

            return Client.Put<RestDocument>(
                this.Links,
                LinkRelations.CHECKOUT.Rel,
                null,
                null);
        }

        /// <summary>
        /// Cancel checkout the document
        /// </summary>
        /// <returns></returns>
        public RestDocument CancelCheckout()
        {
            if (IsCheckedOut())
            {
                Client.Delete(
                    this.Links,
                    LinkRelations.CANCEL_CHECKOUT.Rel,
                    null);
            }
            return this.fetch<RestDocument>();
        }

        /// <summary>
        /// Checkin a new document as next major version
        /// </summary>
        /// <param name="newDoc"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public RestDocument CheckinMajor(RestDocument newDoc, GenericOptions options)
        {
            return Client.Post<RestDocument>(
                this.Links,
                LinkRelations.CHECKIN_NEXT_MAJOR.Rel,
                newDoc,
                options);
        }

        /// <summary>
        /// Checkin a new document as next minor version
        /// </summary>
        /// <param name="newDoc"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public RestDocument CheckinMinor(RestDocument newDoc, GenericOptions options)
        {
            return Client.Post<RestDocument>(
                this.Links,
                LinkRelations.CHECKIN_NEXT_MINOR.Rel,
                newDoc,
                options);
        }

        /// <summary>
        /// Checkin a new document as branch version
        /// </summary>
        /// <param name="newDoc"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public RestDocument CheckinBranch(RestDocument newDoc, GenericOptions options)
        {
            return Client.Post<RestDocument>(
                this.Links,
                LinkRelations.CHECKIN_BRANCH_VERSION.Rel,
                newDoc,
                options);
        }

        /// <summary>
        /// Check in a new document with content as next major version
        /// </summary>
        /// <param name="newDoc"></param>
        /// <param name="contentStream"></param>
        /// <param name="mimeType"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public RestDocument CheckinMajor(RestDocument newDoc, Stream contentStream, string mimeType, GenericOptions options)
        {
            IDictionary<Stream, string> otherParts = new Dictionary<Stream, string>();
            otherParts.Add(contentStream, mimeType);
            return Client.Post<RestDocument>(
                this.Links,
                LinkRelations.CHECKIN_NEXT_MAJOR.Rel,
                newDoc,
                otherParts,
                options);
        }

        /// <summary>
        /// Checkin a new document with content as next minor version
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="contentStream"></param>
        /// <param name="mimeType"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public RestDocument CheckinMinor(RestDocument doc, Stream contentStream, string mimeType, GenericOptions options)
        {
            RestDocument retDoc = null;

            IDictionary<Stream, string> otherParts = new Dictionary<Stream, string>();
            otherParts.Add(contentStream, mimeType);
            string objectId = doc.getAttributeValue("r_object_id").ToString();
            Dictionary<string, object> allProperties = null;
            if (objectId != null && !objectId.Trim().Equals(""))
            {
                allProperties = doc.Properties;
                doc.Properties = doc.ChangedProperties;
            }
            retDoc = Client.Post<RestDocument>(
                this.Links,
                LinkRelations.CHECKIN_NEXT_MINOR.Rel,
                doc,
                otherParts,
                options);

            if (objectId != null && !objectId.Trim().Equals(""))
            {
                doc.Properties = allProperties;
            }
            return retDoc;
        }

        /// <summary>
        /// Check in a new document with content as branch version
        /// </summary>
        /// <param name="newDoc"></param>
        /// <param name="contentStream"></param>
        /// <param name="mimeType"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public RestDocument CheckinBranch(RestDocument newDoc, Stream contentStream, string mimeType, GenericOptions options)
        {
            IDictionary<Stream, string> otherParts = new Dictionary<Stream, string>();
            otherParts.Add(contentStream, mimeType);
            return Client.Post<RestDocument>(
                this.Links,
                LinkRelations.CHECKIN_BRANCH_VERSION.Rel,
                newDoc,
                otherParts,
                options);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsCheckedOut()
        {
            bool ret = !getAttributeValue("r_lock_owner").Equals("");
            return ret;
        }

        /// <summary>
        /// Whether the document can be checked out
        /// </summary>
        /// <returns></returns>
        public bool CanCheckout()
        {
            return LinkRelations.FindLinkAsString(this.Links, LinkRelations.CHECKOUT.Rel) != null;
        }

        /// <summary>
        /// Whether the document can be checked in
        /// </summary>
        /// <returns></returns>
        public bool CanCheckin()
        {
            return LinkRelations.FindLinkAsString(this.Links, LinkRelations.CHECKIN_NEXT_MAJOR.Rel) != null
                || LinkRelations.FindLinkAsString(this.Links, LinkRelations.CHECKIN_NEXT_MINOR.Rel) != null
                || LinkRelations.FindLinkAsString(this.Links, LinkRelations.CHECKIN_BRANCH_VERSION.Rel) != null;
        }

        /// <summary>
        /// Cancel check out the document
        /// </summary>
        /// <returns></returns>
        public bool CanCancelCheckout()
        {
            return LinkRelations.FindLinkAsString(this.Links, LinkRelations.CANCEL_CHECKOUT.Rel) != null;
        }

        /// <summary>
        /// Whether the document has a predecessor version
        /// </summary>
        /// <returns></returns>
        public bool HasPredecessorVersion()
        {
            return LinkRelations.FindLinkAsString(this.Links, LinkRelations.PREDECESSOR_VERSION.Rel) != null;
        }


    }
}
