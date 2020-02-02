using AppServiceCosmosDB.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppServiceCosmosDB.DataService
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
        private const string _employeesContainerName = "Employees";
        private const string _companiesContainerName = "Companies";
        private string _cosmosDatabaseName;
        private string _cosmosServiceName;
        private string _cosmosServiceKey;
        private string _cosmosRegion;
        private Container _employees;
        private Container _companies;
        private CosmosClient _client;
        private ILogger<CosmosDBService> _logger;
        public CosmosDBService(
            IConfiguration configuration,
            ILogger<CosmosDBService> logger)
        {
            this._cosmosServiceName = configuration["COSMOS_SERVICENAME"] ?? ""; 
            this._cosmosServiceKey = configuration["COSMOS_KEY"] ?? ""; 
            this._cosmosDatabaseName = configuration["COSMOS_DATABASENAME"] ?? "testdb";
            this._cosmosRegion = configuration["COSMOS_REGION"] ?? "East US2"; ;

            this._client = null;
            this._employees = null;
            this._companies = null;
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
                    this._companies = this._client.GetContainer(this._cosmosDatabaseName, _companiesContainerName);
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
                    ContainerResponse c = await databaseResponse.Database.CreateContainerIfNotExistsAsync(_companiesContainerName, "/id");

                    if (((c.StatusCode == System.Net.HttpStatusCode.Created) || (c.StatusCode == System.Net.HttpStatusCode.OK)) &&
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
        async Task<ulong> GetCompaniesCount()
        {
            ulong result = 0;
            string sqlQueryText = "SELECT VALUE COUNT(1) FROM c";
            QueryDefinition queryDefinition = new QueryDefinition(sqlQueryText);
            FeedIterator<ulong> queryResultSetIterator = this._companies.GetItemQueryIterator<ulong>(queryDefinition);

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
                if (await GetEmployeesCount() == 0)
                {

                    await AddEmployeeAsync(
                                    new Employee
                                    {
                                        id = Guid.NewGuid().ToString(),
                                        address = "1 Seashore street",
                                        city = "Santa Cruz",
                                        country = "USA",
                                        employeeId = 1,
                                        firstName = "John",
                                        lastName = "Belize",
                                        zipCode = "13098"
                                    });

                    await AddEmployeeAsync(
                                    new Employee
                                    {
                                        id = Guid.NewGuid().ToString(),
                                        address = "1 Mountain street",
                                        city = "Salt Lake City",
                                        country = "USA",
                                        employeeId = 2,
                                        firstName = "Chris",
                                        lastName = "Cross",
                                        zipCode = "33098"
                                    });
                    await AddEmployeeAsync(
                                    new Employee
                                    {
                                        id = Guid.NewGuid().ToString(),
                                        address = "2 Field Mice street",
                                        city = "Baltimore",
                                        country = "USA",
                                        employeeId = 3,
                                        firstName = "Eve",
                                        lastName = "Mortadella",
                                        zipCode = "45678"
                                    }
                                    );

                    Console.WriteLine($"created Employees records");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception while creating tables: " + ex.Message);
            }

        }




        public async Task AddCompanyAsync(Company item)
        {
            try
            {
                await this._companies.CreateItemAsync<Company>(item, new PartitionKey(item.id));
            }
            catch(Exception ex)
            {
                _logger.LogError("Exception while adding a company: ", ex.Message);
            }
        }

        public async Task DeleteCompanyAsync(string id)
        {
            try
            {
                await this._companies.DeleteItemAsync<Company>(id, new PartitionKey(id));
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception while removing a company: ", ex.Message);
            }

        }

        public async Task<Company> GetCompanyAsync(string id)
        {
            try
            {
                ItemResponse<Company> response = await this._companies.ReadItemAsync<Company>(id, new PartitionKey(id));
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                  _logger.LogError("Exception while finding a company: ", ex.Message);
                return null;
            }

        }

        public async Task<IEnumerable<Company>> GetCompaniesAsync()
        {
            string queryString = "SELECT * FROM c";
            List<Company> results = new List<Company>();
            try
            {
                var query = this._companies.GetItemQueryIterator<Company>(new QueryDefinition(queryString));
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

        public async Task UpdateCompanyAsync(Company item)
        {
            try
            {
                await this._companies.UpsertItemAsync<Company>(item, new PartitionKey(item.id));
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception while updating a company: ", ex.Message);
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
