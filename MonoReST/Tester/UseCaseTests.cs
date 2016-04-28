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
        protected string testDirectory = @"C:\Temp\";
        protected string randomEmailsDirectory = @"Test\emails";
        protected string randomFilesDirectory = @"Test";
        protected string renditionsDirectory = @"Renditions";
        protected string primaryContentDirectory = @"PrimaryContent";
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

                testDirectory = testConfig["testbasedirectory"];
                if (!testDirectory.EndsWith(Path.DirectorySeparatorChar + "")) testDirectory = testDirectory + Path.DirectorySeparatorChar + "";
                testDirectory = testDirectory + testPrefix;
                testDirectory = getPathRelativeToExecutable(testDirectory);
				Directory.CreateDirectory(testDirectory);
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
                randomFilesDirectory = testConfig["randomfilesdirectory"].ToString();
                randomFilesDirectory = getPathRelativeToExecutable(randomFilesDirectory);

                randomEmailsDirectory = testConfig["randomemailsdirectory"].ToString();
                randomEmailsDirectory = getPathRelativeToExecutable(randomEmailsDirectory);

				renditionsDirectory = testDirectory + Path.DirectorySeparatorChar + "Renditions";
                primaryContentDirectory = testDirectory + Path.DirectorySeparatorChar + "PrimaryContent";
                if (!Directory.Exists(randomFilesDirectory))
                {
                    String msg = "Unable to find the directory specified: " + randomFilesDirectory + " to pull "
                        + "random content files from. Unable to proceed. Please set this value  to a directory with "
                        + "some files in it in the application configuration file";
                    WriteOutput(msg);
                    throw new Exception(msg);
                }
                if (!Directory.Exists(randomEmailsDirectory))
                {
                    String msg = "Unable to find the directory specified: " + randomEmailsDirectory + " to pull"
                        + " random email files from. If customized Rest email processing is installed, the"
                        + " customized email test will fail. This message can normally be ignored unless you"
                        + " have the custom Rest email adapter installed.";
                    WriteOutput(msg);
                }
                Directory.CreateDirectory(primaryContentDirectory);
                Directory.CreateDirectory(renditionsDirectory);
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
                            ViewDocument(primaryContentDirectory, Tracker[p], openEachFile);
                        }
                        if (showdownloadedfiles) System.Diagnostics.Process.Start(primaryContentDirectory);
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
                        ViewRenditions(renditionsDirectory, IDsWithRenditions, openEachFile);
                        // Open a directory with the downloaded renditions to show the tester
                        if (showdownloadedfiles) System.Diagnostics.Process.Start(renditionsDirectory);
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
        /// <summary>
        /// 
        /// </summary>
        public void Start()
        {

                getPreferences();
                long testStart = DateTime.Now.Ticks;
                long tStart = DateTime.Now.Ticks;

                RestService home = client.Get<RestService>(RestHomeUri, null);
                if (home == null)
                {
                    WriteOutput("\nUnable to get Rest Service at: " + RestHomeUri + " check to see if the service is available.");
                    return;
                }
                home.SetClient(client);
                WriteOutput("Took " + ((DateTime.Now.Ticks - testStart)/TimeSpan.TicksPerMillisecond) + "ms to get RestService");
            //Feed<Repository> repositories = home.GetRepositories<Repository>(new FeedGetOptions { Inline = true, Links = true });
            //Repository CurrentRepository = repositories.GetRepository(repositoryName);
            CurrentRepository = home.GetRepository(repositoryName);
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
            Feed<RestDocument> feedDocs = CurrentRepository.ExecuteDQL<RestDocument>("select r_object_id from dm_document where folder('" + ProcessBasePath + parentFolderId + "', DESCEND)", options);
            List<RestDocument> docs = ObjectUtil.getFeedAsList<RestDocument>(feedDocs, true);
            StringBuilder ids = new StringBuilder();
            int pass = 0;
            foreach(RestDocument doc in docs) {
                string id = doc.getAttributeValue("r_object_id").ToString();
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
            Feed<RestDocument> results = CurrentRepository.ExecuteDQL<RestDocument>(String.Format("select * from dm_document where FOLDER('/Templates') "), new FeedGetOptions { Inline = true, Links = true });
            List<RestDocument> docs = ObjectUtil.getFeedAsList<RestDocument>(results, true);
            int resultSamples = docs.Count;

            WriteOutput(String.Format("\t[TemplateList] Returning list of templates..."));
            foreach (RestDocument doc in docs)
            {
                WriteOutput(String.Format("\t\t\tTemplate Name: {0} ID: {1}",
                        doc.getAttributeValue("object_name").ToString(),
                        doc.getAttributeValue("r_object_id").ToString()));
            }
            List<string> req = childList;
            for (int i = 0; i < 10; i++)
            {
                DocumentTracker trackerDoc = Tracker[rnd.Next(0, Tracker.Count)];
                string movePath = trackerDoc.getPath();
                string childId = trackerDoc.ChildId;
                //select one of the documents
                RestDocument template = docs[rnd.Next(0, resultSamples)];
                RestDocument newDoc = CurrentRepository.copyDocument(template.getAttributeValue("r_object_id").ToString(), ProcessBasePath + movePath);

                newDoc.setAttributeValue("subject", "Created From Template: " + template.getAttributeValue("object_name"));
                string documentName = ObjectUtil.NewRandomDocumentName("FROMTEMPLATE");
                newDoc.setAttributeValue("object_name", documentName);
                newDoc.Save();
                string objectId = newDoc.getAttributeValue("r_object_id").ToString();
                //String childId = parentFolderId + " CHILD-" + new Random().Next(0, 5);
                Tracker.Add(new DocumentTracker(objectId, parentFolderId, childId));
                WriteOutput("\t[CreateDocumentFromTemplate] Created document " + documentName + " from template " + template.getAttributeValue("object_name").ToString());
            }
        }

        protected void SearchForDocuments()
        {
            long tStart = DateTime.Now.Ticks;

            //WriteOutput("Pausing for ten seconds to make sure indexer has caught up for full text search test....");
            //Thread.Sleep(10000);
            Search search = new Search();
            search.Query = "document";
            search.Locations = ProcessBasePath + parentFolderId; // childList[new Random().Next(0, childList.Count)];

            search.ItemsPerPage = 20;
            //search.PageNumber = 1;
            int totalResults = 0;
            double totalPages = 0d;
            WriteOutput("[SearchResults] Return a list of documents using search criteria: " + search.Query + " Location: '" + search.Locations + "'");
            SearchFeed<RestDocument> feedResults = CurrentRepository.ExecuteSearch<RestDocument>(search);
            if (feedResults != null)
            {
                totalResults = feedResults.Total;
                totalPages = feedResults.PageCount;
                int docProcessed = 0;

                for (int i = 0; i < totalPages; i++)
                {
                    foreach (SearchEntry<RestDocument> result in feedResults.Entries)
                    {
                        WriteOutput("\t[SearchResults] Search - RestDocument: " + result.Content.getAttributeValue("object_name").ToString() + " Summary: " + result.Summary
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
                Feed<RestDocument> results = CurrentRepository.ExecuteDQL<RestDocument>(
                    String.Format("select * from dm_document where folder('{0}', DESCEND)", path), new FeedGetOptions { Inline=true, Links=true });
                List<RestDocument> docs = ObjectUtil.getFeedAsList<RestDocument>(results, true);
                WriteOutput(String.Format("\t\t[ReturnListOfDocumentsFromQuery] Returning list of documents in path [{0}]", path));
                foreach (RestDocument doc in docs)
                {
                    WriteOutput(String.Format("\t\t\tName: {0} ID: {1}", 
                        doc.getAttributeValue("object_name").ToString(), 
                        doc.getAttributeValue("r_object_id").ToString()));
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

                RestDocument doc = CurrentRepository.getObjectById<RestDocument>(myList[i].DocumentId); 
                //RestDocument doc = CurrentRepository.getDocumentByQualification(String.Format("dm_document where r_object_id = '{0}'",
                //    myList[i].DocumentId), new FeedGetOptions { Inline = true, Links = true });
                Feed<OutlineAtomContent> versions = doc.GetVersionHistory<OutlineAtomContent>(null);
                List <Entry<OutlineAtomContent>> entries = versions.Entries;
                WriteOutput("\t\tCurrentDocumentVersion: " + doc.getRepeatingValuesAsString("r_version_label", ",") + " ID: " + doc.getAttributeValue("r_object_id").ToString());
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
                            WriteOutput("\t\t[CheckOut] - Checked out document after cancel checkout..., document is checked out by: " +doc.getLockOwner());        
                        } else {
                            WriteOutput("\t\t[CheckOut] - #####FAILED##### CHECK OUT DOCUMENT");
                        }
                    }
                }
                else
                {
                    WriteOutput("\t\t[CheckOut] - #####FAILED##### CHECK OUT DOCUMENT");
                }
                FileInfo file = ObjectUtil.getRandomFileFromDirectory(randomFilesDirectory);
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

                Feed<OutlineAtomContent> newVersions = doc.GetVersionHistory<OutlineAtomContent>(null);
                List <Entry<OutlineAtomContent>> newEntries = newVersions.Entries;
                WriteOutput("\t\tNew Version Count: " + newEntries.Count);
                WriteOutput("\t\tNewDocumentVersion: " + doc.getRepeatingValuesAsString("r_version_label", ",") + " ID: " + doc.getAttributeValue("r_object_id").ToString());
                WriteOutput("\t\t[ListVersions] - List of document versions:");
                WriteOutput("Versions:");
                List<RestDocument> allVersions = CurrentRepository.getAllDocumentVersions(doc);
                foreach (RestDocument vDoc in allVersions)
                {
                    WriteOutput(String.Format("\t\t\t ChronicleID: {0} ObjectID: {1} VersionLabel: {2}",
                        doc.getAttributeValue("i_chronicle_id").ToString(),
                        vDoc.getAttributeValue("r_object_id").ToString(),
                        vDoc.getRepeatingValuesAsString("r_version_label", ",")));
                }
            }
            
        }

        public void ViewDocument(String path, DocumentTracker tracker, bool openDocument)
        {

            RestDocument doc = CurrentRepository.getObjectById<RestDocument>(tracker.DocumentId);

            ContentMeta contentMeta = doc.getContent();
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
                RestDocument doc = CurrentRepository.getObjectById<RestDocument>(aDoc.DocumentId); //CurrentRepository.getDocumentByQualification(
                    //String.Format("dm_document(all) where r_object_id = '{0}'",aDoc.DocumentId), null);
                WriteOutput("\t" + aDoc.DocumentId + ":" + doc.getAttributeValue("object_name").ToString() + " - RestDocument History:");
                Feed<AuditEntry> auditInfo = CurrentRepository.getDocumentHistory(HistoryType.THISDOCUMENTONLY, doc);
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
                RestDocument docToMove = CurrentRepository.getObjectById<RestDocument>(aDoc.DocumentId);
                List<String> childPathAndFolder = ObjectUtil.getPathAndFolderNameFromPath(currentPath);
                String parentFolderPath = childPathAndFolder[0];
                String childFolderName = childPathAndFolder[1];
                Folder childFolder = CurrentRepository.getFolderByQualification(
                    String.Format("dm_folder where r_object_id = '{0}'", 
                    docToMove.getRepeatingValue("i_folder_id", 0)), new FeedGetOptions { Inline = true, Links = true });
                List<String> parentPathAndFolder = ObjectUtil.getPathAndFolderNameFromPath(parentFolderPath);
                String folderPath = parentPathAndFolder[0];
                String parentFolderName = parentPathAndFolder[1];
                Folder parentFolder = CurrentRepository.getFolderByQualification(
                    String.Format("dm_folder where folder('{0}') and object_name='{1}'", folderPath,
                    parentFolderName), new FeedGetOptions { Inline = true, Links = true });
                CurrentRepository.moveDocument(docToMove, childFolder, parentFolder);
                WriteOutput("\t\t[MoveDocument] - RestDocument removeed from " + currentPath + " to " + parentFolderPath);
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
            Folder tempFolder = CurrentRepository.getOrCreateFolderByPath(tempPath);
            WriteOutput("\tFolder: " + tempFolder.getAttributeValue("object_name") + ":" 
                + tempFolder.getAttributeValue("r_object_id") + " successfully created!");
            WriteOutput("\tCreating " + numDocs + " random documents.");
            string previousChildId = null;
            for (int i = 0; i < numDocs; i++)
            {
                FileInfo file = ObjectUtil.getRandomFileFromDirectory(randomFilesDirectory);
                
                RestDocument tempDoc = CurrentRepository.ImportNewDocument(file, testPrefix + "-" + file.Name, tempPath);
                WriteOutput("\t\t[ImportDocument] - RestDocument " + file.FullName + " imported as " 
                    + tempDoc.getAttributeValue("object_name") + " ObjectID: " 
                    + tempDoc.getAttributeValue("r_object_id").ToString());
                WriteOutput("\t\t\t[DeDuplication] - Performing Duplicate Detection on content in holding area....");
                CheckDuplicates(tempDoc, tempPath);

                // Cannot randomly move parentFolders as threads will step on each other. Limit one thread to one 
                // parentFolder 
                
                String childId = parentFolderId + " CHILD-" + new Random().Next(0,5);
                String objectId = (String)tempDoc.getAttributeValue("r_object_id");
                WriteOutput("[CreateAndtrackerDocument] \t\tCreated " + tempDoc.getAttributeValue("object_name") + ":" + objectId + " Moveing to Parent: " 
                    + parentFolderId + " Child: " + childId);
                WriteOutput("[ChangeExistingDocument] - ReFetching and Setting title attribute");
                RestDocument doc = tempDoc.fetch<RestDocument>();
                doc.setAttributeValue("title", "Set properties test");
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

        protected void CheckDuplicates(RestDocument doc, string path)
        {
            List<PersistentObject> dupes = CurrentRepository.CheckForDuplicate((String)doc.getAttributeValue("r_object_id"), path);
            StringBuilder dupeList = new StringBuilder();
            if (dupes.Count != 0)
            {
                if (printResult)
                {
                    bool first = true;
                    WriteOutput("\t\t\tDocument: " + doc.getAttributeValue("object_name") + ":" + doc.getAttributeValue("r_object_id"));
                    foreach (PersistentObject pObj in dupes)
                    {
                        WriteOutput(String.Format("DUPLICATE OF: {0}", pObj.getRepeatingValuesAsString("parent_id",",").ToString()));
                        if (first)
                        {
                            dupeList.Append("'" + pObj.getAttributeValue("parent_id") + "'");
                        }
                        else
                        {
                            dupeList.Append(",'" + pObj.getAttributeValue("parent_id") + "'");
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
            Folder tempFolder = CurrentRepository.getFolderByQualification("dm_folder where any r_folder_path = '" 
                + tempPath + "'", new FeedGetOptions{ Inline = true, Links = true });
            WriteOutput("\tMoveing Documents from folder: " + tempFolder.getAttributeValue("object_name"));
            foreach (DocumentTracker trackerDoc in Tracker)
            {
                String parentFolderId = trackerDoc.ParentId;
                String childId = trackerDoc.ChildId;
                //WriteOutput("Getting the Parent/Child movement folder...");
                String movePath = ProcessBasePath + trackerDoc.getPath();
                // Our parentFolder/child tracker for doing record declaration later
                addSupportingDoc(movePath);
                Folder destinationDir = CurrentRepository.getOrCreateFolderByPath(movePath);
                RestDocument docToCopy = CurrentRepository.getObjectById<RestDocument>(trackerDoc.DocumentId); // getDocumentByQualification("dm_document where r_object_id = '"
                     //+ trackerDoc.DocumentId + "'", new FeedGetOptions { Inline = true, Links = true });
                // To copy the document, we need to get a reference object
                CheckDuplicates(docToCopy, ProcessBasePath + trackerDoc.getPath());
                RestDocument copiedDoc = destinationDir.CreateSubObject<RestDocument>(docToCopy.GetCopy<RestDocument>(), null);
                WriteOutput("\t[CopyDocument] - Moveed RestDocument: " + copiedDoc.getAttributeValue("object_name") + " ID:" 
                    + trackerDoc.DocumentId + " to " + ProcessBasePath + trackerDoc.getPath());
                // Update the trackerDocumentId to the newly copied document
                trackerDoc.DocumentId = copiedDoc.getAttributeValue("r_object_id").ToString();
            }

            // Delete our temp folder
            CurrentRepository.deleteFolder(tempFolder, true, true);
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
                RestDocument doc = CurrentRepository.getObjectById<RestDocument>(objectId); //getDocumentByQualification(
                //String.Format("dm_document where r_object_id = '{0}'", objectId), null);

                FileInfo file = ObjectUtil.getRandomFileFromDirectory(randomFilesDirectory);
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
                    + doc.getAttributeValue("r_object_id") + ":" 
                    + doc.getAttributeValue("object_name"));
                foreach (Entry<ContentMeta> entry in entries)
                {
                    ContentMeta rendition = entry.Content;
                    WriteOutput("\t\t\tRendition Format: " + rendition.getAttributeValue("full_format")
                        + " Modifier: " + rendition.getRepeatingString("page_modifier", 0)); //((Object[])rendition.getAttributeValue("page_modifier"))[0].ToString());
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
                RestDocument doc = CurrentRepository.getObjectById<RestDocument>(objectId); //.getDocumentByQualification(
                    //String.Format("dm_document where r_object_id = '{0}'", objectId), null);

                ContentMeta renditionMeta = doc.getRenditionByModifier("Test");
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

/****************   REMOVED TESTS NOT IN CORE PRODUCT ****************************

            private void ImportEmail(float pctTest)
        {
            Random rnd = new Random();
            DirectoryInfo dirInfo = new DirectoryInfo(randomEmailsDirectory);
            int fileCount = dirInfo.GetFiles().Count();
            for (int i = 0; i < (Math.Ceiling(fileCount * pctTest * 10)); i++)
            {
                string movePath = childList[rnd.Next(0, childList.Count)];
                FileInfo file = ObjectUtil.getRandomFileFromDirectoryByExtension(randomEmailsDirectory, "msg");
                if (file == null)
                {
                    WriteOutput("####EMAILIMPORTFAIL##### - No MSG Files are in your test files directory" + randomEmailsDirectory + " unable to do mailimport test");
                    return;
                }
                EmailPackage email = CurrentRepository.ImportEmail(file, testPrefix + "-" + file.Name, movePath);
                WriteOutput("\t[EmailImport] Email Import\t[DeDuplication] Email-De-duplication");
                Boolean isDuplicate = email.IsDuplicate;
                if (isDuplicate)
                {
                    WriteOutput("\t\t" + email.Email.getAttributeValue("object_name") + " is a DUPLICATE");
                    if (email.DuplicateType == DuplicateType.FOLDER)
                    {
                        WriteOutput("\t\t" + email.Email.getAttributeValue("object_name") + " already exists in " + movePath + " object returned is the pre-existing email object.");
                    }
                    else
                    {
                        WriteOutput("\t\t" + email.Email.getAttributeValue("object_name") + " already exists elsewhere, LINKING to " + movePath);
                        CurrentRepository.linkToFolder(email.Email, movePath);
                    }
                }
                else
                {
                    WriteOutput("\t\t" + email.Email.getAttributeValue("object_name") + "was imported");
                }
                
                foreach(RestDocument att in email.Attachments) 
                {
                    WriteOutput("\t\t\t Attachment:" + att.getAttributeValue("object_name"));
                }
            } 
        }

        private void CloseSupportingDocThenParent()
        {
            WriteOutput("\t[DeclareRecord,SetCloseCondition,UnsetCloseCondition]");
            foreach (string cr in childList)
            {
                Folder childFolder = null;
                childFolder = CurrentRepository.getFolderByPath(cr);
                DateTime closeDate = DateTime.Now;
                List<PersistentObject> retainers = CurrentRepository.CloseFolderAndStartRetention(Repository.RecordType.Extradition, childFolder, closeDate);
                DateTime checkDate = DateTime.Parse(retainers[0].getAttributeValue("event_date").ToString());
                try
                {

                    if (checkDate.ToShortDateString().Equals(closeDate.ToShortDateString()))
                    {
                        WriteOutput("\t\t[SetCloseCondition] - Closing CHILD" + childFolder.getAttributeValue("object_name") + " Declared as: " + Repository.RecordType.Extradition);
                    }
                    else throw new Exception();
                }
                catch (Exception e)
                {
                    WriteOutput("\t\t#####FAILED!!E####[SetCloseCondition] - Closing CHILD" + childFolder.getAttributeValue("object_name") + "  as: " + Repository.RecordType.Extradition);
                }
                
            }
            Folder parentFolder   = CurrentRepository.getFolderByPath(ProcessBasePath + parentFolderId);
            CurrentRepository.CloseFolderAndStartRetention(Repository.RecordType.MLAT, parentFolder, DateTime.Now);
            WriteOutput("\t\t[SetCloseCondition] - Closing PARENT " + parentFolder.getAttributeValue("object_name") + " Declared as: " + Repository.RecordType.MLAT);
                
        }

        private void ReOpenParentOrChild()
        {
            
            for (int i = 0; i < (Math.Ceiling(childList.Count * .20)); i++)
            {
                string cr = childList[i];
                Repository.RecordType type = Repository.RecordType.MLAT;
                Folder folder = null;
                if (cr.Contains("/"))
                {
                    type = Repository.RecordType.Extradition;
                    folder = CurrentRepository.getFolderByPath(cr);
                }
                else
                {
                    folder = CurrentRepository.getFolderByPath(ProcessBasePath + cr);
                }
                // Pass a null date to zero out the Close Date event on the retainer to stop aging
                List<PersistentObject> retainers = CurrentRepository.CloseFolderAndStartRetention(type, folder, new DateTime());


                if (retainers[0].getAttributeValue("event_date") == null)
                {
                    WriteOutput("\t[UnsetCloseCondition] " + folder.getAttributeValue("object_name"));        
                }
                else
                {
                    WriteOutput("\t#####FAILED#####!! [UnsetCloseCondition] " + folder.getAttributeValue("object_name"));        
                }
            }
        }



            /**
             * The below items were enablements in DCTM-Rest to handle email import like WDK does (split email from attachments)
             * and a record management function to allow setting a event condition date. They are not in standard Rest Services
             */
//                try
//                {
//                    testName = "ImportEmail";
//                    WriteOutput("-----BEGIN " + testName + "----------------");
//                    tStart = DateTime.Now.Ticks;
//                    float pctTest = 1.0F;
//                    ImportEmail(pctTest);
//                    WriteOutput("Imported " + (Math.Ceiling(
//                        (new DirectoryInfo(randomEmailsDirectory).GetFiles().Count()) * pctTest))
//                        + " emails in " + ((DateTime.Now.Ticks - tStart) / TimeSpan.TicksPerMillisecond) + "ms");
//                    WriteOutput("-----END " + testName + "------------------");
//                }
//                catch (Exception e)
//                {
//                    WriteOutput("#####FAILED##### TEST [" + testName + "]" + e.StackTrace.ToString());
//                }
//
//                try
//                {
//                    testName = "Close Parent or Child";
//                    WriteOutput("-----BEGIN " + testName + "----------------");
//                    tStart = DateTime.Now.Ticks;
//                    CloseSupportingDocThenParent();
//                    WriteOutput("Closed " + childList.Count + " parentFolders/childFolderDocs in " + ((DateTime.Now.Ticks - tStart) / TimeSpan.TicksPerMillisecond) + "ms");
//                    WriteOutput("-----END " + testName + "------------------");
//                }
//                catch (Exception e)
//                {
//                    WriteOutput("#####FAILED##### TEST [" + testName + "]" + e.StackTrace.ToString());
//                }

//                try
//                {
//                    testName = "Re-Open Parent or Child";
//                    WriteOutput("-----BEGIN " + testName + "----------------");
//                    tStart = DateTime.Now.Ticks;
//                    ReOpenParentOrChild();
//                    WriteOutput("Closed " + childList.Count + " parentFolders/childFolderDocs in " + ((DateTime.Now.Ticks - tStart) / TimeSpan.TicksPerMillisecond) + "ms");
//                    WriteOutput("-----END " + testName + "------------------");
//                }
//                catch (Exception e)
//                {
//                    WriteOutput("#####FAILED##### TEST [" + testName + "]" + e.StackTrace.ToString());
//                }