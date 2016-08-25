using Emc.Documentum.Rest.Net;
using Emc.Documentum.Rest.Http.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Net.Http;
using System.IO;
using System.IO.Compression;

namespace Emc.Documentum.Rest.DataModel
{
    public partial class Repository 
    {
        /// <summary>
        /// List all the versions of a document
        /// </summary>
        /// <param name="doc"></param>
        /// <returns>List</returns>
        public List<Document> getAllDocumentVersions(Document doc)
        {

            List<Document> docVersions = new List<Document>();

            Feed<Document> allVersions = ExecuteDQL<Document>(
                String.Format("select * from {0}(all) where i_chronicle_id='{1}'",
                doc.GetPropertyValue("r_object_type").ToString(),
                doc.GetPropertyValue("i_chronicle_id").ToString()),
                new FeedGetOptions { Inline = true, Links = true });
            List<Entry<Document>> docEntries = allVersions.Entries;
            foreach (Entry<Document> entry in docEntries)
            {
                Document vDoc = entry.Content;
                Document fullDoc = _client.Get<Document>(vDoc.Links[0].Href, null);
                fullDoc.SetClient(this.Client);
                docVersions.Add(fullDoc);
            }
            return docVersions;
        }

        /// <summary>
        /// Default convenience version of getDocumentHistory that returns 10 audit entries per page by default.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public Feed<AuditEntry> getDocumentHistory(HistoryType type, PersistentObject obj)
        {
            return getDocumentHistory(type, obj, 10);
        }

        /// <summary>
        /// Takes a keyValuepair argument of i_chronicleId=x or r_object_id=x 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="obj"></param>
        /// <param name="itemsPerPage"></param>
        /// <returns></returns>
        public Feed<AuditEntry> getDocumentHistory(HistoryType type, PersistentObject obj, int itemsPerPage)
        {
            String auditQueryAttribute = "";
            String id = "";

            switch (type)
            {
                case HistoryType.FULLVERSIONTREE:
                    id = (String)obj.GetPropertyValue("i_chronicle_id");
                    auditQueryAttribute = "chronicle_id";
                    break;
                case HistoryType.THISDOCUMENTONLY:
                    id = (String)obj.GetPropertyValue("r_object_id");
                    auditQueryAttribute = "audited_obj_id";
                    break;


            }
            String dql = String.Format("select * from dm_audittrail where {0}='{1}'", auditQueryAttribute, id);
            return ExecuteDQL<AuditEntry>(dql, new FeedGetOptions() { ItemsPerPage = itemsPerPage, IncludeTotal = true });
        }

        /// <summary>
        /// Given a path, starts from the bottom to see if the folder path exists and if not, finds
        /// the highest level folder in the path that does exists and returns a list of folders(toCreate)
        /// that can be used to create the complete path.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="toCreate"></param>
        /// <returns>Folder</returns>
        private Folder findExistingAndNonExistingFolders(String path, List<String> toCreate) 
        {
            //Break apart the path so we can check from the bottom up which folder(s) exist.
            String[] pathFragments = path.Split(new String[] { "/" }, StringSplitOptions.RemoveEmptyEntries);

            Folder existingFolder = null;
            int iFrags = pathFragments.Length - 1;
            for (int i = iFrags; i > -1; i--)
            {
                StringBuilder pathBuilder = new StringBuilder();
                String lastFolder = "";
                for (int p = 0; p < i + 1; p++)
                {
                    lastFolder = pathFragments[p];
                    pathBuilder.Append("/").Append(lastFolder);

                }
                existingFolder = getFolderByQualification("dm_folder where any r_folder_path = '" + pathBuilder.ToString() + "'", null);
                if (existingFolder == null)
                {
                    toCreate.Add(lastFolder);
                }
                else
                {
                    break;
                }
            }
            return existingFolder;
        }

        /// <summary>
        /// Creates a list of folders from bottom to top (reverse) order in the toCreate list
        /// then returns the last folder created
        /// </summary>
        /// <param name="createCabinet"></param>
        /// <param name="toCreate"></param>
        /// <param name="existingFolder"></param>
        /// <returns>Folder</returns>
        private Folder createListOfFolders(bool createCabinet, List<String> toCreate, Folder existingFolder)
        {

            // Reverse through the toAdd collection to create any folders that need creating
            for (int f = toCreate.Count - 1; f > -1; f--)
            {
                if (createCabinet)
                {
                    Cabinet obj = NewCabinet(toCreate[f]);
                    existingFolder = (Folder)obj;
                    createCabinet = false;
                }
                else
                {
                    existingFolder = NewFolder(existingFolder, toCreate[f]); 
                }

            }
            return existingFolder;
        }

        /// <summary>
        /// Delete all versions of a document
        /// </summary>
        /// <param name="doc"></param>

        public void deleteAllDocumentVersions(Document doc)
        {
            GenericOptions options = new GenericOptions();
            options.SetQuery("del-version", "all");
            doc.Delete(options);
        }

        /// <summary>
        /// Delete the current version of a document 
        /// </summary>
        /// <param name="doc"></param>
        public void deleteCurrentDocumentVersion(Document doc)
        {
            GenericOptions options = new GenericOptions();
            options.SetQuery("del-version", "selected");
            doc.Delete(options);
        }

        /// <summary>
        /// Delete a folder. If descending is true, all subfolders and contents will be
        /// deleted as well. If allLinks is true, then if the folder or any of its contained
        /// folders are also linked to other locations, the folder will be removed from those 
        /// locations as well.
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="descending"></param>
        /// <param name="allLinks">Specifies whether a multi-linked descendant is deleted
        /// or unlinked from this specified folder. 
        /// • true - This operation deletes a multi-linked descendant along with all its 
        ///   folder links.
        ///   • false - This operation only deletes a descendant’s link to this folder. 
        ///   The descendant is still linked to other folders.
        ///   </param>
        public void deleteFolder(Folder folder, bool descending, bool allLinks)
        {
            GenericOptions options = new GenericOptions();
            options.SetQuery("del-non-empty", descending);
            options.SetQuery("del-all-links", allLinks);
            folder.Delete(options);
        }

        /// <summary>
        /// Moves a document from orgin path to destination path given document's objectID
        /// </summary>
        /// <param name="objectId"></param>
        /// <param name="originPath"></param>
        /// <param name="destinationPath"></param>
        /// <returns>Bool</returns>
        public bool moveDocument(string objectId, string originPath, string destinationPath)
        {
            Folder origin = getFolderByQualification(String.Format("dm_folder where any r_folder_path='{0}'", originPath), null);
            Folder destination = getFolderByQualification(String.Format("dm_folder where any r_folder_path='{0}'", destinationPath), null);
            Document doc = getObjectById<Document>(objectId); // getDocumentByQualification(String.Format("dm_document where r_object_id = '{0}'", objectId), null);
            return moveDocument(doc, origin, destination);
        }

        /// <summary>
        /// Link a folder by giving the full path to the folder in the form:
        /// /Cabinet/folder/subfolder..... The path MUST exist and will not
        /// be created.
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="path"></param>
        /// <returns>FolderLink</returns>
        public FolderLink linkToFolder<T>(T obj, String path)
        {
            Folder folder = getFolderByPath(path);
            if (folder == null)
            {
                throw new Exception("No Such folder: " + path + " found");
            }
            return linkToFolder(obj, folder);
        }

        /// <summary>
        /// Given a folder, link the passed document to the folder
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="folder"></param>
        /// <returns>Folderlink</returns>
        public FolderLink linkToFolder<T>(T obj, Folder folder)
        {
            Folder hrefFolder = folder.CreateHrefObject<Folder>();
            String folderId = folder.GetPropertyValue("r_object_id").ToString();
            PersistentObject pObj = (obj as PersistentObject);
            if(pObj.GetRepeatingValuesAsString("i_folder_id",",").Contains(folderId)) {
                return pObj.GetFolderLinks(new FeedGetOptions { Inline = true, Links = true }).FindInlineEntry(folderId);
            }
            return pObj.LinkToFolder(hrefFolder, null);
        }

        /// <summary>
        /// Given an object id and a path, make a copy of the object at the given path. 
        /// If the path does not exist, it will be created
        /// </summary>
        /// <param name="objectID"></param>
        /// <param name="destinationPath"></param>
        /// <returns>RestDocument</returns>
        public Document copyDocument(string objectID, string destinationPath)
        {
            Document doc = getObjectById<Document>(objectID); // getDocumentByQualification("dm_document where r_object_id = '" + objectID + "'", null);

            Folder tempFolder = getOrCreateFolderByPath(destinationPath);

            return copyDocument(doc, tempFolder);
        }

        /// <summary>
        /// Given a RestDocument, copy it to the given Folder
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="destination"></param>
        /// <returns>RestDocument</returns>
        public Document copyDocument(Document doc, Folder destination)
        {
            Document copyDoc = doc.CreateHrefObject<Document>();
            Document copiedDoc = destination.CreateSubObject<Document>(copyDoc, null);

            return copiedDoc;
            
        }

        /// <summary>
        /// Move a document from origin location to destination location
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="origin"></param>
        /// <param name="destination"></param>
        /// <returns>Bool</returns>
        public bool moveDocument(Document doc, Folder origin, Folder destination) 
        {
            bool ret = true;
            try
            {
                Feed<FolderLink> links = doc.GetFolderLinks(new FeedGetOptions { Inline = true, Links = true });
                FolderLink folderLink3 = links.FindInlineEntryBySummary((string)origin.GetPropertyValue("r_object_id"));
                Folder moveTo = destination.CreateHrefObject<Folder>();
                FolderLink movedFolderLink = folderLink3.MoveTo(moveTo, null);
               // movedFolderLink.Remove();
            }
            catch (Exception e)
            {
                Console.WriteLine("Unable to move RestDocument: " + doc.GetPropertyValue("object_name")
                    + "from " + origin.GetPropertyValue("object_name") + " to " 
                    + destination.GetPropertyValue("object_name") + "\n" + e.StackTrace);
            }
            return ret;
        }

        /// <summary>
        /// Creates a new folder of the the default repository FolderType with the "name" given and links it 
        /// to the "parent" folder specified.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="name"></param>
        /// <returns>Folder</returns>
        public Folder NewFolder(Folder parent, String name)
        {
            if (parent == null) return null;
            if (FolderType == null) FolderType = "dm_folder";
            Folder obj = new Folder();
            obj.Name = "folder";
            obj.Type = FolderType;
            obj.SetPropertyValue("object_name", name );
            obj.SetPropertyValue("r_object_type", FolderType);
           
            return parent.CreateSubFolder(obj, new FeedGetOptions { Inline = true, Links = true });
        }
        /// <summary>
        /// Creates a new folder of the specified "objectType" with the "name" given and links it 
        /// to the "parent" folder specified.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="name"></param>
        /// <param name="objectType"></param>
        /// <returns>Folder</returns>
        public Folder NewFolderByType(Folder parent, String name, string objectType)
        {
            Folder obj = new Folder();
            obj.Name = "folder";
            obj.Type = FolderType;
            obj.SetPropertyValue("object_name", name);
            obj.SetPropertyValue("r_object_type", objectType);
            return parent.CreateSubFolder(obj, new FeedGetOptions { Inline = true, Links = true });
        }

        /// <summary>
        /// Creates a new cabinet object of the "typeName" specified with the needed self link to allow the 
        /// new object to be created. 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="objectType"></param>
        /// <returns>Cabinet</returns>
        public Cabinet NewCabinetByType(string name, string objectType )
        {
            Cabinet obj = new Cabinet();
            obj.Client = Client;
            obj.Name = "cabinet";
            Link cabinets = new Link();
            cabinets.Href = LinkRelations.FindLink(Links, "cabinets").Href;
            cabinets.Rel = "self";
            obj.Links.Add(cabinets);
            obj.Type = objectType;
            obj.SetPropertyValue("object_name", name);
            obj.SetPropertyValue("r_object_type", objectType);
            obj.Save();
            return obj.Fetch<Cabinet>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="file"></param>
        /// <returns>RestDocument</returns>
        public Document ImportDocumentAsNewVersion(Document doc, FileInfo file)
        {
            GenericOptions checkinOptions = new GenericOptions();
            checkinOptions.SetQuery("format", ObjectUtil.getDocumentumFormatForFile(file.Name));
            checkinOptions.SetQuery("page", 0);
            checkinOptions.SetQuery("primary", true);
            checkinOptions.SetQuery("version-label", "ImportAsNewVersion");
            return ImportDocumentAsNewVersion(doc, file.OpenRead(), ObjectUtil.getMimeTypeFromFileName(file.Name), checkinOptions);
        }

        public Document ImportDocumentAsNewVersion(Document doc, Stream contentStream, String mimeType, GenericOptions checkinOptions)
        {
            Feed<OutlineAtomContent> versions = doc.GetVersionHistory<OutlineAtomContent>(null);
            List<Entry<OutlineAtomContent>> entries = versions.Entries;

            // If the document is not already checked out, check it out.
            if (!doc.IsCheckedOut()) doc = doc.Checkout();
            Document checkinDoc = NewDocument(doc.GetPropertyValue("object_name").ToString());
            if(!checkinOptions.ContainsParam("format")) {
                checkinOptions.SetQuery("format", doc.GetPropertyValue("a_content_type"));
            }
            if (!checkinOptions.ContainsParam("page"))
            {
                checkinOptions.SetQuery("page", 0);
            }
            if(!checkinOptions.ContainsParam("primary")) {
                checkinOptions.SetQuery("primary", true);
            }
            
            return doc.CheckinMinor(doc, contentStream, mimeType, checkinOptions);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objectId"></param>
        /// <param name="file"></param>
        /// <returns>RestDocument</returns>
        public Document ImportDocumentAsNewVersion(string objectId, FileInfo file)
        {
            Document doc = getObjectById<Document>(objectId); 
            return ImportDocumentAsNewVersion(doc, file);
            
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <param name="documentName"></param>
        /// <param name="repositoryPath"></param>
        /// <returns>RestDocument</returns>
        public Document ImportNewDocument(FileInfo file, string documentName, string repositoryPath) {
            if (!repositoryPath.StartsWith("/")) throw new Exception("Repository path " + repositoryPath + " is not valid."
                 + " The path must be a fully qualified path");
            Folder importFolder = getOrCreateFolderByPath(repositoryPath);
            if (importFolder == null) throw new Exception("Unable to fetch or create folder by path: " + repositoryPath);

            Document newDocument =  NewDocument(documentName, DocumentType);
            GenericOptions importOptions = new GenericOptions();
            importOptions.SetQuery("format", ObjectUtil.getDocumentumFormatForFile(file.Extension));
            newDocument = importFolder.ImportDocumentWithContent(newDocument, file.OpenRead(), ObjectUtil.getMimeTypeFromFileName(file.Name), importOptions);

            return newDocument;
        }

        public string getRepositoryUri()
        {
            return LinkRelations.FindLinkAsString(this.Links, LinkRelations.SELF.Rel);
        }

        /// <summary>
        /// Imports an email into the system
        /// </summary>
        /// <param name="file"></param>
        /// <param name="documentName"></param>
        /// <param name="repositoryPath"></param>
        /// <returns>EmailPackage</returns>
        public EmailPackage ImportEmail(FileInfo file, string documentName, string repositoryPath)
        {
            if (!repositoryPath.StartsWith("/")) throw new Exception("Repository path " + repositoryPath + " is not valid."
                 + " The path must be a fully qualified path");
            Folder importFolder = getOrCreateFolderByPath(repositoryPath);
            if (importFolder == null) throw new Exception("Unable to fetch or create folder by path: " + repositoryPath);
            Document newDocument = NewDocument(documentName, DocumentType);
            GenericOptions importOptions = new GenericOptions();
            importOptions.SetQuery("format", ObjectUtil.getDocumentumFormatForFile(file.Extension));

            EmailPackage email = importFolder.ImportEmail(newDocument, file.OpenRead(), ObjectUtil.getMimeTypeFromFileName(file.Name), importOptions);

            return email;
        }

        /// <summary>
        /// Creates a new cabinet object with the needed self link to allow the new object to be created. 
        /// The TypeName set for the repository instance is what is used for creation. The default is 
        /// dm_cabinet.
        /// </summary>
        /// <param name="name"></param>
        /// <returns>Cabinet</returns>
        public Cabinet NewCabinet(String name)
        {
            Cabinet obj = new Cabinet();
            obj.Client = Client;
            obj.Name = "cabinet";
            Link cabinets = new Link();
            cabinets.Href = LinkRelations.FindLink(Links, "cabinets").Href;
            cabinets.Rel = "self";
            obj.Links.Add(cabinets);
            obj.Type = "dm_cabinet";
            obj.SetPropertyValue("object_name", name);
            obj.SetPropertyValue("r_object_type", CabinetType);
            obj.Save();
            return obj;
        }

        public enum RecordType
        {
            MLAT,
            Extradition
        }

        /// <summary>
        /// Get a list of persistent Objects
        /// </summary>
        /// <param name="type"></param>
        /// <param name="folder"></param>
        /// <param name="closeDate"></param>
        /// <returns>List</returns>
        public List<PersistentObject> CloseFolderAndStartRetention(RecordType type, Folder folder, DateTime closeDate) 
        {
            FeedGetOptions opts = new FeedGetOptions { Inline = true, Links = true};
            
            Folder recordFolder = getFolderByPath("/SystemA File Plan/INC/" + type.ToString());
            string[] folderIds = folder.GetRepeatingValuesAsString("i_folder_id", ",").Split(new string[] {","}, StringSplitOptions.RemoveEmptyEntries);
            if(!folderIds.Contains(recordFolder.GetPropertyValue("r_object_id").ToString())) {
                Folder linkFolder = recordFolder.CreateHrefObject<Folder>();
                FolderLink folderLink2 = folder.LinkToFolder(linkFolder, null);
            }
            
            Feed<PersistentObject> feed = UpdateCloseDate(folder.GetPropertyValue("r_object_id").ToString(), closeDate);
            return feed != null ? ObjectUtil.getFeedAsList(feed, true) : null;
        }
        
        /// <summary>
        /// Used to update the rentention condition date (close date) on a folder. To re-open, set closeDate = new DateTime() aka NullDate
        /// </summary>
        /// <param name="folderId"></param>
        /// <param name="closeDate"></param>
        /// <returns>Feeed</returns>
        public Feed<PersistentObject> UpdateCloseDate(String folderId, DateTime closeDate)
        { 
            //http://localhost:8080/dctm-rest/repositories/Process/retention.xml?folderId=xxxxxxxxxxxxxxxx&closeDate=12/2/2014
            String updateCloseDateUri = LinkRelations.FindLinkAsString(Links, "self") + LinkRelations.RETENTION;
            if (closeDate.Ticks == new DateTime().Ticks)
            {
                // If we have a DateTime nulldate, convert to java null date value
                closeDate = DateTime.Parse("1/1/1970 00:00:00");
            }
            FeedGetOptions options = new FeedGetOptions();
            options.SetQuery("folderId", folderId);
            options.SetQuery("closeDate", closeDate);
            return Client.GetFeed<PersistentObject>(updateCloseDateUri, options);
        }
        /// <summary>
        /// Given a path, will create any folders in the path that do not exist, then return the 
        /// folder object for the path.
        /// </summary>
        /// <param name="path"></param>
        /// <returns>Folder</returns>
        public Folder getOrCreateFolderByPath(String path)
        {
            // The list of folders that do not exist so we can create them
            List<String> toCreate = new List<String>();
            Folder existingFolder = findExistingAndNonExistingFolders(path, toCreate);

            Boolean createCabinet = existingFolder == null? true: false;
            existingFolder = createListOfFolders(createCabinet, toCreate, existingFolder);
            return existingFolder;
        
        }

        /// <summary>
        /// Find objects in the system with the same content (duplicates). If the 
        /// folderPath argument is not null, it will check for duplicates only 
        /// within the given folder path.
        /// </summary>
        /// <param name="objectId"></param>
        /// <param name="folderPath"></param>
        /// <returns>List</returns>
        public List<PersistentObject> CheckForDuplicate(String objectId, String folderPath)
        {
            List<PersistentObject> dupeObjs = new List<PersistentObject>();
            String dql = null;
            if(folderPath != null && !folderPath.Trim().Equals("")) {
                dql = String.Format("select parent_id from dmr_content where r_content_hash in (select "
                    + "r_content_hash from dmr_content where any parent_id = '{0}') "
                    + "and (not any parent_id = '{0}' and any parent_id in (select "
                    + "r_object_id from dm_sysobject where FOLDER('{1}')))", objectId, folderPath);
            }
            else
            {
                dql = String.Format("select parent_id from dmr_content where r_content_hash in (select "
                        + "r_content_hash from dmr_content where any parent_id = '{0}') "
                        + "and not any parent_id = '{0}' ENABLE(ROW_BASED)", objectId);
            }
            Feed<PersistentObject> queryResult = ExecuteDQL<PersistentObject>(dql, null);
            if (queryResult != null)
            {
                List<Entry<PersistentObject>> objs = (List<Entry<PersistentObject>>)queryResult.Entries;
                foreach (Entry<PersistentObject> obj in objs)
                {
                    dupeObjs.Add(obj.Content);
                }
            }
            return dupeObjs;
        }

        /// <summary>
        /// Creates a new document based on type passed
        /// </summary>
        /// <param name="objectType"></param>
        /// <returns>RestDocument</returns>
        public Document NewDocument(string objectType)
        {
            return NewDocument(null, objectType);
        }

        /// <summary>
        /// Returns a new shell document with the object_name set to objectName and 
        /// the r_object_type set to objectType
        /// </summary>
        /// <param name="objectName">Used to set the document's name</param>
        /// <param name="objectType">Indicates the object type of the document</param>
        /// <returns>RestDocument</returns>
        public Document NewDocument(string objectName, string objectType)
        {
            Document doc = new Document();
            doc.Name = "document";
            doc.Type = objectType;
            if(objectName != null && !objectName.Trim().Equals(""))
                doc.SetPropertyValue("object_name", objectName);
            doc.SetPropertyValue("r_object_type", objectType);
            return doc;
        }

        /// <summary>
        /// Gets a folder by the logical path
        /// </summary>
        /// <param name="path"></param>
        /// <returns>Folder</returns>
        public Folder getFolderByPath(string path)
        {
            Folder folder = getFolderByQualification(
                String.Format("dm_folder where any r_folder_path = '{0}'", 
                path), new FeedGetOptions { Inline = true, Links = true });
            return folder;
        }

       /// <summary>
         /// Exports a Folder and all associated files under that folder structure as a zip
        /// objectId is the folder's r_object_id
       /// </summary>
       /// <param name="folderPath"></param>
       /// <param name="zipFileName"></param>
       /// <returns>FileInfo</returns>
        public FileInfo ExportFolder(string folderPath, string zipFileName)
        {
            //Get the name of the zip file you are creating.
            string folderName = folderPath.Substring(folderPath.LastIndexOf("/") + 1);

            string zipfilename = System.Guid.NewGuid().ToString() + ".zip";
            string query = String.Format("select doc.r_object_id, doc.object_name, fol.r_folder_path from dm_document doc, dm_folder fol "
                + "where doc.i_folder_id = fol.r_object_id and fol.r_folder_path like '{0}%'  enable(row_based)", folderPath);  
             /*"select doc.r_object_id,doc.object_name, fol.r_folder_path from dm_sysobject s," +
             " dm_sysobject_r sr, dm_sysobject_r sr3, dm_folder_r fol, dm_document doc  where s.i_is_deleted = 0" +
             " and sr3.r_object_id = s.r_object_id and sr3.i_position = -1 and sr.r_object_id = s.r_object_id" +
             " and fol.r_object_id = sr.i_folder_id and fol.i_position = -1 and doc.r_object_id = s.r_object_id" +
             " and FOLDER('" + folderPath + "', DESCEND) order by fol.r_folder_path,s.object_name";
             */
            return ExportToZip(query, folderName, zipFileName);


        }

        /// <summary>
        /// Exports a file(s) to a zip file based on a qualifying DQL statement
        /// </summary>
        /// <param name="query"></param>
        /// <param name="folderName"></param>
        /// <param name="zipFileName"></param>
        /// <returns>FileInfo</returns>
        private FileInfo ExportToZip(string query, string folderName, string zipFileName)
        {
            try
            {
                Feed<PersistentObject> queryResult = ExecuteDQL<PersistentObject>(query, new FeedGetOptions() { ItemsPerPage = 100, IncludeTotal = true });
                if (queryResult != null && queryResult.Total > 0)
                {
                    int totalResults = queryResult.Total;
                    double totalPages = queryResult.PageCount;
                    int docProcessed = 0;
                    Dictionary<string, int> kp = new Dictionary<string, int>();
                    
                    for (int i = 0; i < totalPages; i++)
                    {
                        List<PersistentObject> objs = ObjectUtil.getFeedAsList(queryResult, false);
                        foreach (PersistentObject obj in objs)
                        {
                            SingleGetOptions options = new SingleGetOptions();
                            options.SetQuery("media-url-policy", "local");
                            Document doc = getObjectById<Document>(obj.GetPropertyValue("r_object_id").ToString());// getDocumentByQualification("dm_document where r_object_id='" + obj.getAttributeValue("r_object_id") + "'", null);
                            ContentMeta primaryContentMeta = doc.GetPrimaryContent(options);
                            FileInfo downloadedContentFile = primaryContentMeta.DownloadContentMediaFile();
                            // string path = "c:/temp/case" + obj.getAttributeValue("r_folder_path").ToString();
                            string filename = downloadedContentFile.Name;

                            // If a file that is the same name is exported, give it a unique name
                            if (kp.Keys.Contains(filename))
                            {
                                int len = filename.LastIndexOf(".");
                                if (len < 0) len = filename.Length;
                                kp[filename] += 1;
                                string insertinto = "_" + kp[filename].ToString();
                                filename = filename.Insert(len, insertinto);
                                kp.Add(filename, 0);
                            }
                            else { kp.Add(filename, 0); }


                            string zipPath = "";
                            if (folderName == null)
                            {
								zipPath = Path.AltDirectorySeparatorChar + filename;
                                //zipPath = zipPath.Replace(@"/", @"\");
                            }
                            else
                            {
                                zipPath = obj.GetPropertyValue("r_folder_path").ToString();
                                zipPath = zipPath.Substring(zipPath.IndexOf(folderName));
                                if (zipPath.Length == 0)
                                { zipPath = zipPath + filename; }
                                else
								{ zipPath = zipPath + Path.AltDirectorySeparatorChar + filename; }
                                //zipPath = zipPath.Replace(@"/", @"\");
                                //pathinzip = pathinzip.Substring(pathinzip.IndexOf(@"\") + 1);
                            }
                            addFileToZipArchive(downloadedContentFile, zipPath, new FileInfo(zipFileName));
                            docProcessed++;
                        }

                        if (totalResults != docProcessed) queryResult = queryResult.NextPage();
                    }

                } // End if query result null
                else
                {
                    throw new Exception("NORESULTSTOZIP: Query returned no results - " + query);
                }
            }
            catch (Exception e)
            {
                Client.Logger.WriteToLog(Utility.LogLevel.DEBUG, "ZIPEXPORT", "ZIP EXPORT FAILED: ", e);
            }
            return null;
        }

        /// <summary>
        /// Gets a list of documents by object IDs; Zips them and returns Zip file location
        /// </summary>
        /// <param name="objectIDs"></param>
        /// <param name="zipFileName"></param>
        /// <returns>FileInfo</returns>
        public FileInfo ExportDocuments(string objectIDs, string zipFileName)
        {
            objectIDs = "'" + objectIDs.Replace(",", "','") + "'";
            string query = String.Format("select r_object_id,object_name from dm_document where r_object_id IN({0})", objectIDs);
            return ExportToZip(query, null, zipFileName);
        }

        /// <summary>
        /// Used to zip files pulled from the documentum system.
        /// </summary>
        /// <param name="fileToZip"></param>
        /// <param name="pathInZip"></param>
        /// <param name="zipArchive"></param>
        public void addFileToZipArchive(FileInfo fileToZip, string pathInZip, FileInfo zipArchive)
        {
            //Create Empty Archive
            using (ZipArchive archive = ZipFile.Open(zipArchive.FullName, ZipArchiveMode.Update))
            {
                ZipArchiveEntry FileInArchive = archive.CreateEntry(pathInZip);
                ZipArchiveEntry zippedFile = archive.CreateEntryFromFile(fileToZip.FullName, pathInZip);
            }
        }

    } // End Repository Class
}
