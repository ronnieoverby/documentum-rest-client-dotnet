using Emc.Documentum.Rest.Net;
using Emc.Documentum.Rest.Http.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Collections.Specialized;
using System.Runtime.InteropServices;

namespace Emc.Documentum.Rest.DataModel
{
    public partial class ContentMeta
    {
        /// <summary>
        /// Get the associated document resource of this content
        /// </summary>
        /// <param name="options"></param>
        /// <returns>Returns a RestDocument object </returns>
        public Document GetParentDocument(SingleGetOptions options)
        {
            return Client.GetSingleton<Document>(this.Links, LinkRelations.PARENT.Rel, options);
        }

        /// <summary>
        /// Download the content media of this content to a local file
        /// </summary>
        /// <returns>Returns System FileInfo</returns>
        public FileInfo DownloadContentMediaFile()
        {
            
            string contentMediaUri = LinkRelations.FindLinkAsString(this.Links, LinkRelations.CONTENT_MEDIA.Rel);
            string fileName = (string)GetPropertyValue("object_name"); 
            string dosExtension = (string)GetPropertyValue("dos_extension");
            if (String.IsNullOrEmpty(fileName))
            {
                fileName = "namelessobj-" + System.Guid.NewGuid().ToString();
            }

            // This is meant to avoid duplication extensions while also ensuring that
            // known formats get a proper extension if they do not have one.
            String fileExtension = fileName.Contains('.') ? fileName.Substring(fileName.LastIndexOf('.') + 1) : "";
            if (dosExtension != null && fileName.ToLower().EndsWith(dosExtension.ToLower()))
            {
                // The extension is already on the object name, do not append anthing
                fileExtension = "";
            } else {
                if(dosExtension != null && !dosExtension.Trim().Equals("")) {
                    fileExtension = dosExtension;
                } else {
                    fileExtension = ObjectUtil.getDosExtensionFromFormat(GetPropertyValue("full_format").ToString());
                }
            }

            fileName = ObjectUtil.getSafeFileName(fileName);
            // Ensure file extension is not already there

            string fullPath = Path.Combine(Path.GetTempPath(), fileName + ( (fileExtension == null || fileExtension.Equals("")) ? "" : "." + fileExtension) );

            using (Stream media = DownloadContentMediaStream())
            {
                if (media == null)
                {
                    throw new Exception("Stream came back null. This is normally caused by an unreachable ACS Server (DNS problem or Method Server DOWN). ACS URL is: " + contentMediaUri);
                }
                FileStream fs = File.Create(fullPath);
                media.CopyTo(fs);
                fs.Dispose();
            }

            return new FileInfo(fullPath);
        }

        /// <summary>
        /// Download the content media as stream
        /// </summary>
        /// <returns></returns>
        public Stream DownloadContentMediaStream()
        {
            string contentMediaUri = LinkRelations.FindLinkAsString(this.Links, LinkRelations.CONTENT_MEDIA.Rel);
            return Client.GetRaw(contentMediaUri);
        }

        /// <summary>
        /// Get the content media URL
        /// </summary>
        /// <returns></returns>
        public String GetMediaUri()
        {
            return LinkRelations.FindLinkAsString(this.Links, LinkRelations.CONTENT_MEDIA.Rel);
        }
    }
}
