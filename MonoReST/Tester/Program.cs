using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Threading;
using System.Threading.Tasks;
using Emc.Documentum.Rest.DataModel;
using Emc.Documentum.Rest.Net;
using Emc.Documentum.Rest.Http.Utility;
using System.IO;
using System.Collections.Specialized;
using System.Windows.Forms;
using Emc.Documentum.Rest.DataModel.D2;

namespace Emc.Documentum.Rest.Test
{
    class Program
    {
        private static string RestHomeUri = "#EXAMPLE: http://apps.company.com/dctm-rest/services):";
        private static string username = "#EXAMPLE: dmadmin";
        private static string password = "#EXAMPLE: MyS^p3Cc3P@55W0rd!";
        private static RestController client;
        private static string repositoryName = "#EXAMPLE: CompanyDocbase";
        private static bool printResult = false;

        private static bool hasConfigInitialized = false;
        [STAThread]
        static void Main(string[] args)
        {

            Console.ForegroundColor = ConsoleColor.White;
            // Console.BufferHeight = 360;
            // Console.BufferHeight = 210;
            NameValueCollection config = getDefaultConfiguration();
            bool useDefault = config != null && Boolean.Parse(config["useDefaults"]);
            SetupTestData(useDefault);

            Arguments cmd = PrintMenu();
            while (!cmd.Exit())
            {
                string next = cmd.Next();
                try
                {
                    switch (next.ToLower())
                    {
                        case "dql":
                            int itemsPerPage = cmd.IsNextInt() ? cmd.NextInt() : 10;
                            bool pause = cmd.IsNextBool() ? cmd.NextBool() : false;
                            string dql = cmd.ReadToEnd();
                            DqlQueryTest.Run(client, RestHomeUri, dql, itemsPerPage, pause, repositoryName, printResult);
                            break;
                        case "test": // will run the conditions for Processdoc
                            testProcessDocs();
                            break;
                        case "getmimetype":
                            Console.WriteLine("Mime-Type for " + cmd.Peak() + " is:" + ObjectUtil.getMimeTypeFromFileName(cmd.Peak()));
                            break;
                        case "getformat":
                            Console.WriteLine("Format for " + cmd.Peak() + " is:" + ObjectUtil.getDocumentumFormatForFile(cmd.Peak()));
                            break;
                        case "cls":
                            Console.Clear();
                            break;
                        case "batch":
                            Console.WriteLine("Development in progress, not useable as of yet");
                            break;
                        case "search":
                            Console.WriteLine("Development in progress, not useable as of yet");
                            break;
                        case "reconfig":
                            SetupTestData(false);
                            break;
                        case "d2tests":
                            Console.WriteLine("D2 Rest Services available: " + d2Tests());
                            break;
                        default:
                            Console.WriteLine("Nothing entered");
                            break;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Got exception in test: " + e.Message + "\r\n" + e.StackTrace);
                }
                
                if (!next.Equals("cls"))
                {
                    Console.WriteLine("Press any key to continue...\r\n");
                    Console.ReadKey();
                }
                cmd = PrintMenu();
            }
        }

        private static bool d2Tests()
        {
            bool result = true;

            
            client = new RestController(username, password);
            RestService home = client.Get<RestService>(RestHomeUri, null);
            if (home == null)
            {
                string msg = "\nUnable to get Rest Service at: " + RestHomeUri + " check to see if the service is available.";
                Console.WriteLine(msg);
                throw new Exception(msg);
            }
            home.SetClient(client);
            Repository repository = home.GetRepository(repositoryName);
            if (repository.isD2Rest())
            {
                /* Get D2 Configs */
                D2Configurations d2configs = repository.GetD2Configurations(null);
                

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
                    d2doc = repository.ImportD2DocumentWithContent(d2doc,new FileInfo(@"C:\SamplesToImport\RestDotNetFramework.docx")
                        .OpenRead(), ObjectUtil.getMimeTypeFromFileName("RestDotNetFramework.docx"), importOptions);
                    

                    if (d2doc != null)
                    {
                        Console.WriteLine("\n\nNew D2Document: \n" + d2doc.ToString());
                    }
                    else
                    {
                        Console.WriteLine("Creation failed!");
                        result = false;
                    }
                Console.WriteLine("==================================================================================");
                Console.WriteLine("TaskList Basic Info:");
                Feed<D2Task> taskFeed = repository.GetD2TaskList();
                List<D2Task> tasks = ObjectUtil.getFeedAsList(taskFeed);
                int taskNum = 0;
                foreach(D2Task task in tasks)
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
            else {
                Console.WriteLine("You do not appear to be running the D2FS-Rest Services");
                result = false; }
            return result;
        }

        private static void testProcessDocs()
        {
            //if (cmd == null || cmd.Trim().Equals("") || cmd.Equals(test))
            //{
            //    Console.WriteLine("Path argument is required");
            //    break;
            //}
            // Creates documents, assigns them to a temp holding area, then creates case/request folders
            // and assigns the documents randomly. Cleans up the temp folder upon completion.
            int numDocs = getInputNumber("How many documents do you want to create and assign?", 10);
            int threadCount = getInputNumber("How many threads would you like to run?", 1);
            long start = DateTime.Now.Ticks;
            if (threadCount >= 1)
            {
                for (int i = 0; i < threadCount; i++)
                {
                    UseCaseTests aTest = new UseCaseTests(new RestController(username, password),
                            RestHomeUri, repositoryName, threadCount > 1 ? true : false, "/Temp/Test-" + DateTime.Now.Ticks, i, numDocs);
                    ThreadStart job = new ThreadStart(aTest.Start);
                    new Thread(job).Start();
                }
                Console.WriteLine("\n\n " + numDocs + " documents will be imported and randomly assigned to " + threadCount + " cases, 5 requests for each of "
                + threadCount + " threads");
            }
            else
            {
                UseCaseTests aTest = new UseCaseTests(new RestController(username, password),
                        RestHomeUri, repositoryName, threadCount > 1 ? true : false, "/Temp/Test-" + DateTime.Now.Ticks, 1, numDocs);
                aTest.Start();
            }
        }

        private static string getPrimaryCommand(String line)
        {
            String test = "";
            if (!String.IsNullOrWhiteSpace(line))
            {
                test = line.IndexOf(" ") > 0 ? line.Substring(0, line.IndexOf(" ")) : line;
            }
            return test;
        }

        private static int getInputNumber(string title, int value)
        {

            string input = "";
            try
            {
                Console.WriteLine("\t" + title + " [" + value + "]");
                Console.WriteLine();
                input = getLineOfInput("","");
                if (input != null && !input.Trim().Equals(""))
                {
                    value = int.Parse(input);
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("Input: " + input + " cannot be parsed to an integer, using default value of " + value, e);
            }
            return value;
        }

        public static Arguments PrintMenu()
        {
            Console.WriteLine("Usage: ");
            Console.WriteLine("\treconfig \n\t\t- prompt for re-entering configuration information.");
            Console.WriteLine("\tdql [<itemsPerPage> <pauseBetweenPages>] <dqlQuery>" 
                            + "\n\t\t- Executes a DQL query and prints the results."
                            + "\n\t\t  Example: dql 10 false select * from dm_cabinet");
            Console.WriteLine("\tgetmimetype <filename> \n\t\t- Returns the mimetype of the given fileName.");
            Console.WriteLine("\tgetformat <filename> \n\t\t- Returns the documentum format name of the given fileName.");
            Console.WriteLine("\tsearch \n\t\t- Prompts for search criteria and location then runs the search query.");
            Console.WriteLine("\ttest \n\t\t- Runs the end to end tests with optional tthreads and number of documents. "
                            + "\n\t\t  The old Processdoc command will do the same test.");
            Console.WriteLine("\tcls \n\t\t- Clear the console");
            Console.WriteLine("\texit \n\t\t- Exit the Test");
            Console.Write("\r\n\nCommand > ");

            return new Arguments(getLineOfInput("", "").Trim());
        }

        private static string getLineOfInput(String prompt, String defaultValue)
        {
            string line = null;
            bool isInputValid = false;

            while (!isInputValid )
            {
                if (!String.IsNullOrEmpty(prompt)) Console.WriteLine(prompt);
                line = Console.ReadLine();
                if( String.IsNullOrEmpty(line) && defaultValue.StartsWith("#EXAMPLE: ") || line.StartsWith("#EXAMPLE: ") )
                {
                    Console.WriteLine("\nInvalid input, value should not start with \"#EXAMPLE: \"");
                } else
                {
                    isInputValid = true;
                }
                
            }

            return line;
        }

        private static String getLineOfHiddenInput(String prompt, String defaultValue)
        {
            if (!String.IsNullOrEmpty(prompt)) Console.WriteLine(prompt);
            ConsoleKeyInfo inf;
            StringBuilder input = new StringBuilder();
            inf = Console.ReadKey(true);
            while (inf.Key != ConsoleKey.Enter)
            {
                input.Append(inf.KeyChar);
                inf = Console.ReadKey(true);
            }
            Console.WriteLine();
            return input.ToString();

        }

        private static void SetupTestData(bool useDefaults)
        {

            NameValueCollection restConfig = getDefaultConfiguration();
            if (restConfig != null && !hasConfigInitialized)
            {
                RestHomeUri = restConfig["defaultRestHomeUri"];
                username = restConfig["defaultUsername"];
                password = restConfig["defaultPassword"];
                repositoryName = restConfig["defaultRepositoryName"];
                printResult = Boolean.Parse(restConfig["defaultPrintResult"].ToString());
                validateConfig();
                if (hasConfigInitialized)
                {
                    setupClient();
                    return;
                }
            }
            hasConfigInitialized = false;
            while (!hasConfigInitialized)
            {
                if (useDefaults && hasConfigInitialized)
                {
                    Console.Write("Configuration completed with default settings. ");
                    printConfiguration();
                }
                else
                {

                    readSetupParameters(RestHomeUri, username, password, repositoryName, printResult.ToString());
                    hasConfigInitialized = true;
                    Console.Write("Re-configuration completed. ");
                    printConfiguration();
                    validateConfig();
                }
            }

            setupClient();
            
        }

        private static void setupClient()
        {
            client = String.IsNullOrEmpty(password) ? new RestController(null, null) : new RestController(username, password);
            // alternatively, you can choose .net default data contract serializer: new DefaultDataContractJsonSerializer();
            JsonDotnetJsonSerializer serializer = new JsonDotnetJsonSerializer();
            serializer.PrintStreamBeforeDeserialize = true;
            client.JsonSerializer = serializer;
        }

        private static void validateConfig()
        {
            if (!(
                RestHomeUri.StartsWith("#EXAMPLE: ")
                && username.StartsWith("#EXAMPLE: ")
                && password.StartsWith("#EXAMPLE: ") //Ok, so your password can't start with #EXAMPLE: but I will take those odds
                && repositoryName.StartsWith("#EXAMPLE: ")
                )
             )
            {
                hasConfigInitialized = true;
            } else
            {
                hasConfigInitialized = false;
            }
        }

        private static NameValueCollection getDefaultConfiguration()
        {
            try
            {
                return ConfigurationManager.GetSection("restconfig") as NameValueCollection;
            }
            catch (ConfigurationErrorsException se)
            {
                Console.WriteLine("Configuration could  not load. If you are running under Visual Studio, ensure:\n" +
                    "\n\"<section name=\"Restconfig\" type=\"System.Configuration.NameValueSectionHandler\"/> is used. " +
                    "\nIf running under Mono, ensure: " +
                    "\n<section name=\"Restconfig\" type=\"System.Configuration.NameValueSectionHandler,System\"/> is used");
                return null;
            }
        }

        private static void printConfiguration()
        {
            Console.WriteLine("Please review your settings below:\r\n");
            Console.WriteLine("\tHome document URL \t[" + RestHomeUri + "]");
            Console.WriteLine("\tUser login name \t[" + username + "]");
            Console.WriteLine("\tRepository name \t[" + repositoryName + "]");
            Console.WriteLine("\tPrint the result? \t[" + printResult + "]\r\n");
        }

        public static void readSetupParameters(string defaultRestHomeUri, string defaultUsername, string defaultPassword,
            string defaultRepositoryName, string defaultPrintResult)
        {
            RestHomeUri = getLineOfInput("Set the home document URL [" + defaultRestHomeUri + "] :", defaultRestHomeUri);
            if (String.IsNullOrEmpty(RestHomeUri)) RestHomeUri = defaultRestHomeUri;

            username = getLineOfInput("Set the username [" + defaultUsername + "] :", defaultUsername);
            if (String.IsNullOrEmpty(username)) username = defaultUsername;


            password = getLineOfHiddenInput("Set the user password [**********] :",defaultPassword);
            if (String.IsNullOrEmpty(password)) password = defaultPassword;

            repositoryName = getLineOfInput("Set the repository name [" + defaultRepositoryName + "] :", defaultRepositoryName);
            if (String.IsNullOrEmpty(repositoryName)) repositoryName = defaultRepositoryName;

            string input = getLineOfInput("Whether to print the result [" + defaultPrintResult + "] :", defaultPrintResult);
            printResult = String.IsNullOrEmpty(input) ? Boolean.Parse(defaultPrintResult) : Boolean.Parse(input);
        }
    }
}

