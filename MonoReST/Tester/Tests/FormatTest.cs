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
    public class FormatTest
    {
        public static void Run(RestController client, string RestHomeUri, string filter, int itemsPerPage, bool pauseBetweenPages, string repositoryName, bool printResult)
        {
            HomeDocument home = client.Get<HomeDocument>(RestHomeUri, null);
            Feed<Repository> repositories = home.GetRepositories<Repository>(new FeedGetOptions { Inline = true, Links = true });
            Repository repository = repositories.FindInlineEntry(repositoryName);

            Console.WriteLine(String.Format("Getting formats for filter '{0}' on repository '{1}', with page size {2}", filter, repository.Name, itemsPerPage));
            
            // REST call to get the 1st page of formats for the specified filter
            FeedGetOptions options = new FeedGetOptions() { ItemsPerPage = itemsPerPage, Inline = true, IncludeTotal = true, Filter = String.IsNullOrEmpty(filter) ? null : filter };
            Feed<Format> formats = repository.GetFormats<Format>(options);
            if (formats != null)
            {
                int totalResults = formats.Total;
                double totalPages = formats.PageCount;
                int docProcessed = 0;
                for (int i = 0; i < totalPages; i++)
                {
                    Console.WriteLine("**************************** PAGE " + (i + 1) + " *******************************");
                    foreach (Entry<Format> obj in formats.Entries)
                    {
                        StringBuilder values = new StringBuilder();
                        Console.WriteLine(String.Format("Name: {0}, \tExtension: {1}, \tMime: {2}",
                                    obj.Content.GetPropertyString("name"),
                                    obj.Content.GetPropertyString("dos_extension"),
                                    obj.Content.GetPropertyString("mime_type")));
                        Console.WriteLine(values.ToString());
                        docProcessed++;
                    }

                    // REST call to get next page 
                    if (totalResults != docProcessed) formats = formats.NextPage();
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
            if (printResult) Console.WriteLine(formats == null ? "NULL" : formats.ToString());
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
