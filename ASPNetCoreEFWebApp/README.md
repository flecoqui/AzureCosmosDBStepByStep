# Sample ASP.NET core using Azure Cosmos DB service with Entity Framework Core 


## Building the application AppServiceEFCosmosDB:

1. Install .Net Core SDK 3.1.101</p>
The SDK can be downloaded from there:</p> 
https://dotnet.microsoft.com/download/dotnet-core/3.1

2. Check the installation
Enter the following commands with your operating system shell:

        dotnet --list-sdks
        dotnet --version

3. Clone the current repository on your machine

        git clone https://github.com/flecoqui/AzureCosmosDBStepByStep.git

4. Change directory to navigate into the project folder

        cd AzureCosmosDBStepByStep\AppServiceCosmosDB\ASPNetCoreEFWebApp

5. Edit the configuration appsettings.json to specify the Cosmos DB service and configure the Cosmos DB service firewall to support your local IP address.

        "COSMOS_SERVICENAME": "https://<cosmosdb>.documents.azure.com:443/",
        "COSMOS_KEY": "",
        "COSMOS_DATABASENAME": "testdb",

6. Build the project 

        dotnet build

7. Run

        dotnet run

8. Open the Application pages with your browser:

        open url https://localhost:5001/


