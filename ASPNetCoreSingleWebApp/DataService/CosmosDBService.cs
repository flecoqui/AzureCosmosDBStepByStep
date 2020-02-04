using AppServiceSingleCosmosDB.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppServiceSingleCosmosDB.DataService
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Models;
    using Microsoft.Azure.Cosmos;
    using Microsoft.Azure.Cosmos.Fluent;
    using Microsoft.Extensions.Configuration;

    public class CosmosDBService : ICosmosDBService
    {
        private const string _employeesContainerName = "persondata";
        private const string _companiesContainerName = "Companies";
        private string _cosmosDatabaseName;
        private string _cosmosServiceName;
        private string _cosmosServiceKey;
        private string _cosmosRegion;
        private Container _employees;
        private CosmosClient _client;
        private ILogger<CosmosDBService> _logger;

        private string[] regionArray = {
                Regions.WestUS,
                Regions.UKSouth,
                Regions.BrazilSouth,
                Regions.USGovArizona,
                Regions.USGovTexas,
                Regions.USGovVirginia,
                Regions.EastUS2EUAP,
                Regions.CentralUSEUAP,
                Regions.FranceCentral,
                Regions.FranceSouth,
                Regions.USDoDCentral,
                Regions.USDoDEast,
                Regions.AustraliaCentral,
                Regions.AustraliaCentral2,
                Regions.SouthAfricaNorth,
                Regions.SouthAfricaWest,
                Regions.UAECentral,
                Regions.UAENorth,
                Regions.USNatEast,
                Regions.USNatWest,
                Regions.USSecEast,
                Regions.USSecWest,
                Regions.SwitzerlandNorth,
                Regions.SwitzerlandWest,
                Regions.GermanyNorth,
                Regions.GermanyWestCentral,
                Regions.UKWest,
                Regions.NorwayEast,
                Regions.KoreaCentral,
                Regions.ChinaEast2,
                Regions.WestUS2,
                Regions.WestCentralUS,
                Regions.EastUS,
                Regions.EastUS2,
                Regions.CentralUS,
                Regions.SouthCentralUS,
                Regions.NorthCentralUS,
                Regions.WestEurope,
                Regions.NorthEurope,
                Regions.EastAsia,
                Regions.SoutheastAsia,
                Regions.JapanEast,
                Regions.JapanWest,
                Regions.AustraliaEast,
                Regions.AustraliaSoutheast,
                Regions.CentralIndia,
                Regions.SouthIndia,
                Regions.WestIndia,
                Regions.CanadaEast,
                Regions.CanadaCentral,
                Regions.GermanyCentral,
                Regions.GermanyNortheast,
                Regions.ChinaNorth,
                Regions.ChinaEast,
                Regions.ChinaNorth2,
                Regions.KoreaSouth,
                Regions.NorwayWest
            };
        public string GetCosmosRegion(string region)
        {
            foreach (string s in regionArray)
            {
                if (s.Equals(region, StringComparison.InvariantCultureIgnoreCase))
                    return s;
            }
            foreach (string s in regionArray)
            {
                string r = s.Replace(" ", "");
                if (r.Equals(region, StringComparison.InvariantCultureIgnoreCase))
                    return s;
            }

            return "East US 2";
        }
        public CosmosDBService(
            IConfiguration configuration,
            ILogger<CosmosDBService> logger)
        {
            this._cosmosServiceName = configuration["COSMOS_SERVICENAME"] ?? ""; 
            this._cosmosServiceKey = configuration["COSMOS_KEY"] ?? ""; 
            this._cosmosDatabaseName = configuration["COSMOS_DATABASENAME"] ?? "eadatabase";
            this._cosmosRegion = configuration["COSMOS_REGION"] ?? "East US2"; ;

            this._client = null;
            this._employees = null;
            InitializeCosmosClient();
            _logger = logger;
        }
        public bool InitializeCosmosClient()
        {
            bool result = false;
            try
            {
                
                CosmosClientBuilder clientBuilder = new CosmosClientBuilder(this._cosmosServiceName, this._cosmosServiceKey);
                this._client = clientBuilder
                                    .WithConnectionModeDirect()
                                    .WithApplicationRegion(this._cosmosRegion)
                                    .Build();
                if (this._client != null)
                {
                    this._employees = this._client.GetContainer(this._cosmosDatabaseName, _employeesContainerName);
                    result = true;
                }
            }
            catch(Exception ex)
            {
                _logger.LogError("Exception while initializing the Cosmos DB Client : ", ex.Message);
            }
            return result;
        }
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
                    if ((e.StatusCode == System.Net.HttpStatusCode.Created) || (e.StatusCode == System.Net.HttpStatusCode.OK))
                    {
                        this._employees = this._client.GetContainer(this._cosmosDatabaseName, _employeesContainerName);
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

        async Task<ulong> GetEmployeesCount()
        {
            ulong result = 0;
            string sqlQueryText = "SELECT VALUE COUNT(1) FROM c";
            QueryDefinition queryDefinition = new QueryDefinition(sqlQueryText);
            FeedIterator<ulong> queryResultSetIterator = this._employees.GetItemQueryIterator<ulong>(queryDefinition);

            while (queryResultSetIterator.HasMoreResults)
            {
                FeedResponse<ulong> currentResultSet = await queryResultSetIterator.ReadNextAsync();
                foreach (ulong count in currentResultSet)
                {
                    result = count;
                }
            }
            return result;
        }
        public async Task WriteTablesAsync()

        {
            try
            {
                if (await GetEmployeesCount() == 0)
                {


                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception while creating tables: " + ex.Message);
            }

        }





        public async Task AddEmployeeAsync(Employee item)
        {
            try
            {
                await this._employees.CreateItemAsync<Employee>(item, new PartitionKey(item.id));
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception while adding a company: ", ex.Message);
            }
        }

        public async Task DeleteEmployeeAsync(string id)
        {
            try
            {
                await this._employees.DeleteItemAsync<Employee>(id, new PartitionKey(id));
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception while removing a company: ", ex.Message);
            }

        }

        public async Task<Employee> GetEmployeeAsync(string id)
        {
            try
            {
                ItemResponse<Employee> response = await this._employees.ReadItemAsync<Employee>(id, new PartitionKey(id));
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                _logger.LogError("Exception while finding a company: ", ex.Message);
                return null;
            }

        }

        public async Task<IEnumerable<Employee>> GetEmployeesAsync()
        {
            string queryString = "SELECT * FROM c";
            List<Employee> results = new List<Employee>();
            try
            {
                var query = this._employees.GetItemQueryIterator<Employee>(new QueryDefinition(queryString));
                while (query.HasMoreResults)
                {
                    var response = await query.ReadNextAsync();

                    results.AddRange(response.ToList());
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception while getting the list of company: ", ex.Message);
            }
            return results;
        }

        public async Task UpdateEmployeeAsync(Employee item)
        {
            try
            {
                await this._employees.UpsertItemAsync<Employee>(item, new PartitionKey(item.id));
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception while updating a company: ", ex.Message);
            }
        }

    }
}
