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
    public class SearchTest
    {
        public static void Run(RestController client, string RestHomeUri, string terms, int itemsPerPage, bool pauseBetweenPages, string repositoryName, bool printResult)
        {
            HomeDocument home = client.Get<HomeDocument>(RestHomeUri, null);
            Feed<Repository> repositories = home.GetRepositories<Repository>(new FeedGetOptions { Inline = true, Links = true });
            Repository repository = repositories.FindInlineEntry(repositoryName);

            Console.WriteLine(String.Format("Running full-text search for terms '{0}' on repository '{1}', with page size {2}", terms, repository.Name, itemsPerPage));
            
            // REST call to get the 1st page of the search result
            SearchOptions options = new SearchOptions() { ItemsPerPage = itemsPerPage, Inline = false, IncludeTotal = true, SearchQuery = terms };
            Feed<PersistentObject> searchResult = repository.ExecuteSimpleSearch<PersistentObject>(options);
            if (searchResult != null)
            {
                int totalResults = searchResult.Total;
                double totalPages = searchResult.PageCount;
                int docProcessed = 0;
                for (int i = 0; i < totalPages; i++)
                {
                    Console.WriteLine("**************************** PAGE " + (i + 1) + " *******************************");
                    foreach (Entry<PersistentObject> obj in searchResult.Entries)
                    {
                        StringBuilder values = new StringBuilder();
                        Console.WriteLine(String.Format("Score: {0:0.000},   ID: {1},   Name: {2},    Highlight:\n {3}",
                                    obj.Score,
                                    obj.Id,
                                    obj.Title,
                                    obj.Summary));
                        Console.WriteLine(values.ToString());
                        docProcessed++;
                    }

                    // REST call to get next page of the search
                    if (totalResults != docProcessed) searchResult = searchResult.NextPage();
                    Console.WriteLine("*******************************************************************"); 
                    Console.WriteLine("Page:" + (i + 1) + " Results: " + docProcessed + " out of " + totalResults + " Processed");
                    Console.WriteLine("*******************************************************************");
                    Console.WriteLine("\n\n");
                    if (pauseBetweenPages)
                    {
                        Console.WriteLine("Press 'q' to quit, 'g' to run to end, or any other key to run next page..");
                        ConsoleKeyInfo next = Console.ReadKey();
                        if (next.KeyChar.Equals('q'))
                        {
                            return;
                        }
                        if (next.KeyChar.Equals('g'))
                        {
                            pauseBetweenPages = false;
                        }
                    }
                }
                    
            }
            if (printResult) Console.WriteLine(searchResult == null ? "NULL" : searchResult.ToString());
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
