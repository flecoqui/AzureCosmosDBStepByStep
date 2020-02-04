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

5. Edit the configuration file appsettings.json to specify the Cosmos DB service Account name and Key. Moreover, if you want to run the application locally on your machine configure the Cosmos DB service firewall to support your local IP address.

        "COSMOS_SERVICENAME": "https://<cosmosdb>.documents.azure.com:443/",
        "COSMOS_KEY": "",
        "COSMOS_DATABASENAME": "testdb",

6. Build the project 

        dotnet build

7. Run

        dotnet run

8. Open the Application pages with your browser:

        open url https://localhost:5001/



## AppServiceEFCosmosDB under the hood:

### Configuration
Before running the application you need to edit the file AppSettings.json and the set the values: 
COSMOS_SERVICENAME with the url of the Cosmos DB service 
COSMOS_KEY with the key associated with the Cosmos DB service 
COSMOS_DATABASENAME with the name of the database

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

  "AllowedHosts": "*"
}

```

### Running the application without CosmosDB service using in Memory database
If the configuration value COSMOS_SERVICENAME is an empty string the Application will create an In Memory Database instead of using the Cosmos DB service.
This approach is useful if you want to debug your Web App without Internet connection. 

```csharp

            string CosmosService = AppSettings.CosmosServiceName;
            if (string.IsNullOrEmpty(CosmosService))
                services.AddDbContext<CosmosDBContext>(options => options.UseInMemoryDatabase(databaseName: AppSettings.CosmosDatabaseName));
            else
                services.AddDbContext<CosmosDBContext>(options => options.UseCosmos(AppSettings.CosmosServiceName, AppSettings.CosmosServiceKey, AppSettings.CosmosDatabaseName));
            services.AddTransient<CosmosDBService>();

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
                //3. Get the instance of database in our services layer
                var services = scope.ServiceProvider;
                var service = services.GetRequiredService<CosmosDBService>();
                await service.CreateTheDatabaseAsync();
                await service.WriteTablesAsync();
            }

```

Creating database if not exists:

```csharp

        public async Task CreateTheDatabaseAsync()
        {
            var created = false;
            try
            {
                created = await _context.Database.EnsureCreatedAsync();
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
        }

```

Creating database containers if not exist:

```csharp

        public class CosmosDBContext : DbContext
        {
            public CosmosDBContext(DbContextOptions<CosmosDBContext> options)
                : base(options)
            {
            }
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Models.Company>().ToContainer("Companies");
                modelBuilder.Entity<Models.Employee>().ToContainer("Employees");

            }
            public DbSet<Models.Company> Companies { get; set; }
            public DbSet<Models.Employee> Employees { get; set; }
        }


```

Populating database containers if empty:

```csharp
        
        public async Task WriteTablesAsync()
        {
            try
            {
                if (_context.Companies.LongCount() == 0)
                {
                    _context.Companies.AddRange(
                                    new Company
                                    {
                                        id = Guid.NewGuid().ToString(),
                                        companyId = 1,
                                        name = "MotherCompany",
                                        address = "One Mother Company Way",
                                        zipCode = "29330",
                                        city = "Antananarivo",
                                        country = "Madagascar"
                                    },
                                    new Company
                                    {
                                        id = Guid.NewGuid().ToString(),
                                        companyId = 2,
                                        name = "FirstCompany",
                                        address = "One First Company Way",
                                        zipCode = "22330",
                                        city = "Bilbao",
                                        country = "Spain"
                                    }); ;
                    int changed = await _context.SaveChangesAsync();
                    Console.WriteLine($"created Companies {changed} records");
                }


```