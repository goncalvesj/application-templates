using Azure.Messaging.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Extensions.Logging;
using Net.ServiceBus.Entities;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Net.ServiceBus.Notification.Functions
{
    public class StartOrchestration
    {
        private readonly ILogger<StartOrchestration> _log;

        public StartOrchestration(ILogger<StartOrchestration> injectedLogger)
        {
            _log = injectedLogger;
        }

        [FunctionName("Start_Orchestration")]
        public static async Task Start(
            [ServiceBusTrigger("output-queue", Connection = "ServiceBusConnectionString")] ServiceBusReceivedMessage myQueueItem,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger log)
        {
            string body = myQueueItem.Body.ToString();
            var data = JsonConvert.DeserializeObject<Root>(body);

            var text = $"Processing complete for {data.data.url}";
            
            string instanceId = await starter.StartNewAsync("Run_Orchestrator", new Entities.Notification { Text = text });

            log.LogInformation($"Started orchestration with ID = '{instanceId}'.");
        }

        [FunctionName("Run_Orchestrator")]
        public async Task<List<string>> RunOrchestrator(
            [OrchestrationTrigger] IDurableOrchestrationContext context)
        {
            var outputs = new List<string>();

            var log = context.CreateReplaySafeLogger(_log);

            var dto = context.GetInput<Entities.Notification>();

            outputs.Add(await context.CallActivityAsync<string>(nameof(SendMailActivity), dto.Text));
            outputs.Add(await context.CallActivityAsync<string>(nameof(SendNotificationActivity), dto.Text));

            return outputs;
        }
    }
}