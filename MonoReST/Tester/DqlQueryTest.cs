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

            Console.WriteLine("Running DQL query on repository '" + repositoryName + "'], with page size " + itemsPerPage);
            Feed<PersistentObject> queryResult = repository.ExecuteDQL<PersistentObject>(query, new FeedGetOptions() { ItemsPerPage = itemsPerPage, IncludeTotal = true});
            String[] attributes = query.Substring(7, query.IndexOf("from")-7)
                .Replace(" ","").Split(new String[] {","}, StringSplitOptions.RemoveEmptyEntries);
            if (queryResult != null)
            {
                int totalResults = queryResult.Total;
                double totalPages = queryResult.PageCount;
                int docProcessed = 0;
                //int pageCount = queryResult.Entries.c
                for (int i = 0; i < totalPages; i++)
                {
                    Console.WriteLine("********************PAGE " + (i + 1) + "****************************");
                    //List<Entry<PersistentObject>> objs = (List<Entry<PersistentObject>>)queryResult.Entries;
                    List<PersistentObject> objs = ObjectUtil.getFeedAsList(queryResult, true);
                    foreach (PersistentObject obj in objs)
                    {
                        StringBuilder values = new StringBuilder();
                        bool first=true;
                        Console.WriteLine(String.Format("\tName: {0} \t\tID: {1}",
                        GetAttr(obj, new string[] {"object_name", "user_name", "group_name", "name"}),
                        GetAttr(obj, new string[] {"r_object_id"})));
                        foreach (string attribute in attributes)
                        {
                            if(first) {
                                values.Append(attribute).Append("=").Append(obj.GetPropertyValue(attribute));
                            } else {
                                values.Append(",").Append(attribute).Append("=").Append(obj.GetPropertyValue(attribute));
                            }
                        }
                        Console.WriteLine(values.ToString());
                        docProcessed++;
                    }

                    if (totalResults != docProcessed) queryResult = queryResult.NextPage(); 
                    Console.WriteLine("*****************************************************"); 
                    Console.WriteLine("Page:" + (i + 1) + " Results: " + docProcessed + " out of " + totalResults + " Processed");
                    Console.WriteLine("*****************************************************");
                    Console.WriteLine("\n\n");
                    if (pauseBetweenPages)
                    {
                        Console.WriteLine("Press any key for next page");
                        Console.ReadKey();
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
