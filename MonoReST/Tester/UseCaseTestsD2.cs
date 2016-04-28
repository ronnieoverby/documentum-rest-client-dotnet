using Emc.Documentum.Rest.DataModel;
using Emc.Documentum.Rest.CustomModel;
using Emc.Documentum.Rest.Net;
using Emc.Documentum.Rest.Http.Utility;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using System.Configuration;
using System.Collections.Specialized;
using System.Windows.Forms;
using Emc.Documentum.Rest.DataModel.D2;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Emc.Documentum.Rest.Test
{
    /// <summary>
    /// 
    /// </summary>
    public class UseCaseTestsD2 : UseCaseTests, IDisposable
    {

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
        public UseCaseTestsD2(RestController client, string RestHomeUri, string repositoryName, bool printResult, string path, 
            int ThreadNum, int numDocs) : base(client, RestHomeUri, repositoryName, printResult, path, ThreadNum, numDocs)
        {

        }


        /// <summary>
        /// 
        /// </summary>
        public UseCaseTestsD2() : base()
        {

        }





        private List<DocumentTracker> MoveDocs { get; set; }

        private bool runTestByName(String testName)
        {
            bool success = false;
            try
            {
                // Import all documents into the holding area (Instantiation Form) before the documents are moveed to parentFolder/child
                WriteOutput("-----BEGIN " + testName + "--------------");
                long tStart = DateTime.Now.Ticks;
                switch (testName)
                {
                    case "CreateTempDocs":
                        CreateTempDocs();
                        WriteOutput("Created " + numDocs + " in " + ((DateTime.Now.Ticks - tStart) / TimeSpan.TicksPerMillisecond) + "ms");
                        break;
                    case "MoveDocs":
                        MoveToFolders();
                        WriteOutput("Moveed " + MoveDocs.Count + " in " + ((DateTime.Now.Ticks - tStart) / TimeSpan.TicksPerMillisecond) + "ms");
                        break;
                    case "CreateFromTemplate":
                        // Create 10 documents by choosing from a random list of templates
                        CreateFromTemplate();
                        WriteOutput("Created 10 documents from template in " + ((DateTime.Now.Ticks - tStart) / TimeSpan.TicksPerMillisecond) + "ms");
                        break;
                    case "MoveDocumentsToParent":
                        // Randomly take some moveed documents and re-move them (Move from a temp location to another location))
                        MoveChildFolderDocumentsToParent();
                        WriteOutput("Moveed " + Math.Ceiling(MoveDocs.Count * .30) + " in "
                            + ((DateTime.Now.Ticks - tStart) / TimeSpan.TicksPerMillisecond) + "ms");
                        break;

                    case "GetDocumentForView":
                        // Take 30% of the documents, and download the content, optionally, will open each one for viewing
                        for (int p = 0; p < (Math.Ceiling(MoveDocs.Count * .3)); p++)
                        {
                            ViewDocument(primaryContentDirectory, MoveDocs[p], openEachFile);
                        }
                        if (showdownloadedfiles) System.Diagnostics.Process.Start(primaryContentDirectory);
                        WriteOutput("Re-Moveed " + Math.Ceiling(MoveDocs.Count * .30) + " in " + ((DateTime.Now.Ticks - tStart) / TimeSpan.TicksPerMillisecond) + "ms");
                        break;
                    case "GetDocumentHistory":
                        GetDocumentHistory();
                        WriteOutput("Fetched RestDocument History of  " + Math.Ceiling(MoveDocs.Count * .10)
                            + " Documents in " + ((DateTime.Now.Ticks - tStart) / TimeSpan.TicksPerMillisecond) + "ms");
                        break;
                    case "UploadRendition":
                        CreateRendition(0, false);
                        WriteOutput("Imported new Renditions of  " + Math.Ceiling(MoveDocs.Count * .30)
                            + " Documents in " + ((DateTime.Now.Ticks - tStart) / TimeSpan.TicksPerMillisecond) + "ms");
                        break;
                    case "ViewRenditions":
                        // This may be used to get the text version of the email for the correspondence view
                        ViewRenditions(renditionsDirectory, IDsWithRenditions, openEachFile);
                        // Open a directory with the downloaded renditions to show the tester
                        if (showdownloadedfiles) System.Diagnostics.Process.Start(renditionsDirectory);
                        WriteOutput("Downloaded renditions of  " + Math.Ceiling(MoveDocs.Count * .30) + " Documents for view in "
                            + ((DateTime.Now.Ticks - tStart) / TimeSpan.TicksPerMillisecond) + "ms");
                        break;
                    case "ImportAsNewVersion":
                        ImportAsNewVersion();
                        WriteOutput("New Versions of  " + Math.Ceiling(MoveDocs.Count * .20) + " Documents for created in " + ((DateTime.Now.Ticks - tStart) / TimeSpan.TicksPerMillisecond) + "ms");
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
                WriteOutput("#####FAILED##### TEST [" + testName + "]" + e.StackTrace.ToString());
            }
            return success;
        }

        /// <summary>
        /// 
        /// </summary>
        new public void Start()
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
            WriteOutput("Took " + ((DateTime.Now.Ticks - testStart) / TimeSpan.TicksPerMillisecond) + "ms to get RestService");
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
                throw new System.Exception("One of the tests that create Documents is required for other tests to run. "
                    + "You must enable either the CreateTempDocs test and/or the CreateTempDocs test in order to create "
                    + "documents that can be used in subsequent tests.");
            }

            C2ViewDocument(@"C:\SamplesToImport", "09000001800d180f", true);
            return;
            MoveDocs = new List<DocumentTracker>();
            foreach (String key in restTests)
            {
                bool preCheckOk = true;
                // This test is not available in versions earlier than 7.2
                double restVersion = Double.Parse((productInfo.Properties.Major.Equals("NA") ? "7.2" : productInfo.Properties.Major));

                if (key.Equals("Search"))
                {
                    if (!(restVersion >= 7.2d))
                    {
                        preCheckOk = false;
                        Console.WriteLine("Documentum Rest Version 7.2 or higher is required to use Search, your version is: "
                            + restVersion + " Skipping...");
                    }
                }

                // These features do not work on Mono yet, should be fine when .NetCore is released though
                if (key.Equals("ExportParent") || key.Equals("ExportListOfFiles"))
                {
                    if (Type.GetType("Mono.Runtime") != null)
                    {
                        preCheckOk = false;
                        Console.WriteLine("The zip libraries required for [" + key + " ] have failed under Mono, skipping this  test. If you "
                            + "want to test for yourself, you will have to modify the source to allow it udner (UseCaseTests");
                    }
                }

                if (preCheckOk)
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





        public void C2ViewDocument(String path, string objectId, bool openDocument)
        {

            D2Document doc = CurrentRepository.getObjectById<D2Document>(objectId);
            C2Views views = doc.getC2Views();
            List<C2ViewEntry> entries = views.Entries;
            C2ViewEntry entry = entries[0];
            C2View view = entry.C2View;
            using (Stream stream = CurrentRepository.Client.GetRaw(view.Url))
            {
                if (stream == null)
                {
                    throw new Exception("Stream came back null. This is normally caused by an unreachable ACS Server (DNS problem or Method Server DOWN). ACS URL is: " + view.Url);
                }
                FileStream fs = File.Create(path + "\\Test.Pdf");
                stream.CopyTo(fs);
                fs.Dispose();
            }

            
            if (openDocument) System.Diagnostics.Process.Start(path+"\\Test.pdf");
        }

        private void Temp()
        {

            if (CurrentRepository.isD2Rest())
            {
                /* Get D2 Configs */
                     D2Configurations d2configs = CurrentRepository.GetD2Configurations(null);


                /* Get the Search Configurations from D2 Config */
                SearchConfigurations searchConfigs = d2configs.getSearchConfigurations();
                int i = 0;
                for (i = 0; i < searchConfigs.Entries.Count; i++)
                {
                    /* For Each Search configuration, get the entry link */
                    SearchConfigLink scl = searchConfigs.Entries[i];
                    //Console.WriteLine("SearchConfigTitle=" + scl.title + ", SearchConfigId=" + scl.id + " LinkSrc: " + scl.content.Src);
                    /* Ouput SearchConfiguration information for each SearchConfigLink */
                    SearchConfiguration sc = searchConfigs.getSearchConfiguration(scl.content.Src);
                    //Console.WriteLine(sc.ToString());

                }

                /* Get the Profile Configurations from D2 Config */
                ProfileConfigurations profileConfigs = d2configs.getProfileConfigurations();
                i = 0;
                // for (i=0; i < profileConfigs.Entries.Count; i++)
                //{
                /* For each profile configuraton get the entry link */
                ProfileConfigLink pcl = profileConfigs.Entries[i];
                //Console.WriteLine("\n\nProfileConfigTitle=" + pcl.title + ", ProfileConfigId=" + pcl.id + " LinkSrc: " + pcl.content.Src);
                /* Output ProfileConfiguration information for each ProfileConfigLink */
                ProfileConfiguration pc = profileConfigs.getProfileConfiguration(pcl.content.Src);
                //Console.WriteLine(pc.ToString());
                D2Document d2doc = new D2Document();
                d2doc.setAttributeValue("object_name", "D2-ConfigTst-" + DateTime.Now.Ticks);
                d2doc.setAttributeValue("primary_bus_owner", "Rest");
                d2doc.setAttributeValue("template_developers", new String[] { "dmadmin" });
                d2doc.setAttributeValue("comm_reviewers", new String[] { "dmadmin" });
                d2doc.setAttributeValue("business_reviewers", new String[] { "dmadmin" });
                d2doc.setAttributeValue("compliance_reviewers", new String[] { "dmadmin" });
                d2doc.setAttributeValue("brand_reviewers", new String[] { "dmadmin" });
                d2doc.setAttributeValue("legal_reviewers", new String[] { "dmadmin" });
                d2doc.setAttributeValue("ada_reviewers", new String[] { "dmadmin" });
                d2doc.setAttributeValue("template_admins", new String[] { "dmadmin" });
                d2doc.setAttributeValue("form_type", "ACT");
                d2doc.setAttributeValue("form_subtype", "Alternate Loan Notice");
                d2doc.setAttributeValue("document_subject", "Automatic payment");
                d2doc.setAttributeValue("requester", "dmadmin");
                d2doc.setAttributeValue("r_object_type", "wf_form_template");
                d2doc.setAttributeValue("r_is_virtual_doc", Convert.ToInt32(true));
                d2doc.setAttributeValue("import_archive", false);
                d2doc.setAttributeValue("a_status", "Revise");
                d2doc.setAttributeValue("merge_needed", true);
                d2doc.setAttributeValue("system_ver_available", true);
                D2Configuration d2config = new D2Configuration();
                d2config.LifeCycle = "WF Template Lifecycle";
                d2config.StartVersion = 0.5d;
                // This was an attempt to figure out what the properties_string/properties_xml properties that d2-config has. It was a fail 
                // so will have to wait for documentation to update to reflect what these do.
                //d2config.PropertiesString = "title=BLAHBLAHBLAH";
                d2doc.Configuration = d2config;

                GenericOptions importOptions = new GenericOptions();
                importOptions.SetQuery("format", ObjectUtil.getDocumentumFormatForFile("RestDotNetFramework.docx"));
                d2doc = CurrentRepository.ImportD2DocumentWithContent(d2doc, new FileInfo(@"C:\SamplesToImport\RestDotNetFramework.docx")
                    .OpenRead(), ObjectUtil.getMimeTypeFromFileName("RestDotNetFramework.docx"), importOptions);


                if (d2doc != null)
                {
                    Console.WriteLine("\n\nNew D2Document: \n" + d2doc.ToString());
                }
                else
                {
                    Console.WriteLine("Creation failed!");
                }
                Console.WriteLine("==================================================================================");
                Console.WriteLine("TaskList Basic Info:");
                Feed<D2Task> taskFeed = CurrentRepository.GetD2TaskList();
                List<D2Task> tasks = ObjectUtil.getFeedAsList(taskFeed);
                int taskNum = 0;
                foreach (D2Task task in tasks)
                {
                    taskNum++;
                    Console.WriteLine("TASK #" + taskNum + "-------------------------------------------------");
                    Console.WriteLine("\tTaskSubject: " + task.TaskSubject + " TaskInstructions: " + task.TaskInstructions);
                    Console.WriteLine("\tForward Tasks: ");
                    foreach (String key in task.TaskRequirements.ForwardTasks.Keys)
                    {
                        Console.WriteLine("\t\t" + "TaskName: " + key + " TaskId" + task.TaskRequirements.ForwardTasks[key]);
                    }
                }

                Console.ReadLine();


            }
        }
    }
}
