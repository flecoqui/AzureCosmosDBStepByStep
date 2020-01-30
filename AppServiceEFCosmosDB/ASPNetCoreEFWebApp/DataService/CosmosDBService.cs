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
    }
}
