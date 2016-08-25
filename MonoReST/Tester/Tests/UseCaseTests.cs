using Emc.Documentum.Rest.DataModel;
using Emc.Documentum.Rest.CustomModel;
using Emc.Documentum.Rest.Net;
using Emc.Documentum.Rest.Http.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Configuration;
using System.Collections.Specialized;
using System.Windows.Forms;
using System.Threading;
using Emc.Documentum.Rest.Utility;

namespace Emc.Documentum.Rest.Test
{
    /// <summary>
    /// 
    /// </summary>
    public class UseCaseTests : IDisposable
    {
        protected RestController client;
        protected string RestHomeUri;
        protected string repositoryName;
        protected bool printResult;
        protected int numDocs;
        protected int threadNum;
        protected string tempPath;
        protected DateTime testStart;
        protected string testPrefix;

        protected string testDirectory = @"C:\Temp\Export";
        protected string importEmailsDirectory = @"C:\Temp\Import\Emails";
        protected string importFilesDirectory = @"C:\Temp\Import\Files";
        protected string renditionsDirectoryName = @"Renditions";
        protected string primaryContentDirectoryName = @"PrimaryContent";
        protected string ProcessBasePath = "/RestTester/TestFiles/";

        protected List<string> childList = new List<string>();
        protected string parentFolderId;
        protected bool openEachFile = false;
        protected bool showdownloadedfiles = false;
        protected Logger loggerForm = null;
        // Disposable.
        protected bool _disposed;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="RestHomeUri"></param>
        /// <param name="repositoryName"></param>
        /// <param name="printResult"></param>
        /// <param name="path"></param>
        /// <param name="ThreadNum"></param>
        /// <param name="numDocs"></param>
        public UseCaseTests(RestController client, string RestHomeUri, string repositoryName, bool printResult, string path, int ThreadNum, int numDocs)
        {
            this.client = client;
            this.RestHomeUri = RestHomeUri;
            this.repositoryName = repositoryName;
            this.printResult = printResult;
            this.tempPath = path;
            this.threadNum = ThreadNum;
            this.numDocs = numDocs;
            this.testStart = DateTime.Now;
            this.testPrefix = testStart.ToString("yyyyMMddhhmmss")+"-"+threadNum;
            this.parentFolderId = "PARENT-" + testPrefix; // new Random().Next(0, 5); ;
            client.Logger = new LoggerFacade("RestServices", "NA", parentFolderId, parentFolderId);
        }


        private void addSupportingDoc(string childFolderDoc) {
			if(!childList.Contains(childFolderDoc)) {
				childList.Add(childFolderDoc);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public UseCaseTests()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="output"></param>
        protected void WriteOutput(Object output) {
            string outtext = null;
            if (output is String)
            {
                outtext = (String)output;
                byte[] bytes = Encoding.ASCII.GetBytes(outtext);
                outtext = Encoding.ASCII.GetString(bytes);
                // Make sure there are no BEEPs left in the string
                outtext = outtext.Replace('\x0007', ' ');
                outtext = outtext.Replace("\t", "   ");
            }
            if (loggerForm != null)
            {
                Application.DoEvents();
                loggerForm.appendText(output + "\n");
            }
            else
            {
                Console.WriteLine(outtext == null? output : outtext);
            }
            File.AppendAllText(testDirectory + Path.DirectorySeparatorChar + "ProcessDocumentumTest.txt", DateTime.Now + "-" + output + "\n");
            if (loggerForm != null) Application.DoEvents();
        }

        public string getPathRelativeToExecutable(string path)
        {
			if (!path.Contains(":") && !path.StartsWith(""+Path.DirectorySeparatorChar))
            {
                string exeDirectory = AppDomain.CurrentDomain.BaseDirectory;
				if (path.StartsWith(""+Path.DirectorySeparatorChar))
                {
                    path = exeDirectory + path.Substring(1);
                }
                else
                {
                    path = exeDirectory + path;
                }
            }
            return path;
        }



        protected void getPreferences()
        {
            bool useFormLogging = false;
            NameValueCollection testConfig = null;
            try {
				testConfig = ConfigurationManager.GetSection("restconfig") as NameValueCollection;
			} catch(ConfigurationErrorsException se) {
				Console.WriteLine("Configuration could  not load. If you are running under Visual Studio, ensure:\n" +
					"\n\"<section name=\"restconfig\" type=\"System.Configuration.NameValueSectionHandler\"/> is used. " +
					"\nIf running under Mono, ensure: " + 
					"\n<section name=\"restconfig\" type=\"System.Configuration.NameValueSectionHandler,System\"/> is used");
				Console.WriteLine ("\n\n" + se + "\n\n");
			}
            if (testConfig != null)
            {
                useFormLogging = Boolean.Parse(testConfig["useformlogging"].ToString());

                openEachFile = Boolean.Parse(testConfig["openeachfile"].ToString());
                showdownloadedfiles = Boolean.Parse(testConfig["showdownloadedfiles"].ToString());

                // Setup logger and peroformance output
                string LogThreshold = testConfig["LogThreshold"].ToString();
                if (client.Logger != null)
                {
                    if (LogThreshold.ToLower().Trim().Equals("debug"))
                    {
                        client.Logger.LogLevelThreshold = LogLevel.DEBUG;
                        client.Logger.TestDirectory = testDirectory;
                        client.Logger.isPerformance = Boolean.Parse(testConfig["performancedatatoconsole"].ToString());
                    }
                }

                testDirectory = testConfig["exportFilesDirectory"];
                if (!testDirectory.EndsWith(Path.DirectorySeparatorChar + ""))
                {
                    testDirectory = testDirectory + Path.DirectorySeparatorChar + "";
                }
                testDirectory = testDirectory + testPrefix;
                testDirectory = getPathRelativeToExecutable(testDirectory);
                if (!Directory.Exists(testDirectory))
                {
                    DirectoryInfo dir = Directory.CreateDirectory(testDirectory);

                    String msg = "A temporary content export directory: '" + testDirectory + "' is created by the program to save "
                        + "content files to.";
                    WriteOutput(msg);
                }

                importFilesDirectory = testConfig["importFilesDirectory"].ToString();
                importFilesDirectory = getPathRelativeToExecutable(importFilesDirectory);
                if (!Directory.Exists(importFilesDirectory))
                {
                    String msg = "Unable to find the directory specified: '" + importFilesDirectory + "' to pull "
                        + "random content files from. Unable to proceed. Please set an import files directory manually. Input the directory path below:";
                    WriteOutput(msg);
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    importFilesDirectory = Console.ReadLine().Trim();
                    Console.ForegroundColor = ConsoleColor.White;
                }
                FileInfo[] filesInfo = new DirectoryInfo(importFilesDirectory).GetFiles();
                while (filesInfo.Length == 0)
                {
                    String msg = "The import directory: '" + importFilesDirectory + "' cannot be empty. Please try to put some files in the directory and " +
                        "press Enter to continue..";
                    Console.WriteLine(msg);
                    Console.ReadLine();
                    filesInfo = new DirectoryInfo(importFilesDirectory).GetFiles();
                }

                importEmailsDirectory = testConfig["importEmailsDirectory"].ToString();
                importEmailsDirectory = getPathRelativeToExecutable(importEmailsDirectory);
                renditionsDirectoryName = testDirectory + Path.DirectorySeparatorChar + "Renditions";
                primaryContentDirectoryName = testDirectory + Path.DirectorySeparatorChar + "PrimaryContent";
                if (!Directory.Exists(importEmailsDirectory))
                {
                    String msg = "Unable to find the directory specified: '" + importEmailsDirectory + "' to pull"
                        + " random email files from. If customized Rest email processing is installed, the"
                        + " customized email test will fail. This message can normally be ignored unless you"
                        + " have the custom Rest email adapter installed.";
                    WriteOutput(msg);
                }
                Directory.CreateDirectory(primaryContentDirectoryName);
                Directory.CreateDirectory(renditionsDirectoryName);
            }
            if(Type.GetType("Mono.Runtime") != null && useFormLogging && threadNum > 1)
            {
                WriteOutput("*** Form logging cannot be used for multiple threads on Mono, setting to false.");
                useFormLogging = false;
            }
            if (useFormLogging)
            {
                loggerForm = new Logger();
                loggerForm.setTitle(parentFolderId);
                loggerForm.AutoSize = true;
                loggerForm.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
                int offset = threadNum == 0 ? threadNum : threadNum * 250;
                loggerForm.Top = threadNum + offset;
                loggerForm.Left = 0;
                loggerForm.getLoggerTextBox().Height = 250;
                loggerForm.getLoggerTextBox().Width = 1024;
                loggerForm.Show();
            }      
        }

        private List<DocumentTracker> Tracker { get; set; }

        private bool runTestByName(String testName)
        {
            bool success = false;
            try
            {
                // Import all documents into the holding area (Instantiation Form) before the documents are moveed to parentFolder/child
                WriteOutput("-----BEGIN " + testName + "--------------");
                long tStart = DateTime.Now.Ticks;
                switch(testName)
                {
                    case "CreateTempDocs":
                        CreateTempDocs();
                        WriteOutput("Created " + numDocs + " in " + ((DateTime.Now.Ticks - tStart) / TimeSpan.TicksPerMillisecond) + "ms");
                        break;
                    case "tracker":
                        MoveToFolders();
                        WriteOutput("Moveed " + Tracker.Count + " in " + ((DateTime.Now.Ticks - tStart) / TimeSpan.TicksPerMillisecond) + "ms");
                        break;
                    case "CreateFromTemplate":
                        // Create 10 documents by choosing from a random list of templates
                        CreateFromTemplate();
                        WriteOutput("Created 10 documents from template in " + ((DateTime.Now.Ticks - tStart) / TimeSpan.TicksPerMillisecond) + "ms");
                        break;
                    case "MoveDocumentsToParent":
                        // Randomly take some moveed documents and re-move them (Move from a temp location to another location))
                        MoveChildFolderDocumentsToParent();
                        WriteOutput("Moveed " + Math.Ceiling(Tracker.Count * .30) + " in " 
                            + ((DateTime.Now.Ticks - tStart) / TimeSpan.TicksPerMillisecond) + "ms");
                        break;

                    case "GetDocumentForView":
                        // Take 30% of the documents, and download the content, optionally, will open each one for viewing
                        for (int p = 0; p < (Math.Ceiling(Tracker.Count * .3)); p++)
                        {
                            ViewDocument(primaryContentDirectoryName, Tracker[p], openEachFile);
                        }
                        if (showdownloadedfiles) System.Diagnostics.Process.Start(primaryContentDirectoryName);
                        WriteOutput("Re-Moveed " + Math.Ceiling(Tracker.Count * .30) + " in " + ((DateTime.Now.Ticks - tStart) / TimeSpan.TicksPerMillisecond) + "ms");
                        break;
                    case "GetDocumentHistory":
                        GetDocumentHistory();
                        WriteOutput("Fetched RestDocument History of  " + Math.Ceiling(Tracker.Count * .10) 
                            + " Documents in " + ((DateTime.Now.Ticks - tStart) / TimeSpan.TicksPerMillisecond) + "ms");
                        break;
                    case "UploadRendition":
                        CreateRendition(0, false);
                        WriteOutput("Imported new Renditions of  " + Math.Ceiling(Tracker.Count * .30) 
                            + " Documents in " + ((DateTime.Now.Ticks - tStart) / TimeSpan.TicksPerMillisecond) + "ms");
                        break;
                    case "ViewRenditions":
                        // This may be used to get the text version of the email for the correspondence view
                        ViewRenditions(renditionsDirectoryName, IDsWithRenditions, openEachFile);
                        // Open a directory with the downloaded renditions to show the tester
                        if (showdownloadedfiles) System.Diagnostics.Process.Start(renditionsDirectoryName);
                        WriteOutput("Downloaded renditions of  " + Math.Ceiling(Tracker.Count * .30) + " Documents for view in " 
                            + ((DateTime.Now.Ticks - tStart) / TimeSpan.TicksPerMillisecond) + "ms");
                        break;
                    case "ImportAsNewVersion":
                        ImportAsNewVersion();
                        WriteOutput("New Versions of  " + Math.Ceiling(Tracker.Count * .20) + " Documents for created in " + ((DateTime.Now.Ticks - tStart) / TimeSpan.TicksPerMillisecond) + "ms");
                        break;
                    case "ReturnListOfDocuments":
                        ReturnListOfDocuments();
                        WriteOutput("List of Documents for 5 childFolderDocs returned in "
                            + ((DateTime.Now.Ticks - tStart) / TimeSpan.TicksPerMillisecond) + "ms");
                        break;
                    case "Search":
                        // This test does its own timing output
                        SearchForDocuments();
                        break;
                    case "ExportParent":
                        ExportParent();
                        break;
                    case "ExportListOfFiles":
                        ExportFiles();
                        break;
                    default:
                        WriteOutput("Invalid test name: [" + testName + "] ");
                        return false;
                }
                WriteOutput("-----FINISHED " + testName + "--------------");
                success = true;
            }
            catch (Exception e)
            {
                WriteOutput("#####FAILED##### TEST [" + testName + "]" + e.Message + "," + e.StackTrace.ToString());
            }
            return success;
        }

        protected List<String> IDsWithRenditions { get; set; }
        protected Repository CurrentRepository { get; set; }

        protected virtual Repository GetCurrentRepository(HomeDocument homeDoc, string repository)
        {
            return homeDoc.GetRepository<Repository>(repositoryName);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Start()
        {

            getPreferences();
            long testStart = DateTime.Now.Ticks;
            long tStart = DateTime.Now.Ticks;

            HomeDocument home = client.Get<HomeDocument>(RestHomeUri, null);
            if (home == null)
            {
                WriteOutput("\nUnable to get Rest Service at: " + RestHomeUri + " check to see if the service is available.");
                return;
            }
            home.SetClient(client);
            WriteOutput("Took " + ((DateTime.Now.Ticks - testStart)/TimeSpan.TicksPerMillisecond) + "ms to get RestService");
            CurrentRepository = GetCurrentRepository(home, repositoryName);

            ProductInfo productInfo = home.GetProductInfo();
            if (CurrentRepository == null) throw new Exception("Unable to login to the CurrentRepository, please see server logs for more details.");
            // Set our default folder and document types. 
            CurrentRepository.DocumentType = "dm_document";
            CurrentRepository.FolderType = "dm_folder";
            NameValueCollection restTests = ConfigurationManager.GetSection("resttests") as NameValueCollection;
            if (!(Boolean.Parse(restTests["CreateTempDocs"].ToString()) || Boolean.Parse(restTests["CreateTempDocs"].ToString())))
            {
                throw new System.Exception("On of the tests that create Documents is required for other tests to run. "
                    + "You must enable either the CreateTempDocs test and/or the CreateTempDocs test in order to create "
                    + "documents that can be used in subsequent tests.");
            }

            Tracker = new List<DocumentTracker>();
            foreach (String key in restTests)
            {
                bool preCheckOk = true;
                // This test is not available in versions earlier than 7.2
                double restVersion = Double.Parse((productInfo.Properties.Major.Equals("NA") ? "7.2" : productInfo.Properties.Major));

                if (key.Equals("Search")) {
                    if (!(restVersion >= 7.2d))
                    {
                        preCheckOk = false;
                        Console.WriteLine("Documentum Rest Version 7.2 or higher is required to use Search, your version is: " 
                            + restVersion + " Skipping...");
                    }
                }

                // These features do not work on Mono yet, should be fine when .NetCore is released though
                if(key.Equals("ExportParent") || key.Equals("ExportListOfFiles"))
                {
                    if (Type.GetType("Mono.Runtime") != null)
                    {
                        preCheckOk = false;
                        Console.WriteLine("The zip libraries required for [" + key + " ] have failed under Mono, skipping this  test. If you "
                            + "want to test for yourself, you will have to modify the source to allow it udner (UseCaseTests");
                    }
                }
                
                if(preCheckOk)
                {
                    if (Boolean.Parse(restTests[key].ToString())) runTestByName(key);
                }
                
            }

            WriteOutput("#####################################");
            WriteOutput("COMPLETED TESTS IN: " + ((DateTime.Now.Ticks - testStart) / TimeSpan.TicksPerMillisecond) / 1000 / 60 + "minutes");
            WriteOutput("#####################################");
            System.Diagnostics.Process.Start(testDirectory);

            if (loggerForm != null)
            {
                while (loggerForm.Visible)
                {
                    Application.DoEvents();
                }
            }
        }

        protected void ExportFiles()
        {
            FeedGetOptions options = new FeedGetOptions { ItemsPerPage = 10 };
            Feed<Document> feedDocs = CurrentRepository.ExecuteDQL<Document>("select r_object_id from dm_document where folder('" + ProcessBasePath + parentFolderId + "', DESCEND)", options);
            List<Document> docs = ObjectUtil.getFeedAsList<Document>(feedDocs, true);
            StringBuilder ids = new StringBuilder();
            int pass = 0;
            foreach(Document doc in docs) {
                string id = doc.GetPropertyValue("r_object_id").ToString();
                if (pass == 0)
                {
                    pass++;
                    ids.Append(id);
                } else {
                    ids.Append(",").Append(id);
                }
            }
            WriteOutput("[ExportFilesToZip] - Export list of files completed and stored: " + testDirectory + Path.DirectorySeparatorChar + "RandomDocsInParent.zip");
            CurrentRepository.ExportDocuments(ids.ToString(), testDirectory + Path.DirectorySeparatorChar + "RandomDocsInParent.zip");
        }

        protected void ExportParent()
        {
            string parentPath = ProcessBasePath + parentFolderId;
			FileInfo zipFile = CurrentRepository.ExportFolder(parentPath, testDirectory + Path.DirectorySeparatorChar + parentFolderId + ".zip");
			WriteOutput("[ExportFolderToZip] Export Folder completed and stored: " + testDirectory + Path.DirectorySeparatorChar + parentFolderId + ".zip");
        }

        protected void CreateFromTemplate()
        {
            
            Random rnd = new Random();
            //get list of templates
            Feed<Document> results = CurrentRepository.ExecuteDQL<Document>(String.Format("select * from dm_document where FOLDER('/Templates') "), new FeedGetOptions { Inline = true, Links = true });
            List<Document> docs = ObjectUtil.getFeedAsList<Document>(results, true);
            int resultSamples = docs.Count;

            WriteOutput(String.Format("\t[TemplateList] Returning list of templates..."));
            foreach (Document doc in docs)
            {
                WriteOutput(String.Format("\t\t\tTemplate Name: {0} ID: {1}",
                        doc.GetPropertyValue("object_name").ToString(),
                        doc.GetPropertyValue("r_object_id").ToString()));
            }
            List<string> req = childList;
            for (int i = 0; i < 10; i++)
            {
                DocumentTracker trackerDoc = Tracker[rnd.Next(0, Tracker.Count)];
                string movePath = trackerDoc.getPath();
                string childId = trackerDoc.ChildId;
                //select one of the documents
                Document template = docs[rnd.Next(0, resultSamples)];
                Document newDoc = CurrentRepository.CopyDocument(template.GetPropertyValue("r_object_id").ToString(), ProcessBasePath + movePath);

                newDoc.SetPropertyValue("subject", "Created From Template: " + template.GetPropertyValue("object_name"));
                string documentName = ObjectUtil.NewRandomDocumentName("FROMTEMPLATE");
                newDoc.SetPropertyValue("object_name", documentName);
                newDoc.Save();
                string objectId = newDoc.GetPropertyValue("r_object_id").ToString();
                //String childId = parentFolderId + " CHILD-" + new Random().Next(0, 5);
                Tracker.Add(new DocumentTracker(objectId, parentFolderId, childId));
                WriteOutput("\t[CreateDocumentFromTemplate] Created document " + documentName + " from template " + template.GetPropertyValue("object_name").ToString());
            }
        }

        protected void SearchForDocuments()
        {
            long tStart = DateTime.Now.Ticks;

            //WriteOutput("Pausing for ten seconds to make sure indexer has caught up for full text search test....");
            //Thread.Sleep(10000);
            SearchOptions search = new SearchOptions();
            search.SearchQuery = "document";
            search.Locations = ProcessBasePath + parentFolderId; // childList[new Random().Next(0, childList.Count)];

            search.ItemsPerPage = 20;
            //search.PageNumber = 1;
            int totalResults = 0;
            double totalPages = 0d;
            WriteOutput("[SearchResults] Return a list of documents using search criteria: " + search.SearchQuery + " Location: '" + search.Locations + "'");
            Feed<Document> feedResults = CurrentRepository.ExecuteSimpleSearch<Document>(search);
            if (feedResults != null)
            {
                totalResults = feedResults.Total;
                totalPages = feedResults.PageCount;
                int docProcessed = 0;

                for (int i = 0; i < totalPages; i++)
                {
                    foreach (Entry<Document> result in feedResults.Entries)
                    {
                        WriteOutput("\t[SearchResults] Search - RestDocument: " + result.Content.GetPropertyValue("object_name").ToString() + " Summary: " + result.Summary
                            + " Score: " + result.Score + " Terms: " + String.Join(",", result.Terms));
                        docProcessed++;
                    }

                    if (totalResults > docProcessed) feedResults = feedResults.NextPage();
                    WriteOutput("\t*****************************************************");
                    WriteOutput("Page:" + (i + 1) + " Results: " + docProcessed + " out of " + totalResults + " Processed");
                    WriteOutput("\t*****************************************************");
                    WriteOutput("\n\n");
                }

            } else
            {
                throw new Exception("SearchResults are NULL so an error occurred. Check the log for errors.");
            }
            WriteOutput("[SearchResults] Result Count: " + totalResults + " Pages: " + totalPages + " Processed in " + ((DateTime.Now.Ticks - tStart) / TimeSpan.TicksPerMillisecond) + "ms");
        }


        protected void ReturnListOfDocuments()
        {
            Random rnd = new Random();
            // Return a list of documents in a parentFolder
            int resultSamples = Tracker.Count < 5? Tracker.Count : 5;
            for(int i=0; i<resultSamples;i++) {
                string path = ProcessBasePath + Tracker[rnd.Next(0,resultSamples)];
                Feed<Document> results = CurrentRepository.ExecuteDQL<Document>(
                    String.Format("select * from dm_document where folder('{0}', DESCEND)", path), new FeedGetOptions { Inline=true, Links=true });
                List<Document> docs = ObjectUtil.getFeedAsList<Document>(results, true);
                WriteOutput(String.Format("\t\t[ReturnListOfDocumentsFromQuery] Returning list of documents in path [{0}]", path));
                foreach (Document doc in docs)
                {
                    WriteOutput(String.Format("\t\t\tName: {0} ID: {1}", 
                        doc.GetPropertyValue("object_name").ToString(), 
                        doc.GetPropertyValue("r_object_id").ToString()));
                }
            }
            
        }

        protected void ImportAsNewVersion()
        {
            List<DocumentTracker> myList = new List<DocumentTracker>(Tracker);
            WriteOutput(("\t[ImportAsNewVersion] Importing new Content to existing objects as New Versions"));
            for (int i = 0; i < (Math.Ceiling(myList.Count * .20)); i++)
            {
                DocumentTracker aDoc = myList[i];
                myList.Remove(aDoc);

                Document doc = CurrentRepository.GetSysObjectById<Document>(myList[i].DocumentId); 
                //RestDocument doc = CurrentRepository.getDocumentByQualification(String.Format("dm_document where r_object_id = '{0}'",
                //    myList[i].DocumentId), new FeedGetOptions { Inline = true, Links = true });
                Feed<OutlineAtomContent> versions = doc.GetAllVersions<OutlineAtomContent>(null);
                List <Entry<OutlineAtomContent>> entries = versions.Entries;
                WriteOutput("\t\tCurrentDocumentVersion: " + doc.GetRepeatingValuesAsString("r_version_label", ",") + " ID: " + doc.GetPropertyValue("r_object_id").ToString());
                WriteOutput("\t\tVersion Count Prior to Importing New Version:" + entries.Count);
                if(!doc.IsCheckedOut()) doc = doc.Checkout();// Handles when you unwind during debugging and doc was checked out before.
                if (doc.IsCheckedOut())
                {
                    WriteOutput("\t\t[CheckOut] - Checked out document...");
                    doc = doc.CancelCheckout();
                    if(!doc.IsCheckedOut()) {
                        WriteOutput("\t\t[CancelCheckOut] - Canceled Checkout....");
                        doc = doc.Checkout();
                        if (doc.IsCheckedOut())
                        {
                            WriteOutput("\t\t[CheckOut] - Checked out document after cancel checkout..., document is checked out by: " +doc.GetCheckedOutBy());        
                        } else {
                            WriteOutput("\t\t[CheckOut] - #####FAILED##### CHECK OUT DOCUMENT");
                        }
                    }
                }
                else
                {
                    WriteOutput("\t\t[CheckOut] - #####FAILED##### CHECK OUT DOCUMENT");
                }
                FileInfo file = ObjectUtil.getRandomFileFromDirectory(importFilesDirectory);
                doc = CurrentRepository.ImportDocumentAsNewVersion(doc, file);
                WriteOutput("\t [ImportAsNewVersion] Import RestDocument as New Version");

                if (doc.IsCheckedOut())
                {
                    WriteOutput("\t\t[CheckIn] - #####FAILED##### CHECK IN DOCUMENT");
                }
                else
                {
                    WriteOutput("\t\t[CheckIn] - RestDocument Checked IN...");
                }

                Feed<OutlineAtomContent> newVersions = doc.GetAllVersions<OutlineAtomContent>(null);
                List <Entry<OutlineAtomContent>> newEntries = newVersions.Entries;
                WriteOutput("\t\tNew Version Count: " + newEntries.Count);
                WriteOutput("\t\tNewDocumentVersion: " + doc.GetRepeatingValuesAsString("r_version_label", ",") + " ID: " + doc.GetPropertyValue("r_object_id").ToString());
                WriteOutput("\t\t[ListVersions] - List of document versions:");
                WriteOutput("Versions:");
                Feed<Document> allVersions = doc.GetAllVersions<Document>(new FeedGetOptions { Inline = true, View = ":all" });
                foreach (Entry<Document> vDoc in allVersions.Entries)
                {
                    WriteOutput(String.Format("\t\t\t ChronicleID: {0} ObjectID: {1} VersionLabel: {2}",
                        doc.GetPropertyValue("i_chronicle_id").ToString(),
                        vDoc.Content.GetPropertyValue("r_object_id").ToString(),
                        vDoc.Content.GetRepeatingValuesAsString("r_version_label", ",")));
                }
            }     
        }

        public void ViewDocument(String path, DocumentTracker tracker, bool openDocument)
        {

            Document doc = CurrentRepository.GetSysObjectById<Document>(tracker.DocumentId);

            ContentMeta contentMeta = doc.GetPrimaryContent(new SingleGetOptions { View = ":all"});
            if (contentMeta == null)
            {
                WriteOutput("!!!!!!!!!!!!!!!!VIEW TEST FAILURE!!!!!!!!!!!!!!!!!!!!");
                return;
            }
            FileInfo downloadedContentFile = contentMeta.DownloadContentMediaFile();
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
			downloadedContentFile.MoveTo(path + Path.DirectorySeparatorChar + tracker.ChildId + "-" + downloadedContentFile.Name);
            WriteOutput("\t\t[GetFileForView] - RestDocument file is located: " + downloadedContentFile.FullName);
            if (openDocument) System.Diagnostics.Process.Start(downloadedContentFile.FullName);
        }

        protected void GetDocumentHistory()
        {
            List<DocumentTracker> newList = new List<DocumentTracker>(Tracker);

            double moveCount = Math.Ceiling(newList.Count * .10);
            for (int a = 0; a < moveCount; a++)
            {
                DocumentTracker aDoc = ObjectUtil.getRandomObjectFromList<DocumentTracker>(newList);
                // Make sure we do not use this again
                newList.Remove(aDoc);
                Document doc = CurrentRepository.GetSysObjectById<Document>(aDoc.DocumentId); //CurrentRepository.getDocumentByQualification(
                    //String.Format("dm_document(all) where r_object_id = '{0}'",aDoc.DocumentId), null);
                WriteOutput("\t" + aDoc.DocumentId + ":" + doc.GetPropertyValue("object_name").ToString() + " - RestDocument History:");
                Feed<AuditEntry> auditInfo = CurrentRepository.GetAuditHistory(HistoryType.THISDOCUMENTONLY, doc);
                List<AuditEntry> entries = ObjectUtil.getFeedAsList(auditInfo,true);
                //List<Entry<AuditEntry>> entries = (List<Entry<AuditEntry>>)auditInfo.Entries;
                foreach (AuditEntry history in entries)
                {
                    WriteOutput("\t\tEvent:" + history.getEventName() + " Description:" + history.getEventDescription()
                        + " ObjectName:" + history.getObjectName() + " Time:" + history.getTimeStamp());
                }
                WriteOutput("\t\t[AuditHistory] - RestDocument history pulled from the Audit tables");
            }
        }

        /// <summary>
        /// Move 5 percent of the (no less than 1) documents from child to parentFolder level (Shows Re-Filing Documents)
        /// </summary>
        /// <param name="tracker"></param>
        protected void MoveChildFolderDocumentsToParent()
        {
            List<DocumentTracker> newList = new List<DocumentTracker>(Tracker);
            double moveCount = Math.Ceiling(newList.Count * .30);
            for (int a = 0; a < moveCount; a++)
            {
                DocumentTracker aDoc = ObjectUtil.getRandomObjectFromList<DocumentTracker>(newList);
                // Make sure we do not use this again
                newList.Remove(aDoc);
                String currentPath = ProcessBasePath + aDoc.getPath();
                Document docToMove = CurrentRepository.GetSysObjectById<Document>(aDoc.DocumentId);
                List<String> childPathAndFolder = ObjectUtil.getPathAndFolderNameFromPath(currentPath);
                String parentFolderPath = childPathAndFolder[0];
                String childFolderName = childPathAndFolder[1];
                Folder childFolder = CurrentRepository.GetFolderByQualification(
                    String.Format("dm_folder where r_object_id = '{0}'", 
                    docToMove.GetRepeatingValue("i_folder_id", 0)), new FeedGetOptions { Inline = true, Links = true });
                List<String> parentPathAndFolder = ObjectUtil.getPathAndFolderNameFromPath(parentFolderPath);
                String folderPath = parentPathAndFolder[0];
                String parentFolderName = parentPathAndFolder[1];
                Folder parentFolder = CurrentRepository.GetFolderByQualification(
                    String.Format("dm_folder where folder('{0}') and object_name='{1}'", folderPath,
                    parentFolderName), new FeedGetOptions { Inline = true, Links = true });
                CurrentRepository.MoveDocument(docToMove, childFolder, parentFolder);
                WriteOutput("\t\t[MoveDocument] - RestDocument removed from " + currentPath + " to " + parentFolderPath);
            }
        }

        /// <summary>
        /// CreateTempDocs and tracker, together, show how one might create documents in a temporary location, then move/move
        /// them to another location later. This is handly for the Tester application to be able to choose documents at random
        /// for moving/linking and choosing a percentage of documents for performaing actions on.
        /// Use Cases  [Import New RestDocument], [Copy/Move RestDocument], [DeleteDocument]
        ///             [Folder Structure]
        /// </summary>
        /// <returns></returns>
        protected void CreateTempDocs()
        {
            tempPath = tempPath + "-" + threadNum;

            WriteOutput("[CreateOrGetFolderPath] - Creating or getting folder by path: " + tempPath);
            Folder tempFolder = CurrentRepository.GetOrCreateFolderByPath(tempPath);
            WriteOutput("\tFolder: " + tempFolder.GetPropertyValue("object_name") + ":" 
                + tempFolder.GetPropertyValue("r_object_id") + " successfully created!");
            WriteOutput("\tCreating " + numDocs + " random documents.");
            string previousChildId = null;
            for (int i = 0; i < numDocs; i++)
            {
                FileInfo file = ObjectUtil.getRandomFileFromDirectory(importFilesDirectory);
                
                Document tempDoc = CurrentRepository.ImportNewDocument(file, testPrefix + "-" + file.Name, tempPath);
                WriteOutput("\t\t[ImportDocument] - RestDocument " + file.FullName + " imported as " 
                    + tempDoc.GetPropertyValue("object_name") + " ObjectID: " 
                    + tempDoc.GetPropertyValue("r_object_id").ToString());
                WriteOutput("\t\t\t[DeDuplication] - Performing Duplicate Detection on content in holding area....");
                CheckDuplicates(tempDoc, tempPath);

                // Cannot randomly move parentFolders as threads will step on each other. Limit one thread to one 
                // parentFolder 
                
                String childId = parentFolderId + " CHILD-" + new Random().Next(0,5);
                String objectId = (String)tempDoc.GetPropertyValue("r_object_id");
                WriteOutput("[CreateAndtrackerDocument] \t\tCreated " + tempDoc.GetPropertyValue("object_name") + ":" + objectId + " Moveing to Parent: " 
                    + parentFolderId + " Child: " + childId);
                WriteOutput("[ChangeExiFetchDocument] - ReFetching and Setting title attribute");
                Document doc = tempDoc.Fetch<Document>();
                doc.SetPropertyValue("title", "Set properties test");
                doc.Save();
                Tracker.Add(new DocumentTracker(objectId, parentFolderId, childId));
                // To show we can move the same document to multiple childFolderDocs
                if (previousChildId != null && !previousChildId.Equals(childId))
                {
                    WriteOutput("\t\t\tMoveing this document to another child to show the same document can be copied/moveed to multiple childFolderDocs");
                    Tracker.Add(new DocumentTracker(objectId, parentFolderId, previousChildId));
                }
                previousChildId = childId;
            } // Done with temp creation loop
        }

        protected void CheckDuplicates(Document doc, string path)
        {
            List<PersistentObject> dupes = CurrentRepository.CheckForDuplicate((String)doc.GetPropertyValue("r_object_id"), path);
            StringBuilder dupeList = new StringBuilder();
            if (dupes.Count != 0)
            {
                if (printResult)
                {
                    bool first = true;
                    WriteOutput("\t\t\tDocument: " + doc.GetPropertyValue("object_name") + ":" + doc.GetPropertyValue("r_object_id"));
                    foreach (PersistentObject pObj in dupes)
                    {
                        WriteOutput(String.Format("DUPLICATE OF: {0}", pObj.GetRepeatingValuesAsString("parent_id",",").ToString()));
                        if (first)
                        {
                            dupeList.Append("'" + pObj.GetPropertyValue("parent_id") + "'");
                        }
                        else
                        {
                            dupeList.Append(",'" + pObj.GetPropertyValue("parent_id") + "'");
                        }
                    }
                    
                }

                if (path == null)
                {
                    WriteOutput("\t\t\t[DeDuplication] - " + dupes.Count + " duplicates were identified in the SYSTEM. Choosing to allow");
                }
                else
                {
                    WriteOutput("\t\t\t[DeDuplication] - " + dupes.Count + " duplicates were identified in the destination FOLDER, Choosing to allow.");
                }

            }
            else
            {
                WriteOutput("\t\t\t[DeDuplication] - No Duplicates of this document found.");
            }
        }

        /// <summary>
        /// Called by CreateTempDocs in order to create parent/child folders and randomly move documents to parent/child folders.
        /// </summary>
        /// <param name="tracker"></param>
        protected void MoveToFolders ()
        {
            //WriteOutput("Getting the holding folder for documents prior to Parent/Child movement...");
            Folder tempFolder = CurrentRepository.GetFolderByQualification("dm_folder where any r_folder_path = '" 
                + tempPath + "'", new FeedGetOptions{ Inline = true, Links = true });
            WriteOutput("\tMoveing Documents from folder: " + tempFolder.GetPropertyValue("object_name"));
            foreach (DocumentTracker trackerDoc in Tracker)
            {
                String parentFolderId = trackerDoc.ParentId;
                String childId = trackerDoc.ChildId;
                //WriteOutput("Getting the Parent/Child movement folder...");
                String movePath = ProcessBasePath + trackerDoc.getPath();
                // Our parentFolder/child tracker for doing record declaration later
                addSupportingDoc(movePath);
                Folder destinationDir = CurrentRepository.GetOrCreateFolderByPath(movePath);
                Document docToCopy = CurrentRepository.GetSysObjectById<Document>(trackerDoc.DocumentId); // getDocumentByQualification("dm_document where r_object_id = '"
                     //+ trackerDoc.DocumentId + "'", new FeedGetOptions { Inline = true, Links = true });
                // To copy the document, we need to get a reference object
                CheckDuplicates(docToCopy, ProcessBasePath + trackerDoc.getPath());
                Document copiedDoc = destinationDir.CreateSubObject<Document>(docToCopy.CreateCopyObject<Document>(), null);
                WriteOutput("\t[CopyDocument] - Moveed RestDocument: " + copiedDoc.GetPropertyValue("object_name") + " ID:" 
                    + trackerDoc.DocumentId + " to " + ProcessBasePath + trackerDoc.getPath());
                // Update the trackerDocumentId to the newly copied document
                trackerDoc.DocumentId = copiedDoc.GetPropertyValue("r_object_id").ToString();
            }

            // Delete our temp folder
            GenericOptions options = new GenericOptions();
            options.SetQuery("del-non-empty", true);
            options.SetQuery("del-all-links", true);
            tempFolder.Delete(options);
            WriteOutput("[DeleteFolderAndContents] - Deleted the holding folder and documents");
        }

        protected void CreateRendition(int page, bool isPrimary)
        {
            List<string> idsWithRenditions = new List<string>();

            List<DocumentTracker> newList = new List<DocumentTracker>(Tracker);

            double moveCount = Math.Ceiling(newList.Count * .30);
            for (int a = 0; a < moveCount; a++)
            {
                DocumentTracker aDoc = ObjectUtil.getRandomObjectFromList<DocumentTracker>(newList);
                // Make sure we do not use this again
                newList.Remove(aDoc);
                String objectId = aDoc.DocumentId;
                Document doc = CurrentRepository.GetSysObjectById<Document>(objectId); //getDocumentByQualification(
                //String.Format("dm_document where r_object_id = '{0}'", objectId), null);

                FileInfo file = ObjectUtil.getRandomFileFromDirectory(importFilesDirectory);
                String mimeType = ObjectUtil.getMimeTypeFromFileName(file.Name);

                // Upload the content as a new rendition
                GenericOptions rendOptions = new GenericOptions();
                String format = ObjectUtil.getDocumentumFormatForFile(file.Extension);
                rendOptions.SetQuery("format", format);
                rendOptions.SetQuery("page", page);

                // If you want to allow multiple renditions of the same format, the modifier must be set, this makes the rendition unique in the list
                // the "modifier" is more like a label/tag for the rendition in the list.
                rendOptions.SetQuery("modifier", "Test");
                // With primary false, will be added as a rendition
                rendOptions.SetQuery("primary", isPrimary); 

                ContentMeta renditionMeta = doc.CreateContent(file.OpenRead(), mimeType, rendOptions);
                Feed<ContentMeta> contents = doc.GetContents<ContentMeta>(new FeedGetOptions { Inline = true });
                List<Entry<ContentMeta>> entries = (List<Entry<ContentMeta>>)contents.Entries;
                WriteOutput("\t\t[AddRendition] - Rendition Added for RestDocument ID: " 
                    + doc.GetPropertyValue("r_object_id") + ":" 
                    + doc.GetPropertyValue("object_name"));
                foreach (Entry<ContentMeta> entry in entries)
                {
                    ContentMeta rendition = entry.Content;
                    WriteOutput("\t\t\tRendition Format: " + rendition.GetPropertyValue("full_format")
                        + " Modifier: " + rendition.GetRepeatingString("page_modifier", 0)); //((Object[])rendition.getAttributeValue("page_modifier"))[0].ToString());
                }
                idsWithRenditions.Add(objectId);
            }
            IDsWithRenditions = idsWithRenditions;
        }


        protected void ViewRenditions(string renditionDirectory, List<string> idsWithRenditions, bool openDocument)
        {
            int renditionNumber = 0;
            foreach (string objectId in idsWithRenditions)
            {
                renditionNumber++;
                Document doc = CurrentRepository.GetSysObjectById<Document>(objectId); //.getDocumentByQualification(
                    //String.Format("dm_document where r_object_id = '{0}'", objectId), null);

                ContentMeta renditionMeta = doc.GetRenditionByModifier("Test");
                if (renditionMeta == null)
                {
                    WriteOutput("!!!!!!!!!!!!!!!!RENDITION VIEW TEST FAILURE!!!!!!!!!!!!!!!!!!!!");
                    return;
                }
                FileInfo downloadedContentFile = renditionMeta.DownloadContentMediaFile();
                if (!Directory.Exists(renditionDirectory))
                {
                    Directory.CreateDirectory(renditionDirectory);
                }
				downloadedContentFile.MoveTo(renditionDirectory + Path.DirectorySeparatorChar + objectId + "-"+renditionNumber + "-" + downloadedContentFile.Name);
                WriteOutput("\t\t[ViewRendition] - Rendition file is located: " + downloadedContentFile.FullName);
                if(openDocument) System.Diagnostics.Process.Start(downloadedContentFile.FullName);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    loggerForm.Dispose();
                }
            }
            _disposed = true;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}