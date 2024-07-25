using Azure;
using Azure.Data.Tables;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;

namespace AzFunc.Webhook
{
    public class Webhooks
    {
        private readonly ILogger<Webhooks> _logger;

        private readonly TableClient tableClient;

        public Webhooks(ILogger<Webhooks> logger)
        {
            _logger = logger;

            var connString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
            tableClient = new TableClient(connString, "subscriptions");
            tableClient.CreateIfNotExists();
        }

        private static string GetHash(string input)
        {
            var bytes = Encoding.UTF8.GetBytes(input);
            var hash = SHA256.HashData(bytes);
            return BitConverter.ToString(hash).Replace("-", string.Empty);
        }

        [Function("subscribe")]
        public IActionResult Sub([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req, string callbackUrl)
        {
            if (string.IsNullOrEmpty(callbackUrl))
                return new BadRequestResult();
          
            var hash = GetHash(callbackUrl);

            var tableEntity = new TableEntity("subscriptions", hash)
            {
                { "callbackUrl", callbackUrl }
            };
            tableClient.AddEntity(tableEntity);

            return new OkResult();
        }

        [Function("list")]
        public IActionResult List([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req)
        {
            var callbackUrls = tableClient.Query<TableEntity>(filter: $"PartitionKey eq 'subscriptions'")
                              .Select(qEntity => qEntity.GetString("callbackUrl"))
                              .ToList();

            return new OkObjectResult(callbackUrls);
        }

        [Function("clear")]
        public IActionResult Clear([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req)
        {
            Pageable<TableEntity> queryResults = tableClient.Query<TableEntity>();

            foreach (TableEntity entity in queryResults)
            {
                tableClient.DeleteEntity(entity.PartitionKey, entity.RowKey);
            }

            return new OkResult();
        }

        [Function("trigger")]
        public async Task<IActionResult> Trigger([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req)
        {
            using HttpClient client = new();
            var httpContent = new StringContent(@"{""trigger"":""fired""}", Encoding.UTF8, "application/json");

            Pageable<TableEntity> queryResultsFilter = tableClient.Query<TableEntity>(filter: $"PartitionKey eq 'subscriptions'");
            foreach (TableEntity qEntity in queryResultsFilter)
            {
                var callbackUrl = qEntity.GetString("callbackUrl");

                _logger.LogInformation(callbackUrl);                

                // Set a timeout for the request
                using CancellationTokenSource cts = new(TimeSpan.FromSeconds(10));
                try
                {
                    var response = await client.PostAsync(callbackUrl, httpContent, cts.Token);

                    // Check if the response indicates success
                    if (response.IsSuccessStatusCode)
                    {
                        Console.WriteLine("Request succeeded.");
                    }
                    else
                    {
                        Console.WriteLine($"Request failed with status code: {response.StatusCode}");
                    }
                }
                catch (TaskCanceledException)
                {
                    Console.WriteLine("Request timed out, but continuing processing.");
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine($"Request error: {e.Message}");
                }

            }

            return new OkResult();
        }

        [Function("unsubscribe")]
        public IActionResult Unsub([HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequest req, string callbackUrl)
        {
            if (string.IsNullOrEmpty(callbackUrl))
                return new BadRequestResult();

            var hash = GetHash(callbackUrl);

            tableClient.DeleteEntity("subscriptions", hash);            

            return new OkResult();
        }
    }
}
