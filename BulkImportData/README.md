# Sample .NET CosmosDB Bulk Import Application used to optimize the CosmosDB provisioned throughput (RU/s)
This sample application is used to test the Cosmos DB Bulk Import API. 

## Building the application BulkImportData:

1. Build the project 

        dotnet build

2. Run

        BulkImportData --account <CosmosDBAccount> --key <CosmosDBKey> --count <NumberOfItemsToInsert>
            "          --database <CosmosDBDatabase> --container <CosmosDBContainer> 




## BulkImportData under the hood:

### Configuration
Before running the application you need to edit the file AppSettings.json and the set the values: 
COSMOS_SERVICENAME with the url of the Cosmos DB service 
COSMOS_KEY with the key associated with the Cosmos DB service 
COSMOS_DATABASENAME with the name of the database
COSMOS_REGION with the name of the region where the Web Application is installed. This information is useful if the database is geo replicated in  several regions.

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "COSMOS_SERVICENAME": "https://<cosmosdb>.documents.azure.com:443/",
  "COSMOS_KEY": "",
  "COSMOS_DATABASENAME": "testdb",
  "COSMOS_REGION": "East US 2",

  "AllowedHosts": "*"
}

```

### Creating the database
The Application creates the database if not exists. 

```csharp

        Database database = await cosmosClient.CreateDatabaseIfNotExistsAsync(DatabaseName);

```
### Creating the container with a throughput of 50000 RU/s
The Application creates the container for the test with a throughput of 50000 RU/s. 

```csharp

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

```

### Preparing the data to insert
Before inserting the new items in the database, the data needs to be prepared. 
The data model: 

```csharp

        public class Item
        {
            public string id { get; set; }
            public string pk { get; set; }
            public string username { get; set; }
        }

```
Creating the Items collection to insert with Bogus:

```csharp

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

```

Creating the Dictionary of Memory Streams which will be used to insert items:

```csharp

            Dictionary<PartitionKey, Stream> itemsToInsert = new Dictionary<PartitionKey, Stream>(ItemsToInsert);
            foreach (Item item in Program.GetItemsToInsert(ItemsToInsert))
            {
                MemoryStream stream = new MemoryStream();
                await JsonSerializer.SerializeAsync(stream, item);
                itemsToInsert.Add(new PartitionKey(item.pk), stream);
            }

```


The main loop with the list of tasks:

```csharp
        
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
```
