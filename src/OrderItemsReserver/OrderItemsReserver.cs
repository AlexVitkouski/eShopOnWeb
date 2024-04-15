using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using Azure.Storage.Blobs;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Bson;

namespace OrderItemsReserverHttpTrigger
{
    public class OrderItemsReserver
    {
        [FunctionName("OrderItemsReserver")]
        public void Run([ServiceBusTrigger("%QueueName%", Connection = "ServiceBusConnectionString")]string reservationMessage, ILogger log)
        {
            log.LogInformation($"C# ServiceBus queue trigger function processed message: {reservationMessage}");

            int retryNumber = int.Parse(Environment.GetEnvironmentVariable("RetryNumber"));

            TryToUploadReservation(retryNumber, reservationMessage);
        }

        private void TryToUploadReservation(int retryNumber, string reservationMessage)
        {
            int i = 0;
            while (true)
            {
                try
                {
                    UploadToBlob(reservationMessage);
                    return;
                }
                catch (Exception e)
                {
                    if (i >= retryNumber)
                    {
                        SendEmail(e.Message);
                        return;
                    }
                }
                i++;
            }
        }

        private void UploadToBlob(string reservationMessage)
        {
            string storageConnection = Environment.GetEnvironmentVariable("StorageConnectionString");
            string containerName = Environment.GetEnvironmentVariable("ContainerName");
            var blobClient = new BlobContainerClient(storageConnection, containerName);
            var blob = blobClient.GetBlobClient($"reservation-{Guid.NewGuid()}".ToString());
            blob.Upload(GetStreamToUpload(reservationMessage));
        }

        private void SendEmail(string errorMessage)
        {
            string emailTriggerUrl = Environment.GetEnvironmentVariable("EmailTriggerUrl");

            var email = new { EmailSubject = $"Error happened when reserving order", ErrorMessage = errorMessage };
            StringContent content = ToJson(email);

            var client = new HttpClient();
            client.PostAsync(emailTriggerUrl, content);
        }

        private Stream GetStreamToUpload(string message)
        {
            var byteArray = Encoding.UTF8.GetBytes(message);
            return new MemoryStream(byteArray);
        }

        private StringContent ToJson(object obj)
        {
            return new StringContent(JsonSerializer.Serialize(obj), Encoding.UTF8, "application/json");
        }
    }
}
