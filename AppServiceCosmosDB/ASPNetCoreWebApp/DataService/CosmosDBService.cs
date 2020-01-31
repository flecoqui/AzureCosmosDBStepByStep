using AppServiceEFCosmosDB.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppServiceEFCosmosDB.DataService
{
    public class CosmosDBService
    {
        private readonly ILogger<CosmosDBService> _logger;
        private object dblock;
        private readonly CosmosDBContext _context;

        public CosmosDBService(ILogger<CosmosDBService> Logger, CosmosDBContext Context)
        {
            _logger = Logger ?? throw new ArgumentNullException(nameof(Logger));
            _context = Context ?? throw new ArgumentNullException(nameof(Context));
            dblock = new object();
        }

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
                if (_context.Employees.LongCount() == 0)
                {

                    _context.Employees.AddRange(
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
                                    },
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
                                    },
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

                    int changed = await _context.SaveChangesAsync();
                    Console.WriteLine($"created Employees {changed} records");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception while creating tables: " + ex.Message);
            }

        }

        public async Task AddCompanyAsync(Company item)
        {
            //await this._context.CreateCompanyAsync<Company>(item, new PartitionKey(item.Id));
            try
            {
                await this._context.Companies.AddAsync(item);
                await _context.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                _logger.LogError("Exception while adding a company: ", ex.Message);
            }
        }

        public async Task DeleteCompanyAsync(string id)
        {
            //await this._context.DeleteCompanyAsync<Company>(id, new PartitionKey(id));
            Company c = await GetCompanyAsync(id);
            try
            {
                if (c != null)
                {
                    this._context.Companies.Remove(c);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception while removing a company: ", ex.Message);
            }

        }

        public async Task<Company> GetCompanyAsync(string id)
        {
            //try
            //{
            //    CompanyResponse<Company> response = await this._context.ReadCompanyAsync<Company>(id, new PartitionKey(id));
            //    return response.Resource;
            //}
            //catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            //{
            //    return null;
            //}
            try
            {
                return await this._context.Companies.FindAsync(id);
            }
            catch(Exception ex)
            {
                _logger.LogError("Exception while finding a company: ", ex.Message);
                return null;
            }

        }

        public IEnumerable<Company> GetCompaniesAsync()
        {
            List<Company> results = new List<Company>();
            //var query = this._context.Companies.GetCompanyQueryIterator<Company>(new QueryDefinition(queryString));
            //while (query.HasMoreResults)
            //{
            //    var response = await query.ReadNextAsync();

            //    results.AddRange(response.ToList());
            //}
            try
            {
                foreach (var c in this._context.Companies.ToList())
                {
                    results.Add(c);
                }
            }
            catch(Exception ex)
            {
                _logger.LogError("Exception while getting the list of company: ", ex.Message);
            }
            return results;
        }

        public async Task UpdateCompanyAsync(Company item)
        {
            try
            {
                _context.Companies.Update(item);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception while updating a company: ", ex.Message);
            }

            //await this._context.UpsertCompanyAsync<Company>(item, new PartitionKey(id));
        }


        public async Task AddEmployeeAsync(Employee item)
        {
            //await this._context.CreateCompanyAsync<Company>(item, new PartitionKey(item.Id));
            try
            {
                await this._context.Employees.AddAsync(item);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception while adding a company: ", ex.Message);
            }
        }

        public async Task DeleteEmployeeAsync(string id)
        {
            //await this._context.DeleteCompanyAsync<Company>(id, new PartitionKey(id));
            Employee c = await GetEmployeeAsync(id);
            try
            {
                if (c != null)
                {
                    this._context.Employees.Remove(c);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception while removing a company: ", ex.Message);
            }

        }

        public async Task<Employee> GetEmployeeAsync(string id)
        {
            //try
            //{
            //    CompanyResponse<Company> response = await this._context.ReadCompanyAsync<Company>(id, new PartitionKey(id));
            //    return response.Resource;
            //}
            //catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            //{
            //    return null;
            //}
            try
            {
                return await this._context.Employees.FindAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception while finding a company: ", ex.Message);
                return null;
            }

        }

        public IEnumerable<Employee> GetEmployeesAsync()
        {
            List<Employee> results = new List<Employee>();
            //var query = this._context.Companies.GetEmployeeQueryIterator<Company>(new QueryDefinition(queryString));
            //while (query.HasMoreResults)
            //{
            //    var response = await query.ReadNextAsync();

            //    results.AddRange(response.ToList());
            //}
            try
            {
                foreach (var c in this._context.Employees.ToList())
                {
                    results.Add(c);
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
                _context.Employees.Update(item);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception while updating a company: ", ex.Message);
            }

            //await this._context.UpsertCompanyAsync<Company>(item, new PartitionKey(id));
        }
    }
}
