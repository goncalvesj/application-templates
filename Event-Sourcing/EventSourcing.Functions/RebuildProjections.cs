using EventSourcing.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventSourcing.Functions
{
    public static class RebuildProjections
    {
        [FunctionName("RebuildProjections")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var storageAccount = CloudStorageAccount.DevelopmentStorageAccount;
            var tableClient = storageAccount.CreateCloudTableClient(new TableClientConfiguration());

            var eventStoreTable = tableClient.GetTableReference("EventStoreTable");
            eventStoreTable.CreateIfNotExists();

            var eventProjectionsTable = tableClient.GetTableReference("EventProjectionsTable");
            eventProjectionsTable.CreateIfNotExists();

            var linqQuery = eventStoreTable.CreateQuery<EventStoreEntity>().ToList();

            var partitions = linqQuery.GroupBy(x => x.PartitionKey).Select(x => x.Key);

            var conferenceList = new List<ConferenceDataModel>();
            TableOperation insertOrMergeOperation;

            foreach (var partition in partitions)
            {
                var events = linqQuery
                    .Where(x => x.PartitionKey == partition)
                    .OrderBy(x => x.Timestamp);

                var entity = ProjectionService.CreateConferenceProjection(eventProjectionsTable, new Message { Stream = "Conference", Id = partition }, events);

                conferenceList.Add(JsonConvert.DeserializeObject<ConferenceDataModel>(entity.Payload));

                insertOrMergeOperation = TableOperation.InsertOrReplace(entity);

                await eventProjectionsTable.ExecuteAsync(insertOrMergeOperation);
            }

            var allConferences = new EventProjectionsEntity
            {
                PartitionKey = "LookUps",
                RowKey = "AllConferences",
                Payload = JsonConvert.SerializeObject(conferenceList)
            };

            insertOrMergeOperation = TableOperation.InsertOrReplace(allConferences);

            await eventProjectionsTable.ExecuteAsync(insertOrMergeOperation);

            return new OkObjectResult("");
        }
    }
}
