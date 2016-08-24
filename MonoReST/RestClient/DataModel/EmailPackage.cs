using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emc.Documentum.Rest.Net;
namespace Emc.Documentum.Rest.DataModel
{
    public enum DuplicateType
    {
        /// <summary>
        /// EMAIL was found in the same folder it was being imported into
        /// </summary>
        FOLDER,
        /// <summary>
        /// EMAIL was found somewhere else in the system.
        /// </summary>
        SYSTEM,
    }
   /// <summary>
   /// Email Package entry
   /// </summary>
    public class EmailPackage
    {
        private bool duplicate = false;
        /// <summary>
        /// Does the email exist already in the system
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="isDuplicate"></param>
        public bool IsDuplicate
        {
            get { return duplicate; }
            set { duplicate = value; }
        }


        /// <summary>
        /// If the email is a duplicate, was in found in the same FOLDER
        /// or somewhere else in the SYSTEM
        /// </summary>
        public DuplicateType DuplicateType { get; set; }

        public EmailPackage(RestDocument doc, bool isDuplicate)
        {
            Email = doc;
            Attachments = new List<RestDocument>();
            duplicate = isDuplicate;
        }

        public override string ToString()
        {
            JsonDotnetJsonSerializer serializer = new JsonDotnetJsonSerializer();
            return serializer.Serialize(this);
        }

        public RestDocument Email { get; set; }
        public List<RestDocument> Attachments { get; set; }
    }
}
