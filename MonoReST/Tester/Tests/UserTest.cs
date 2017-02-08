using Emc.Documentum.Rest.DataModel;
using Emc.Documentum.Rest.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Emc.Documentum.Rest.Test
{
    public class UserTest
    {
        public static void Run(RestController client, string RestHomeUri, string filter, int itemsPerPage, bool pauseBetweenPages, string repositoryName, bool printResult, string userName)
        {
            // get repository resource
            HomeDocument home = client.Get<HomeDocument>(RestHomeUri, null);
            Feed<Repository> repositories = home.GetRepositories<Repository>(new FeedGetOptions { Inline = true, Links = true });
            Repository repository = repositories.FindInlineEntry(repositoryName);

            // get a collection of users
            GetUsers(repository, filter, itemsPerPage, pauseBetweenPages, printResult);

            OperationsOnUser(repository, userName, itemsPerPage, pauseBetweenPages, printResult);
        }

        private static void OperationsOnUser(Repository repository, string userName, int itemsPerPage, bool pauseBetweenPages, bool printResult)
        {
            //create or get user
            User user = CreateOrGetUser(repository, userName, printResult);
            if (user == null)
            {
                return;
            }
            RepeatableOperations(user, itemsPerPage, pauseBetweenPages, printResult);
            DeleteUser(user);
        }

        private static User CreateOrGetUser(Repository repository, string userName, bool printResult)
        {
            User user;
            if (String.IsNullOrWhiteSpace(userName))
            {
                userName = "Net_Sample_Test_User" + new Random().Next();
                if (!isContinue("Press 'q' to exit, or any other key to create user '" + userName + "' for following tests..", 'q'))
                {
                    return null;
                }
                user = CreateUser(repository, userName);
            }
            else
            {

                if (!isContinue("Press 'q' to exit, or any other key to create/retrieve the user '" + userName + "'..", 'q'))
                {
                    return null;
                }

                // create user if the userName does not exist
                user = CreateUserIfNeed(repository, printResult, userName);
            }
            return user;
        }

        private static void RepeatableOperations(User user, int itemsPerPage, bool pauseBetweenPages, bool printResult)
        {
            do
            {
                Console.WriteLine("\nPress 'e' to edit user , 'f' to get user's default folder,'p' to get parent groups, 'q' to back to previous layer, or any other key run all tests..");
                Console.ForegroundColor = ConsoleColor.Yellow;
                ConsoleKeyInfo next = Console.ReadKey();
                Console.ForegroundColor = ConsoleColor.White;
                string keyword = next.KeyChar.ToString();

                switch (keyword)
                {
                    case "e":
                        user = UpdateUser(user, printResult);
                        break;
                    case "f":
                        GetDefaultFolder(user, printResult);
                        break;
                    case "p":
                        GetParentGroups(user, itemsPerPage, pauseBetweenPages, printResult);
                        break;
                    case "q":
                        Console.WriteLine("\n");
                        return;
                    default:
                        user = UpdateUser(user, printResult);
                        GetDefaultFolder(user, printResult);
                        GetParentGroups(user, itemsPerPage, pauseBetweenPages, printResult);
                        break;
                }
            } while (true);
        }

        private static void GetUsers(Repository repository, string filter, int itemsPerPage, bool pauseBetweenPages, bool printResult)
        {
            // REST call to get the 1st page of users 
            FeedGetOptions options = new FeedGetOptions() { ItemsPerPage = itemsPerPage, Inline = true, IncludeTotal = true, Filter = (String.IsNullOrEmpty(filter) || filter.Equals("null")) ? null : filter };
            Feed<User> users = repository.GetUsers<User>(options);

            printFeed(users, pauseBetweenPages, printResult, "There is exception to get users in repository '" + repository.Name + "'");
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

        private static User CreateUserIfNeed(Repository repository, bool printResult, string userNameToCreate)
        {
            string _defaultUserName = "Net_Sample_Test_User" + new Random().Next();
            if (String.IsNullOrEmpty(userNameToCreate))
            {
                userNameToCreate = _defaultUserName;
            }

            string filter = "user_name='" + userNameToCreate + "' or user_login_name='" + userNameToCreate + "'";
            FeedGetOptions options = new FeedGetOptions() { ItemsPerPage = 10, Inline = true, IncludeTotal = true, Filter = filter };
            Feed<User> users = repository.GetUsers<User>(options);
            User user;
            int total = users.Total;
            if (total > 0)
            {
                user = GetUserFromFeed(users, userNameToCreate);
            }
            else
            {
                user = CreateUser(repository, userNameToCreate);
            }

            if (printResult)
            {
                Console.WriteLine(user.ToString());
            }
            return user;
        }

        private static User GetUserFromFeed(Feed<User> users, string userNameToCreate)
        {
            Console.WriteLine("\n\tSkip creating user, as there is already a user whose user_name or/and user_login_name is '" + userNameToCreate + "' in server");
            List<Entry<User>> entries = users.Entries;
            if (entries.Count == 1)
            {
                return entries.ElementAt(0).Content;
            }
            else
            {
                return users.FindInlineEntry(userNameToCreate);
            }
        }

        private static User CreateUser(Repository repository, string userNameToCreate)
        {
            Console.WriteLine("\n\t\tCreates a user with the same user_name and user_login_name: '" + userNameToCreate + "'");
            User toCreatedUser = new User();
            toCreatedUser.SetPropertyValue("user_name", userNameToCreate);
            toCreatedUser.SetPropertyValue("user_login_name", userNameToCreate);
            toCreatedUser.SetPropertyValue("default_folder", "/" + userNameToCreate);

            return repository.CreateUser(toCreatedUser, null);
        }

        private static User UpdateUser(User user, bool printResult)
        {
            object nameObject;

            user.Properties.TryGetValue("user_login_name", out nameObject);
            string loginName = nameObject.ToString();
            string newLoginName = "updated_" + loginName;

            Console.WriteLine("\n\tUpdates the user's user_login_name to '" + newLoginName + "' from '" + loginName + "'");
            User toUpdateUser = new User();
            toUpdateUser.SetPropertyValue("user_login_name", newLoginName);
            User updatedUser = user.Update(toUpdateUser, null);

            if (printResult) Console.WriteLine(updatedUser.ToString());
            return user;
        }

        private static void DeleteUser(User user)
        {
            object nameObject;
            bool hasValue = user.Properties.TryGetValue("user_name", out nameObject);
            string userName = nameObject.ToString();
            if (isContinue("\nPress 'q' to exit, or any other key to delete user '" + userName + "'..", 'q'))
            {
                user.Delete();
                Console.WriteLine("\n\tDeleted the user '" + userName + "'");
            }
            else
            {
                return;
            }
        }

        private static void GetParentGroups(User user, int itemsPerPage, bool pauseBetweenPages, bool printResult)
        {
            string userName = user.GetPropertyValue("user_name").ToString();
            Console.WriteLine(String.Format("\nGetting parent groups for user '{0}'", userName));
            // REST call to get the 1st page of groups 
            FeedGetOptions options = new FeedGetOptions() { ItemsPerPage = itemsPerPage, Inline = true, IncludeTotal = true };
            Feed<Group> groups = user.GetParentGroups<Group>(options);
            printFeed(groups, pauseBetweenPages, printResult, "There is no parent group for user '" + userName + "'");
        }

        private static void GetDefaultFolder(User user, bool printResult)
        {
            Console.WriteLine("\n\tGets the home cabinet of user'" + user.GetPropertyString("user_name") + "'..");
            SingleGetOptions options = new SingleGetOptions();
            options.View = ":default";
            options.Links = true;
            Folder defaultFolder = user.GetHomeCabinet(options);
            if (printResult)
                Console.WriteLine(defaultFolder == null ? "NULL" : defaultFolder.ToString());
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
                        Console.WriteLine(String.Format("Name: {0}, \tSummary: {1}",
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
