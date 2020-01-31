using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;



namespace AppServiceCosmosDB.DataService
{
    public class CosmosDBContext 
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
}
