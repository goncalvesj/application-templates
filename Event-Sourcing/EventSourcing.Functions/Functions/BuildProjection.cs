using EventSourcing.Common;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace EventSourcing.Functions
{
    public class BuildProjection
    {
        private readonly ILogger<RebuildProjections> _logger;
        private readonly IProjectionService _projectionService;

        public BuildProjection(ILogger<RebuildProjections> log, IProjectionService projectionService)
        {
            _logger = log;
            _projectionService = projectionService;
        }

        [FunctionName("BuildProjection")]
        public async Task Run([QueueTrigger("eventsourcing-queue", Connection = "StorageQueue")] string myQueueItem, ILogger log)
        {
            _logger.LogInformation($"C# Queue trigger function processed: {myQueueItem}");

            var message = JsonConvert.DeserializeObject<Message>(myQueueItem);

            await _projectionService.BuildProjection(message);
        }
    }
}
