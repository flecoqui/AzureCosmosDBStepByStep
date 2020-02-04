namespace BulkImportData
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Text.Json;
    using System.Threading.Tasks;
    using Microsoft.Azure.Cosmos;

    public class Program
    {
        private static string VersionString = "1.0.0.0";
        private static string InformationCSAudioTool = "BulkImportData:\r\n" +
            "Bulk CosmosDB Import Data: {0} \r\n" + "Syntax:\r\n" +
            "BulkImportData --account <CosmosDBAccount> --key <CosmosDBKey> --count <NumberOfItemsToInsert>\r\n" +
            "               --database <CosmosDBDatabase> --container <CosmosDBContainer> \r\n" +
            "BulkImportData --help \r\n";

        static bool ParseCommandLine(    string[] args,
                                         out string Account,
                                         out string Key,
                                         out string Database,
                                         out string Container,
                                         out int Count
                                     )
        {
            bool bHelp = false;
            bool result = false;
            string ErrorMessage = string.Empty;
            Account = string.Empty;
            Key = string.Empty;
            Database = string.Empty;
            Container = string.Empty;
            Count = 300000;

            if ((args == null) || (args.Length == 0))
            {
                ErrorMessage = "No parameter in the command line";
            }
            else
            {
                int i = 0;
                while ((i < args.Length) && (string.IsNullOrEmpty(ErrorMessage)))
                {

                    switch (args[i++])
                    {

                        case "--help":
                            bHelp = true;
                            break;
                        case "--account":
                            if ((i < args.Length) && (!string.IsNullOrEmpty(args[i])))
                                Account = args[i++];
                            else
                                ErrorMessage = "Account not set";
                            break;
                        case "--key":
                            if ((i < args.Length) && (!string.IsNullOrEmpty(args[i])))
                                Key = args[i++];
                            else
                                ErrorMessage = "Key not set";
                            break;
                        case "--database":
                            if ((i < args.Length) && (!string.IsNullOrEmpty(args[i])))
                                Database = args[i++];
                            else
                                ErrorMessage = "Database not set";
                            break;
                        case "--container":
                            if ((i < args.Length) && (!string.IsNullOrEmpty(args[i])))
                                Container = args[i++];
                            else
                                ErrorMessage = "Container not set";
                            break;
                        case "--count":
                            if ((i < args.Length) && (!string.IsNullOrEmpty(args[i])))
                                int.TryParse(args[i++], out Count);
                            else
                                ErrorMessage = "Duration not set";
                            break;
                        default:

                            if ((args[i - 1].ToLower() == "dotnet") ||
                                (args[i - 1].ToLower() == "bulkimportdata.dll") ||
                                (args[i - 1].ToLower() == "bulkimportdata.exe"))
                                break;

                            ErrorMessage = "wrong parameter: " + args[i - 1];
                            break;
                    }

                }
            }
            if ((!string.IsNullOrEmpty(Account)) &&
                (!string.IsNullOrEmpty(Database)) &&
                (!string.IsNullOrEmpty(Container)) &&
                (!string.IsNullOrEmpty(Key)))
                result = true;
            else
                ErrorMessage = "Missing parameter(s) to launch the features";


            if ((bHelp) || (!string.IsNullOrEmpty(ErrorMessage)))
            {
                if (!string.IsNullOrEmpty(ErrorMessage))
                    Console.WriteLine(ErrorMessage);
                Console.WriteLine(string.Format(InformationCSAudioTool, VersionString));
            }

            return result;
        }
        static async Task Main(string[] args)
        {
            string DatabaseName = "bulk-tutorial";
            string ContainerName = "items";
            string Account;
            string Key;
            int ItemsToInsert = 300000;
            if (ParseCommandLine(args, out Account, out Key, out DatabaseName, out ContainerName, out ItemsToInsert))
            {

                string EndpointUrl = $"https://{Account}.documents.azure.com:443/";
                string AuthorizationKey = Key;
                CosmosClient cosmosClient = new CosmosClient(EndpointUrl, AuthorizationKey, new CosmosClientOptions() { AllowBulkExecution = true });



                // Create with a throughput of 50000 RU/s
                // Indexing Policy to exclude all attributes to maximize RU/s usage
                Console.WriteLine("This sample app will create a 50000 RU/s container, press any key to continue.");
                Console.ReadKey();



                // <Initialize>
                Database database = await cosmosClient.CreateDatabaseIfNotExistsAsync(DatabaseName);

                await database.DefineContainer(ContainerName, "/pk")
                        .WithIndexingPolicy()
                            .WithIndexingMode(IndexingMode.Consistent)
                            .WithIncludedPaths()
                                .Attach()
                            .WithExcludedPaths()
                                .Path("/*")
                                .Attach()
                        .Attach()
                    .CreateAsync(50000);
                // </Initialize>



                try
                {
                    // Prepare items for insertion
                    Console.WriteLine($"Preparing {ItemsToInsert} items to insert...");
                    // <Operations>

                    Dictionary<PartitionKey, Stream> itemsToInsert = new Dictionary<PartitionKey, Stream>(ItemsToInsert);
                    foreach (Item item in Program.GetItemsToInsert(ItemsToInsert))
                    {
                        MemoryStream stream = new MemoryStream();
                        await JsonSerializer.SerializeAsync(stream, item);
                        itemsToInsert.Add(new PartitionKey(item.pk), stream);
                    }

                    // </Operations>



                    // Create the list of Tasks
                    Console.WriteLine($"Starting...");
                    Stopwatch stopwatch = Stopwatch.StartNew();

                    // <ConcurrentTasks>
                    Container container = database.GetContainer(ContainerName);
                    List<Task> tasks = new List<Task>(ItemsToInsert);

                    foreach (KeyValuePair<PartitionKey, Stream> item in itemsToInsert)
                    {
                        tasks.Add(container.CreateItemStreamAsync(item.Value, item.Key)
                            .ContinueWith((Task<ResponseMessage> task) =>
                            {
                                using (ResponseMessage response = task.Result)
                                {
                                    if (!response.IsSuccessStatusCode)
                                    {
                                        Console.WriteLine($"Received {response.StatusCode} ({response.ErrorMessage}) status code for operation {response.RequestMessage.RequestUri.ToString()}.");
                                    }
                                }
                            }));
                    }



                    // Wait until all are done
                    await Task.WhenAll(tasks);
                    // </ConcurrentTasks>

                    stopwatch.Stop();

                    Console.WriteLine($"Finished in writing {ItemsToInsert} items in {stopwatch.Elapsed}.");
                }

                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

                finally
                {
                    Console.WriteLine("This sample app will delete the database, press any key to continue.");
                    Console.ReadKey();
                    Console.WriteLine("Cleaning up resources...");
                    await database.DeleteAsync();
                }
            }
        }



        private static IReadOnlyCollection<Item> GetItemsToInsert(int count)
        {
            return new Bogus.Faker<Item>()
            .StrictMode(true)
            //Generate item
            .RuleFor(o => o.id, f => Guid.NewGuid().ToString()) //id
            .RuleFor(o => o.username, f => f.Internet.UserName())
            .RuleFor(o => o.pk, (f, o) => o.id) //partitionkey
            .Generate(count);
        }
        // </Bogus>



        // <Model>
        public class Item
        {
            public string id { get; set; }
            public string pk { get; set; }
            public string username { get; set; }
        }
        // </Model>

    }
}
