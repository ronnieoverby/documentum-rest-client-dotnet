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
    public class TypeTest
    {
        public static void Run(RestController client, string RestHomeUri, string parent, int itemsPerPage, bool pauseBetweenPages, string repositoryName, bool printResult)
        {
            HomeDocument home = client.Get<HomeDocument>(RestHomeUri, null);
            Feed<Repository> repositories = home.GetRepositories<Repository>(new FeedGetOptions { Inline = true, Links = true });
            Repository repository = repositories.FindInlineEntry(repositoryName);

            Console.WriteLine(String.Format("Getting types for parent '{0}' on repository '{1}', with page size {2}", parent, repository.Name, itemsPerPage));
            
            // REST call to get the 1st page of the types
            FeedGetOptions options = new FeedGetOptions() { ItemsPerPage = itemsPerPage, Inline = true, IncludeTotal = true };
            if (!String.IsNullOrEmpty(parent))
            {
                options.SetQuery("parent-type", parent);
            }
            options.SetQuery("inherited", false);
            Feed<DmType> types = repository.GetTypes(options);
            if (types != null)
            {
                int totalResults = types.Total;
                double totalPages = types.PageCount;
                int docProcessed = 0;
                for (int i = 0; i < totalPages; i++)
                {
                    Console.WriteLine("**************************** PAGE " + (i + 1) + " *******************************");
                    foreach (Entry<DmType> obj in types.Entries)
                    {
                        StringBuilder values = new StringBuilder();
                        Console.WriteLine(String.Format("Name: {0}, \t\tParent: {1}, \tNon-inherited Attrs: {2}",
                                    obj.Title,
                                    String.IsNullOrEmpty(obj.Content.Parent) ? "" : obj.Content.Parent.Substring(obj.Content.Parent.LastIndexOf("/") + 1),
                                    obj.Content.TypeProperties == null ? 0 : obj.Content.TypeProperties.Count));
                        Console.WriteLine(values.ToString());
                        docProcessed++;
                    }

                    // REST call to get next page of the types
                    if (totalResults != docProcessed) types = types.NextPage();
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
            if (printResult) Console.WriteLine(types == null ? "NULL" : types.ToString());
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
