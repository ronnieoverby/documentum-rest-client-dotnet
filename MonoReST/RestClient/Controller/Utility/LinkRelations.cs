using Emc.Documentum.Rest.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emc.Documentum.Rest.Http.Utility
{

    /// <summary>
    /// Class used for resolving document action links
    /// </summary>
    public class LinkRelations
    {

        /********************BEGIN CUSTOM REST LINK RELATIONS ******************************/
        /// <summary>
        /// The location to append to the repositoryBaseUri for the mail import service
        /// </summary>
        public static string EMAILIMPORT = "/mailimport";
        /// <summary>
        /// The location to append to the repositoryBaseUri for the retention event date service
        /// </summary>
        public static string RETENTION = "/retention";

        /********************BEGIN CORE REST LINK RELATIONS ******************************/
        /// <summary>
        /// The relection/self link of the current object
        /// </summary>
        public static LinkRelations SELF = new LinkRelations("self");
        /// <summary>
        /// The link used to post changes to the object
        /// </summary>
        public static LinkRelations EDIT = new LinkRelations("edit");
        /// <summary>
        /// The object used to delete an object
        /// </summary>
        public static LinkRelations DELETE = new LinkRelations("delete", true);
        /// <summary>
        /// The link used to return the document/object content
        /// </summary>
        public static LinkRelations CONTENTS = new LinkRelations("contents");
        /// <summary>
        /// Link used to return a list of versions
        /// </summary>
        public static LinkRelations VERSIONS = new LinkRelations("version-history");
        /// <summary>
        /// The parent of this object
        /// </summary>
        public static LinkRelations PARENT = new LinkRelations("parent");
        public static LinkRelations CHILD = new LinkRelations("child");
        public static LinkRelations ALTERNATE = new LinkRelations("alternate");
        public static LinkRelations PAGING_NEXT = new LinkRelations("next");
        public static LinkRelations PAGING_PREV = new LinkRelations("previous");
        public static LinkRelations PAGING_FIRST = new LinkRelations("first");
        public static LinkRelations PAGING_LAST = new LinkRelations("last");
        public static LinkRelations ENCLOSURE = new LinkRelations("enclosure");
        public static LinkRelations CANONICAL = new LinkRelations("canonical");
        public static LinkRelations PREDECESSOR_VERSION = new LinkRelations("predecessor-version");
        public static LinkRelations TYPE = new LinkRelations("type");
        public static LinkRelations TYPES = new LinkRelations("types", true);
        public static LinkRelations FOLDERS = new LinkRelations("folders", true);
        public static LinkRelations DOCUMENTS = new LinkRelations("documents", true);
        public static LinkRelations OBJECTS = new LinkRelations("objects", true);
        public static LinkRelations CABINET = new LinkRelations("cabinet", true);
        public static LinkRelations CONTENT_MEDIA = new LinkRelations("content-media", true);
        public static LinkRelations PRIMARY_CONTENT = new LinkRelations("primary-content", true);
        public static LinkRelations CHECKED_OUT_OBJECTS = new LinkRelations("checked-out-objects", true);
        public static LinkRelations REPOSITORIES = new LinkRelations("repositories", true);
        public static LinkRelations ABOUT = new LinkRelations("about", false);
        public static LinkRelations RELATIONS = new LinkRelations("relations", true);
        public static LinkRelations RELATION_TYPE = new LinkRelations("relation-type", true);
        public static LinkRelations RELATION_TYPES = new LinkRelations("relation-types", true);
        public static LinkRelations BATCHES = new LinkRelations("batches", true);
        public static LinkRelations BATCH_CAPABILITIES = new LinkRelations("batch-capabilities", true);
        public static LinkRelations USERS = new LinkRelations("users", true);
        public static LinkRelations USER = new LinkRelations("user", true);
        public static LinkRelations GROUPS = new LinkRelations("groups", true);
        public static LinkRelations CURRENT_USER = new LinkRelations("current-user", true);
        public static LinkRelations DEFAULT_FOLDER = new LinkRelations("default-folder", true);
        public static LinkRelations MODIFIER = new LinkRelations("modifier", true);
        public static LinkRelations CREATOR = new LinkRelations("creator", true);
        public static LinkRelations OWNER = new LinkRelations("owner", true);
        public static LinkRelations LOCK_OWNER = new LinkRelations("lock-owner", true); 
        public static LinkRelations FORMAT = new LinkRelations("format", true);
        public static LinkRelations CHECKIN_NEXT_MAJOR = new LinkRelations("checkin-next-major", true);
        public static LinkRelations CHECKIN_NEXT_MINOR = new LinkRelations("checkin-next-minor", true);
        public static LinkRelations CHECKIN_BRANCH_VERSION = new LinkRelations("checkin-branch", true);
        public static LinkRelations CHECKOUT = new LinkRelations("checkout", true);
        public static LinkRelations CANCEL_CHECKOUT = new LinkRelations("cancel-checkout", true);
        public static LinkRelations CURRENT_VERSION = new LinkRelations("current-version", true);
        public static LinkRelations OBJECT_ID = new LinkRelations("object-id", true);
        public static LinkRelations FOLDER_PATH = new LinkRelations("folder-path", true);
        public static LinkRelations FOLDER_CHILDREN = new LinkRelations("folder-children", true);
        public static LinkRelations DQL = new LinkRelations("dql", true);
        public static LinkRelations SEARCH = new LinkRelations("search", true);
        public static LinkRelations CABINETS = new LinkRelations("cabinets", true);
        public static LinkRelations FORMATS = new LinkRelations("formats", true);
        public static LinkRelations NETWORK_LOCATIONS = new LinkRelations("network-locations", true);
        public static LinkRelations PARENT_LINKS = new LinkRelations("parent-links", true);
        public static LinkRelations CHILD_LINKS = new LinkRelations("child-links", true);

        /********************BEGIN D2 REST LINK RELATIONS ******************************/
        public static LinkRelations D2_CONFIGURATION = new LinkRelations("d2-configurations", true);
        public static LinkRelations SEARCH_CONFIGURATION = new LinkRelations("search-configuration", true);
        public static LinkRelations PROFILE_CONFIGURATION = new LinkRelations("profile-configuration", true);
        public static LinkRelations CREATION_PROFILES = new LinkRelations("creation-profile", true);
        public static LinkRelations CREATION_PROFILE = new LinkRelations("creation-profile", true);
        public static LinkRelations TYPE_CONFIGURATIONS = new LinkRelations("type-configurations", true);
        public static LinkRelations TYPE_CONFIGURATION = new LinkRelations("type-configuration", true);
        public static LinkRelations OBJECT_CREATION = new LinkRelations("object-creation", true);
        public static LinkRelations DOCUMENT_TEMPLATES = new LinkRelations("document-templates", true);
        public static LinkRelations DOCUMENT_TEMPLATE = new LinkRelations("document-template", true);
        public static LinkRelations COMMENTS = new LinkRelations("comments", true);
        public static LinkRelations COMMENT = new LinkRelations("comment", true);
        public static LinkRelations TASK_LIST = new LinkRelations("tasklist-d2", true);
        public static LinkRelations C2_VIEW = new LinkRelations("views/c2-view", true);
        public static LinkRelations C2_PRINT = new LinkRelations("views/c2-print", true);
        public static LinkRelations PROPERTY_PAGE = new LinkRelations("property-age", true);
        /** Temporary until Only on the NA/4.6 Version, once extensibility is in place. Apparantly some
        of the core rest link relations like checkout/checkin/cancelcheckout will be gone in D2 rest? */
        public static LinkRelations PREVIEW_URLs = new LinkRelations("http://identifiers.com/com/linkrel/preview-urls", true);



        private static string PREFIX = "http://identifiers.emc.com/linkrel/";
        private bool isDocumentum;
        private string rawRel;



        /// <summary>
        /// 
        /// </summary>
        public string Rel 
        {
            get
            {
                String sRel = rawRel;
                if (isDocumentum)
                {
                    sRel = PREFIX + rawRel;
                }
                return sRel;
            }
        }
    
        /// <summary>
        /// 
        /// </summary>
        /// <param name="rel"></param>
        private LinkRelations(string rel) {
            this.rawRel = rel;
            this.isDocumentum = false;
        }
    
        /// <summary>
        /// 
        /// </summary>
        /// <param name="rel"></param>
        /// <param name="isDocumentum"></param>
        private LinkRelations(string rel, bool isDocumentum) {
            this.rawRel = rel;
            this.isDocumentum = isDocumentum;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="links"></param>
        /// <param name="rel"></param>
        /// <returns></returns>
        public static string FindLinkAsString(List<Link> links, string rel)
        {
            if (links != null)
            {

                foreach(Link link in links)
                {
                    if (link.Rel.StartsWith(rel)) 
                    {
                        if(String.IsNullOrEmpty(link.Href))
                        {
                            string hrefT = link.Hreftemplate;
                            if (hrefT.Contains("%7B"))
                            {
                                hrefT = hrefT.Substring(0, hrefT.LastIndexOf("%7B"));
                            }
                            else if (hrefT.Contains("{"))
                            {
                                hrefT = hrefT.Substring(0, hrefT.LastIndexOf("{"));
                            }
                            return hrefT;                           
                        }
                        else
                        {
                            return link.Href;
                        }
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="links"></param>
        /// <param name="rel"></param>
        /// <returns></returns>
        public static Link FindLink(List<Link> links, string rel)
        {
            Link retLink = null;
            if (links != null)
            {

                foreach (Link link in links)
                {
                    if (link.Rel.StartsWith(rel))
                    {
                        return link;
                    }
                }
                if (retLink == null)
                {
                    // At the repository level, link rel is the URL so we cannot find by REL name, we have to find it by 
                    // what the REL ENDS with
                    foreach (Link link in links)
                    {
                        if (link.Rel.EndsWith(rel))
                        {
                            retLink = link;
                            retLink.Rel = rel;
                            return retLink;
                        }
                    }
                }
                return retLink;
            }
            return null;
        }
    }
}
