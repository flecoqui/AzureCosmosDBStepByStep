using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using AppService3EFCosmosDB.DataService;
namespace AppService3EFCosmosDB
{
    public class Program
    {
        public static async System.Threading.Tasks.Task<int> Main(string[] args)
        {
            
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

            //Continue to run the application
            host.Run();
            return 0;
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
