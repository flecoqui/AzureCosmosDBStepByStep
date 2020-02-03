# Sample .NET CosmosDB Bulk Import Application used to optimize the CosmosDB provisioned throughput (RU/s


## Building the application BulkImportData:

1. Build the project 

        dotnet build

2. Run

        BulkImportData --account <CosmosDBAccount> --key <CosmosDBKey> --count <NumberOfItemsToInsert>
            "          --database <CosmosDBDatabase> --container <CosmosDBContainer> 





