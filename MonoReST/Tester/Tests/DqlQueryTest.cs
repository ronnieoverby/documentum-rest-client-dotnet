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
    public class DqlQueryTest
    {
        public static void Run(RestController client, string RestHomeUri, string query, int itemsPerPage, bool pauseBetweenPages, string repositoryName, bool printResult)
        {
            HomeDocument home = client.Get<HomeDocument>(RestHomeUri, null);
            home.SetClient(client);
            Feed<Repository> repositories = home.GetRepositories<Repository>(new FeedGetOptions { Inline = true, Links = true });
            Repository repository = repositories.FindInlineEntry(repositoryName);

            Console.WriteLine(String.Format("Running DQL query '{0}' on repository '{1}', with page size {2}", query, repository.Name, itemsPerPage));
            
            // REST call to get the 1st page of the dql query
            Feed<PersistentObject> queryResult = repository.ExecuteDQL<PersistentObject>(query, new FeedGetOptions() { ItemsPerPage = itemsPerPage, IncludeTotal = true});
            if (queryResult != null)
            {
                int totalResults = queryResult.Total;
                double totalPages = queryResult.PageCount;
                int docProcessed = 0;
                //int pageCount = queryResult.Entries.c
                for (int i = 0; i < totalPages; i++)
                {
                    Console.WriteLine("**************************** PAGE " + (i + 1) + " *******************************");
                    foreach (Entry<PersistentObject> obj in queryResult.Entries)
                    {
                        StringBuilder values = new StringBuilder();
                        Console.WriteLine(String.Format("  ID: {0} \t\tName: {1}",
                        GetAttr(obj.Content, new string[] {"r_object_id"}),
                        GetAttr(obj.Content, new string[] {"object_name", "user_name", "group_name", "name"})));
                        Console.WriteLine(values.ToString());
                        docProcessed++;
                    }

                    // REST call to get next page of the dql query
                    if (totalResults != docProcessed) queryResult = queryResult.NextPage();
                    Console.WriteLine("*******************************************************************"); 
                    Console.WriteLine("Page:" + (i + 1) + " Results: " + docProcessed + " out of " + totalResults + " Processed");
                    Console.WriteLine("*******************************************************************");
                    Console.WriteLine("\n\n");
                    if (pauseBetweenPages)
                    {
                        Console.WriteLine("Press any key for next page or 'q' to quit DQL query test..");
                        ConsoleKeyInfo key = Console.ReadKey();
                        if (key.KeyChar.Equals('q'))
                        {
                            return;
                        }
                    }
                }
                    
            }
            if (printResult) Console.WriteLine(queryResult==null ? "NULL" : queryResult.ToString());
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
