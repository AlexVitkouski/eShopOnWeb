using System;
using System.Text.Json;
using System.Threading.Tasks;
using DeliveryOrderProcessor.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace DeliveryOrderProcessor
{
    public static class DeliveryOrderProcessor
    {
        [FunctionName("DeliveryOrderProcessor")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            
            string connectionString = Environment.GetEnvironmentVariable("CosmoDbUri");
            string cosmoKey = Environment.GetEnvironmentVariable("CosmoKey");
            string dbName = Environment.GetEnvironmentVariable("CosmoDbName");
            string containerName = Environment.GetEnvironmentVariable("ContainerName");

            CosmosClient client = new CosmosClient(connectionString, cosmoKey);
            Database database = client.GetDatabase(dbName);
            Container container = database.GetContainer(containerName);
            
            OrderDto order = JsonSerializer.Deserialize<OrderDto>(req.Body);
            
            await container.UpsertItemAsync(order, new PartitionKey(order.CountryCity));
            

            return new OkObjectResult("Order processed.");
        }
    }
}
