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
    public partial class Document
    {
        private static readonly string MEDIA_URL_POLICY = "media-url-policy";

        /// <summary>
        /// Get contents feed of this document
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="options"></param>
        /// <returns></returns>
        public Feed<T> GetContents<T>(FeedGetOptions options)
        {
            SetMediaUrlPolicy(options);
            return Client.GetFeed<T>(
                this.Links,
                LinkRelations.CONTENTS.Rel,
                options);
        }

        /// <summary>
        /// Get the primary content resource of this document
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public ContentMeta GetPrimaryContent(SingleGetOptions options)
        {
            SetMediaUrlPolicy(options);
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
            return GetRenditionByModifierAndFormat(null, format);
        }

        /// <summary>
        /// Get a rendition by modifier
        /// </summary>
        /// <param name="modifier"></param>
        /// <returns></returns>
        public ContentMeta GetRenditionByModifier(string modifier)
        {
            return GetRenditionByModifierAndFormat(modifier, null);
        }

        /// <summary>
        /// Get rendition by Modifier and Format for RestDocument
        /// </summary>
        /// <param name="modifier"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        public ContentMeta GetRenditionByModifierAndFormat(string modifier, string format)
        {
            SingleGetOptions mediaOptions = new SingleGetOptions { View = ":all" };
            SetMediaUrlPolicy(mediaOptions);
            SetFormat(mediaOptions, format);
            SetModifier(mediaOptions, modifier);
            return GetPrimaryContent(mediaOptions);
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
        /// Get checked out user
        /// </summary>
        /// <returns></returns>
        public string GetCheckedOutBy()
        {
            string checkedOutBy = "";
            if (IsCheckedOut())
            {
                if (GetPropertyValue("r_object_type").ToString().StartsWith("Process"))
                {
                    checkedOutBy = GetPropertyValue("Process_lock_owner").ToString();
                }
                if (checkedOutBy.Equals(""))
                {
                    checkedOutBy = GetPropertyValue("r_lock_owner").ToString();
                }
            }
            return checkedOutBy;
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
        public Document GetCurrentVersion(SingleGetOptions options)
        {
            return Client.GetSingleton<Document>(
                this.Links,
                LinkRelations.CURRENT_VERSION.Rel,
                options);
        }

        /// <summary>
        /// Get the predecessor version of this document
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public Document GetPredessorVersion(SingleGetOptions options)
        {
            return Client.GetSingleton<Document>(
                this.Links,
                LinkRelations.PREDECESSOR_VERSION.Rel,
                options);
        }

        /// <summary>
        ///  Checkout the document
        /// </summary>
        /// <returns></returns>
        public Document Checkout()
        {
            if (IsCheckedOut())
            {
                return this;
            }

            return Client.Put<Document>(
                this.Links,
                LinkRelations.CHECKOUT.Rel,
                null,
                null);
        }

        /// <summary>
        /// Cancel checkout the document
        /// </summary>
        /// <returns></returns>
        public Document CancelCheckout()
        {
            if (IsCheckedOut())
            {
                Client.Delete(
                    this.Links,
                    LinkRelations.CANCEL_CHECKOUT.Rel,
                    null);
            }
            return this.Fetch<Document>();
        }

        /// <summary>
        /// Checkin a new document as next major version
        /// </summary>
        /// <param name="newDoc"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public Document CheckinMajor(Document newDoc, GenericOptions options)
        {
            return Client.Post<Document>(
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
        public Document CheckinMinor(Document newDoc, GenericOptions options)
        {
            return Client.Post<Document>(
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
        public Document CheckinBranch(Document newDoc, GenericOptions options)
        {
            return Client.Post<Document>(
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
        public Document CheckinMajor(Document newDoc, Stream contentStream, string mimeType, GenericOptions options)
        {
            IDictionary<Stream, string> otherParts = new Dictionary<Stream, string>();
            otherParts.Add(contentStream, mimeType);
            return Client.Post<Document>(
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
        public Document CheckinMinor(Document doc, Stream contentStream, string mimeType, GenericOptions options)
        {
            Document retDoc = null;

            IDictionary<Stream, string> otherParts = new Dictionary<Stream, string>();
            otherParts.Add(contentStream, mimeType);
            string objectId = doc.GetPropertyValue("r_object_id").ToString();
            Dictionary<string, object> allProperties = null;
            if (objectId != null && !objectId.Trim().Equals(""))
            {
                allProperties = doc.Properties;
                doc.Properties = doc.ChangedProperties;
            }
            retDoc = Client.Post<Document>(
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
        public Document CheckinBranch(Document newDoc, Stream contentStream, string mimeType, GenericOptions options)
        {
            IDictionary<Stream, string> otherParts = new Dictionary<Stream, string>();
            otherParts.Add(contentStream, mimeType);
            return Client.Post<Document>(
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
            bool ret = !GetPropertyValue("r_lock_owner").Equals("");
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

        private void SetMediaUrlPolicy(GenericOptions options)
        {
            if (!options.ContainsParam(MEDIA_URL_POLICY))
            {
                options.SetQuery(MEDIA_URL_POLICY, "all");
            }
        }

        private void SetFormat(GenericOptions options, string format)
        {
            options.SetQuery("format", format);
        }
        
        private void SetModifier(GenericOptions options, string modifier)
        {
            if (!String.IsNullOrEmpty(modifier))
            {
                options.SetQuery("modifier", modifier);
            }
        }
    }
}
