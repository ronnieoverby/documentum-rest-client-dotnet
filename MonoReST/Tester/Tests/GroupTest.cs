using Emc.Documentum.Rest.DataModel;
using Emc.Documentum.Rest.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Emc.Documentum.Rest.Test
{
    public class GroupTest
    {
        public static void Run(RestController client, string RestHomeUri, string filter, int itemsPerPage, bool pauseBetweenPages, string repositoryName, bool printResult, string groupName)
        {
            // get repository resource
            HomeDocument home = client.Get<HomeDocument>(RestHomeUri, null);
            Feed<Repository> repositories = home.GetRepositories<Repository>(new FeedGetOptions { Inline = true, Links = true });
            Repository repository = repositories.FindInlineEntry(repositoryName);

            // get a collection of groups
            GetGroups(repository, filter, itemsPerPage, pauseBetweenPages, printResult);

            OperationsOnGroup(repository, groupName, itemsPerPage, pauseBetweenPages, printResult);
        }

        private static void OperationsOnGroup(Repository repository, string groupName, int itemsPerPage, bool pauseBetweenPages, bool printResult)
        {
            //create or get group
            Group group = CreateOrGetGroup(repository, groupName, printResult);
            if (group == null)
            {
                return;
            }
            RepeatableOperations(group, itemsPerPage, pauseBetweenPages, printResult);
            DeleteGroup(group);
        }

        private static Group CreateOrGetGroup(Repository repository, string groupName, bool printResult)
        {
            Group group;
            if (String.IsNullOrWhiteSpace(groupName))
            {
                groupName = "Net_Sample_Test_Group" + new Random().Next();
                if (!isContinue("Press 'q' to exit, or any other key to create group '" + groupName + "' for following tests..", 'q'))
                {
                    return null;
                }
                group = CreateGroup(repository, groupName);
            }
            else
            {

                if (!isContinue("Press 'q' to exit, or any other key to create/retrieve the group '" + groupName + "'..", 'q'))
                {
                    return null;
                }

                // create group if the groupName does not exist
                group = CreateGroupIfNeed(repository, printResult, groupName);
            }
            return group;
        }

        private static void RepeatableOperations(Group group, int itemsPerPage, bool pauseBetweenPages, bool printResult)
        {
            do
            {
                Console.WriteLine("\nPress 'e' to edit group , 'u' to get group's sub users,'g' to get group's sub groups,'p' to get parent groups, 'q' to back to previous layer, or any other key run all tests..");
                Console.ForegroundColor = ConsoleColor.Yellow;
                ConsoleKeyInfo next = Console.ReadKey();
                Console.ForegroundColor = ConsoleColor.White;
                string keyword = next.KeyChar.ToString();

                switch (keyword)
                {
                    case "e":
                        group = UpdateGroup(group, printResult);
                        break;
                    case "u":
                        GetGroupUsers(group, itemsPerPage, pauseBetweenPages, printResult);
                        break;
                    case "g":
                        GetGroupGroups(group, itemsPerPage, pauseBetweenPages, printResult);
                        break;
                    case "p":
                        GetParentGroups(group, itemsPerPage, pauseBetweenPages, printResult);
                        break;
                    case "q":
                        Console.WriteLine("\n");
                        return;
                    default:
                        group = UpdateGroup(group, printResult);
                        GetGroupUsers(group, itemsPerPage, pauseBetweenPages, printResult);
                        GetGroupGroups(group, itemsPerPage, pauseBetweenPages, printResult);
                        GetParentGroups(group, itemsPerPage, pauseBetweenPages, printResult);
                        break;
                }
            } while (true);
        }

        private static void GetGroups(Repository repository, string filter, int itemsPerPage, bool pauseBetweenPages, bool printResult)
        {
            // REST call to get the 1st page of groups 
            FeedGetOptions options = new FeedGetOptions() { ItemsPerPage = itemsPerPage, Inline = true, IncludeTotal = true, Filter = (String.IsNullOrEmpty(filter) || filter.Equals("null")) ? null : filter };
            Feed<Group> groups = repository.GetGroups<Group>(options);

            printFeed(groups, pauseBetweenPages, printResult, "There is exception to get groups in repository '" + repository.Name + "'");
        }

        private static bool isContinue(string workflowMessage, char keyword)
        {
            Console.WriteLine("\n\t" + workflowMessage);
            Console.ForegroundColor = ConsoleColor.Yellow;
            ConsoleKeyInfo next = Console.ReadKey();
            Console.ForegroundColor = ConsoleColor.White;
            if (next.KeyChar.Equals(keyword))
            {
                Console.WriteLine("\n");
                return false;
            }
            return true;
        }

        private static Group CreateGroupIfNeed(Repository repository, bool printResult, string groupNameToCreate)
        {
            string _defaultGroupName = "Net_Sample_Test_Group" + new Random().Next();
            if (String.IsNullOrEmpty(groupNameToCreate))
            {
                groupNameToCreate = _defaultGroupName;
            }

            string filter = "group_name='" + groupNameToCreate + "'";
            FeedGetOptions options = new FeedGetOptions() { ItemsPerPage = 10, Inline = true, IncludeTotal = true, Filter = filter };
            Feed<Group> groups = repository.GetGroups<Group>(options);
            Group group;
            int total = groups.Total;
            if (total > 0)
            {
                group = GetGroupFromFeed(groups, groupNameToCreate);
            }
            else
            {
                group = CreateGroup(repository, groupNameToCreate);
            }

            if (printResult)
            {
                Console.WriteLine(group.ToString());
            }
            return group;
        }

        private static Group GetGroupFromFeed(Feed<Group> groups, string groupNameToCreate)
        {
            Console.WriteLine("\n\tSkip creating group, as there is already a group whose group_name is '" + groupNameToCreate + "' in server");
            List<Entry<Group>> entries = groups.Entries;
            if (entries.Count == 1)
            {
                return entries.ElementAt(0).Content;
            }
            else
            {
                return groups.FindInlineEntry(groupNameToCreate);
            }
        }

        private static Group CreateGroup(Repository repository, string groupNameToCreate)
        {
            Console.WriteLine("\n\t\tCreates a group: '" + groupNameToCreate + "'");
            Group toCreatedGroup = new Group();
            toCreatedGroup.SetPropertyValue("group_name", groupNameToCreate);

            return repository.CreateGroup(toCreatedGroup, null);
        }

        private static Group UpdateGroup(Group group, bool printResult)
        {
            object nameObject;

            group.Properties.TryGetValue("owner_name", out nameObject);
            string ownerName = nameObject.ToString();

            Console.WriteLine("\n\tUpdates the group's owner_name to 'Adminstrator' from '" + ownerName + "'");
            Group toUpdateGroup = new Group();
            toUpdateGroup.SetPropertyValue("owner_name", "Administrator");
            Group updatedGroup = group.Update(toUpdateGroup, null);

            if (printResult) Console.WriteLine(updatedGroup.ToString());
            return updatedGroup;
        }

        private static void DeleteGroup(Group group)
        {
            object nameObject;
            bool hasValue = group.Properties.TryGetValue("group_name", out nameObject);
            string groupName = nameObject.ToString();
            if (isContinue("\nPress 'q' to exit, or any other key to delete group '" + groupName + "'..", 'q'))
            {
                group.Delete();
                Console.WriteLine("\n\tDeleted the group '" + groupName + "'");
            }
            else
            {
                return;
            }
        }

        private static void GetParentGroups(Group group, int itemsPerPage, bool pauseBetweenPages, bool printResult)
        {
            string groupName = group.GetPropertyValue("group_name").ToString();
            Console.WriteLine(String.Format("\nGetting parent groups for group '{0}'", groupName));
            // REST call to get the 1st page of groups 
            FeedGetOptions options = new FeedGetOptions() { ItemsPerPage = itemsPerPage, Inline = true, IncludeTotal = true };
            Feed<Group> groups = group.GetParentGroups<Group>(options);
            printFeed(groups, pauseBetweenPages, printResult, "There is no parent group for group '" + groupName + "'");
        }

        private static void GetGroupUsers(Group group, int itemsPerPage, bool pauseBetweenPages, bool printResult)
        {
            string groupName = group.GetPropertyValue("group_name").ToString();
            Console.WriteLine("\n\tGets the group users of group'" + groupName + "'..");
            // REST call to get the 1st page of users 
            FeedGetOptions options = new FeedGetOptions() { ItemsPerPage = itemsPerPage, Inline = true, IncludeTotal = true };
            Feed<User> users = group.GetGroupUsers<User>(options);
            printFeed(users, pauseBetweenPages, printResult, "There is no users under group '" + groupName + "'");
        }

        private static void GetGroupGroups(Group group, int itemsPerPage, bool pauseBetweenPages, bool printResult)
        {
            string groupName = group.GetPropertyValue("group_name").ToString();
            Console.WriteLine("\n\tGets the sub groups of group'" + groupName + "'..");
            // REST call to get the 1st page of groups 
            FeedGetOptions options = new FeedGetOptions() { ItemsPerPage = itemsPerPage, Inline = true, IncludeTotal = true };
            Feed<Group> groups = group.GetGroupGroups<Group>(options);
            printFeed(groups, pauseBetweenPages, printResult, "There is no groups under group '" + groupName + "'");
        }

        private static void printFeed<T>(Feed<T> feed, bool pauseBetweenPages, bool printResult, string emptyEntryOutput)
        {
            if (feed != null)
            {
                int totalResults = feed.Total;
                double totalPages = feed.PageCount;
                int docProcessed = 0;

                if (totalResults == 0)
                {
                    Console.WriteLine(emptyEntryOutput);
                    return;
                }

                for (int i = 0; i < totalPages; i++)
                {
                    Console.WriteLine("**************************** PAGE " + (i + 1) + " *******************************");
                    foreach (Entry<T> obj in feed.Entries)
                    {
                        StringBuilder values = new StringBuilder();
                        Console.WriteLine(String.Format("Title: {0}, \tSummary: {1}",
                            obj.Title,
                            obj.Summary));
                        Console.WriteLine(values.ToString());
                        docProcessed++;
                    }

                    // REST call to get next page 
                    if (totalResults != docProcessed) feed = feed.NextPage();
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
            if (printResult) Console.WriteLine(feed == null ? "NULL" : feed.ToString());
        }
    }
}
