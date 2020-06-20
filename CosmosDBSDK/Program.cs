using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CosmosDBSDK
{
    class Program
    {
        static void Main(string[] args)
        {
            CosmosDBCreation().GetAwaiter().GetResult();
            Console.Read();
        }

        public async static Task CosmosDBCreation()
        {
           var endPoint =  ConfigurationManager.AppSettings["CosmosDBEndPoint"];
            var accountKey = ConfigurationManager.AppSettings["CosmosDBAccountKey"];

            var client = new DocumentClient(new Uri(endPoint), accountKey);
            var dbs = client.CreateDatabaseQuery().ToList();

            foreach(var db in dbs)
            {
                Console.WriteLine($"Retrieved dbs are {db.Id} Rid: {db.ResourceId}");
            }

            var dbDefination = new Database { Id = "vsdb" };
            var result = await client.CreateDatabaseAsync(dbDefination);
            var database = result.Resource;

            var dbUri = UriFactory.CreateDatabaseUri("vsdb");
            await client.DeleteDatabaseAsync(dbUri);
        }
    }
}
