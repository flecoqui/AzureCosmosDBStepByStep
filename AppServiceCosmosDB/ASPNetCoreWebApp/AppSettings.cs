using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
namespace AppServiceCosmosDB
{
    public class AppSettings
    {
        private static IConfiguration Configuration;
        public static void Initialize(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public static string CosmosServiceName { get { return Configuration["COSMOS_SERVICENAME"] ?? ""; } }
        public static string CosmosServiceKey { get { return Configuration["COSMOS_KEY"] ?? ""; } }
        public static string CosmosDatabaseName { get { return Configuration["COSMOS_DATABASENAME"] ?? "testdb"; } }
        public static string CosmosRegion { get { return Configuration["COSMOS_REGION"] ?? "East US 2"; } }
    }
}
