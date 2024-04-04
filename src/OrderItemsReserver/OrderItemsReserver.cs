using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace OrderItemsReserver
{
    public static class OrderItemsReserver
    {
        [FunctionName("OrderItemsReserver")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");
            
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var byteArray = Encoding.UTF8.GetBytes(requestBody);
            var memoryStream = new MemoryStream(byteArray);

            string storageConnection = Environment.GetEnvironmentVariable("StorageConnectionString");
            string containerName = Environment.GetEnvironmentVariable("ContainerName");
            var blobClient = new BlobContainerClient(storageConnection, containerName);
            var blob = blobClient.GetBlobClient($"reservation-{Guid.NewGuid()}".ToString());
            await blob.UploadAsync(memoryStream);
            
            return new OkObjectResult("Reservation done.");
        }
    }
}
