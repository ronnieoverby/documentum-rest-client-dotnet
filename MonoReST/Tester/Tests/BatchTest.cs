using Emc.Documentum.Rest.DataModel;
using Emc.Documentum.Rest.Net;
using Emc.Documentum.Rest.Http.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Emc.Documentum.Rest.Test
{
    public class BatchTest
    {
        public static void Run(RestController client, string RestHomeUri, string repositoryName)
        {
            HomeDocument home = client.Get<HomeDocument>(RestHomeUri, null);
            Feed<Repository> repositories = home.GetRepositories<Repository>(new FeedGetOptions { Inline = true, Links = true });
            Repository repository = repositories.FindInlineEntry(repositoryName);

            BatchCapabilities batchCapabilities = repository.GetBatchCapabilities();
            Console.WriteLine("Batch capabilities for this repository:");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("  - transactional or non-transactional? {0}", batchCapabilities.Transactions);
            Console.WriteLine("  - sequential or non-sequential?       {0}", batchCapabilities.Sequence);
            Console.WriteLine("  - fail on error or continue on error? {0}", batchCapabilities.OnError);
            Console.WriteLine("  - all resources are batch-able except these [{0}] ", String.Join(",", batchCapabilities.NonBatchableResources.ToArray<string>()));
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Press any key to continue batch test..");
            Console.ReadKey();

            Console.WriteLine(String.Format("\r\nPreparing data for the batch.."));  
            User currentUser = repository.GetCurrentUser(new SingleGetOptions());
            Console.WriteLine("\t - Fetched current user: " + currentUser.GetPropertyString("user_name"));
            Cabinet homeCabinet = currentUser.GetHomeCabinet(new SingleGetOptions());
            Console.WriteLine("\t - Fetched home cabinet: " + homeCabinet.GetPropertyString("object_name"));

            // start a batch - create 1 folder & 1 document under home cabinet + get dql + create a new relation
            Console.WriteLine("\r\nReady to create a batch with 5 operations:");
            string folderNamePrefix = "BatchTestFolder-";
            Console.WriteLine("\t - (1) create a new folder under home cabinet with name prefix: " + folderNamePrefix);
            string docNamePrefix = "BatchTestDoc-";
            Console.WriteLine("\t - (2) create a new document under home cabinet with name: " + docNamePrefix);
            string dql = "select * from dm_group";
            Console.WriteLine("\t - (3) execute a dql query: " + dql);
            Console.WriteLine("\t - (4) get all relations");
            Console.WriteLine("\t - (5) create a new cabinet with empty name (should fail)");
            Console.WriteLine("Press any key to run batch..");
            Console.ReadKey();

            List<KeyValuePair<string, object>> dqlParams = new List<KeyValuePair<string, object>>();   
            dqlParams.Add(new KeyValuePair<string, object>("dql", dql));
            string dqlUri = UriUtil.BuildUri(LinkRelations.FindLinkAsString(repository.Links, LinkRelations.DQL.Rel), dqlParams);

            Batch batch = Batch.CreateFromBuilder()
                .Description("a sample batch with 4 operations")
                .BatchSpec(new BatchSpec { Transactional = false, Sequential = false, FailOnError = false, ReturnRequest = false })
                .AddOperation<Folder>(
                    "batch-opt-1", homeCabinet, LinkRelations.FOLDERS.Rel, "POST", ObjectUtil.NewRandomFolder(folderNamePrefix, "dm_folder")
                )
                .AddOperation<Document>(
                    "batch-opt-2", homeCabinet, LinkRelations.DOCUMENTS.Rel, "POST", ObjectUtil.NewRandomDocument(docNamePrefix, "dm_document")
                )
                .AddOperation(
                    "batch-opt-3", dqlUri, "GET"
                )
                .AddOperation(
                    "batch-opt-4", homeCabinet, LinkRelations.RELATIONS.Rel, "GET"
                )
                .AddOperation(
                    "batch-opt-5", repository, LinkRelations.CABINETS.Rel, "POST", new Cabinet()
                )
                .Build();

            Batch result = repository.CreateBatch(batch);
            Console.WriteLine("\r\nPrinting batch result..");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine(String.Format(" - description [{0}]", result.Description));
            Console.WriteLine(String.Format(" - owner       [{0}]", result.Owner));
            Console.WriteLine(String.Format(" - state       [{0}]", result.State));
            Console.WriteLine(String.Format(" - submitted   [{0}]", result.Submitted));
            Console.WriteLine(String.Format(" - started     [{0}]", result.Started));
            Console.WriteLine(String.Format(" - finished    [{0}]", result.Finished));

            foreach (BatchOperation operation in result.Operations)
            {
                Console.WriteLine(String.Format("\r\n - operation    [{0}]", operation.Id));
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine(String.Format("\t--> state        [{0}]", operation.State));
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(String.Format("\t--> started      [{0}]", operation.Started));
                Console.WriteLine(String.Format("\t--> finished     [{0}]", operation.Finished));
                Console.WriteLine("\t--> response");
                Console.WriteLine(String.Format("\t-----------> status  [{0}]", operation.Response.Status));

                if (operation.Response.HasError())
                {
                    Error error = operation.Response.GetError();
                    Console.WriteLine(String.Format("\t-----------> entity  [{0}:{1}]", error.Code, error.Message));
                }
                else
                {
                    if (operation.Id.Equals("batch-opt-1") || operation.Id.Equals("batch-opt-2"))
                    {
                        PersistentObject po = operation.Response.GetObject<PersistentObject>();
                        Console.WriteLine(String.Format("\t-----------> entity  [object of URI {0}]", LinkRelations.FindLinkAsString(po.Links, LinkRelations.SELF.Rel)));
                    }
                    else if (operation.Id.Equals("batch-opt-3") || operation.Id.Equals("batch-opt-4"))
                    {
                        Feed<PersistentObject> feed = operation.Response.GetObject<Feed<PersistentObject>>();
                        Console.WriteLine(String.Format("\t-----------> entity  [feed with title {0} and entry count {1}]", feed.Title, feed.Entries.Count));
                    }
                }
            }
            Console.ForegroundColor = ConsoleColor.White;
        }

        private static string GetAttr(PersistentObject po, string[] attrs)
        {
            foreach (string attr in attrs)
            {
                var v = po.GetPropertyValue(attr);
                if (v != null)
                {
                    return v.ToString();
                }
            }
            return "UNDEFINED";
        }

    }
}
