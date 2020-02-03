using AppServiceSingleCosmosDB.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AppServiceSingleCosmosDB.DataService
{

    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Models;

    public interface ICosmosDBService
    {
        Task<IEnumerable<Employee>> GetEmployeesAsync();
        Task<Employee> GetEmployeeAsync(string id);
        Task AddEmployeeAsync(Employee item);
        Task UpdateEmployeeAsync(Employee item);
        Task DeleteEmployeeAsync(string id);
        Task<IEnumerable<Company>> GetCompaniesAsync();
        Task<Company> GetCompanyAsync(string id);
        Task AddCompanyAsync(Company item);
        Task UpdateCompanyAsync(Company item);
        Task DeleteCompanyAsync(string id);
    }
}
