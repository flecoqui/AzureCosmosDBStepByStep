# Sample ASP.NET core using Azure Cosmos DB service with Cosmos .Net SDK 
The Sample application uses Cosmos DB service with two containers: Companies, Employees

## Building the application AppServiceCosmosDB:

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

        cd AzureCosmosDBStepByStep\AppServiceCosmosDB\ASPNetCoreWebApp

5. Edit the configuration file appsettings.json to specify the Cosmos DB service Account name and Key. Moreover, if you want to run the application locally on your machine configure the Cosmos DB service firewall to support your local IP address.

        "COSMOS_SERVICENAME": "https://<cosmosdb>.documents.azure.com:443/",
        "COSMOS_KEY": "",
        "COSMOS_DATABASENAME": "testdb",
        "COSMOS_REGION": "East US 2",

6. Build the project 

        dotnet build

7. Run

        dotnet run

8. Open the Application pages with your browser:

        open url https://localhost:5001/





## AppServiceCosmosDB under the hood:

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

### Using the CosmosDBService 
The Class CosmosDBService provide an access to the CosmosDB database. 

```csharp

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddControllersWithViews();            
            services.AddSingleton<CosmosDBService>();
        }

```
### Running the application with sample data
When the Web Application starts, if the database doesn't exist, it will create the database.
If the database containers (Companies, Employees) are empty, it will populate the database containers.

Creating and populating database:

```csharp

            //1. Get the IWebHost which will host this application.
            var host = CreateHostBuilder(args).Build();
            //2. Find the service layer within our scope.
            using (var scope = host.Services.CreateScope())
            {
                //3. Get the instance of QartDBContext in our services layer
                var services = scope.ServiceProvider;
                var service = services.GetRequiredService<CosmosDBService>();
                await service.CreateTheDatabaseAsync();
                await service.WriteTablesAsync();
            }

```

Creating database if not exists, creating containers if not exists:

```csharp

        public async Task<bool> CreateTheDatabaseAsync()
        {
            bool created = false;
            try
            {
                if(this._client==null)
                    InitializeCosmosClient();
                DatabaseResponse databaseResponse = await _client.CreateDatabaseIfNotExistsAsync(_cosmosDatabaseName);
                if (databaseResponse != null)
                {
                    ContainerResponse e =  await databaseResponse.Database.CreateContainerIfNotExistsAsync(_employeesContainerName, "/id");
                    ContainerResponse c = await databaseResponse.Database.CreateContainerIfNotExistsAsync(_companiesContainerName, "/id");

                    if (((e.StatusCode == System.Net.HttpStatusCode.Created) || (e.StatusCode == System.Net.HttpStatusCode.OK)) &&
                        ((c.StatusCode == System.Net.HttpStatusCode.Created) || (c.StatusCode == System.Net.HttpStatusCode.OK)))
                    {
                        this._employees = this._client.GetContainer(this._cosmosDatabaseName, _employeesContainerName);
                        this._companies = this._client.GetContainer(this._cosmosDatabaseName, _companiesContainerName);
                        created = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("database exception : " + ex.Message);

            }
            if (created)
            {
                Console.WriteLine("database created");
            }
            else
            {
                Console.WriteLine("database already exists");
            }
            return created;
        }

```


Populating database containers if empty:

```csharp
        
        public async Task WriteTablesAsync()

        {
            try
            {
                if (await GetCompaniesCount() == 0)
                {
                    await AddCompanyAsync(
                                    new Company
                                    {
                                        id = Guid.NewGuid().ToString(),
                                        companyId = 1,
                                        name = "MotherCompany",
                                        address = "One Mother Company Way",
                                        zipCode = "29330",
                                        city = "Antananarivo",
                                        country = "Madagascar"
                                    });
                    
                    await AddCompanyAsync(
                                    new Company
                                    {
                                        id = Guid.NewGuid().ToString(),
                                        companyId = 2,
                                        name = "FirstCompany",
                                        address = "One First Company Way",
                                        zipCode = "22330",
                                        city = "Bilbao",
                                        country = "Spain"
                                    }); 
                    Console.WriteLine($"created Companies records");
                }


```