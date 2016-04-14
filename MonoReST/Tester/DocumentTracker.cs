using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emc.Documentum.Rest.CustomModel
{
    /// <summary>
    /// Keeps track of which documents go to which parent/child folders
    /// </summary>
    public class DocumentTracker
    {
        /// <summary>
        /// The ID of the document. This ID will change/update when the document is assigned to a parent/child folder as assigning is a copy operation
        /// </summary>
        public string DocumentId { get; set; }
        /// <summary>
        /// The parentId the document will be assigned to
        /// </summary>
        public string ParentId { get; set; }
        /// <summary>
        /// The childId the document will be assigned to
        /// </summary>
        public string ChildId { get; set; }

        /// <summary>
        /// Constructor used to assign a document to a parent/child folder
        /// </summary>
        /// <param name="documentId"></param>
        /// <param name="parentId"></param>
        /// <param name="childId"></param>

        public DocumentTracker(string documentId, string parentId, string childId) {
            DocumentId = documentId;
            ParentId = parentId;
            ChildId = childId;
        }

        /// <summary>
        /// Constructor for assigning a document to a parent
        /// </summary>
        /// <param name="documentId"></param>
        /// <param name="parentId"></param>
        public DocumentTracker(string documentId, string parentId)
        {
            DocumentId = documentId;
            ParentId = parentId;
            ChildId = null;
        }

        /// <summary>
        /// Returns the relative path "parent/child folder" vs. "/SystemA/Process/parent/child folder" 
        /// for the assignment
        /// </summary>
        /// <returns> String that is the path</returns>
        public string getPath()
        {
            StringBuilder pathBuilder = new StringBuilder();
            if (ParentId != null && !ParentId.Trim().Equals(""))
            {
                pathBuilder.Append(ParentId);
            }
            return pathBuilder.ToString();
        }
    }
}
