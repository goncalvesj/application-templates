using System;
using System.Collections.Generic;
using System.Dynamic;
using EventSourcing.Common;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Linq;
using System.Threading.Tasks;

namespace EventSourcing.Functions
{
    public static class BuildProjection
    {
        [FunctionName("BuildProjection")]
        public static async Task Run([QueueTrigger("eventsourcing-queue", Connection = "LocalStorage")]string myQueueItem, ILogger log)
        {
            log.LogInformation($"C# Queue trigger function processed: {myQueueItem}");

            var message = JsonConvert.DeserializeObject<Message>(myQueueItem);

            var storageAccount = CloudStorageAccount.DevelopmentStorageAccount;
            var tableClient = storageAccount.CreateCloudTableClient(new TableClientConfiguration());

            var eventStoreTable = tableClient.GetTableReference("EventStoreTable");
            eventStoreTable.CreateIfNotExists();

            var eventProjectionsTable = tableClient.GetTableReference("EventProjectionsTable");
            eventProjectionsTable.CreateIfNotExists();

            var linqQuery = eventStoreTable.CreateQuery<EventStoreEntity>()
                .Where(x => x.PartitionKey == message.Id && x.RowKey == message.SequenceNumber)
                .ToList();

            var entity = message.Stream switch
            {
                "Conference" => ProjectionService.CreateConferenceProjection(eventProjectionsTable, message, linqQuery),
                _ => null
            };

            var insertOrMergeOperation = TableOperation.InsertOrReplace(entity);

            await eventProjectionsTable.ExecuteAsync(insertOrMergeOperation);

            await ProjectionService.UpdateConferenceLookUpProjectionAsync(eventProjectionsTable, linqQuery.SingleOrDefault());
        }
    }
}

//private static EventProjectionsEntity CreateUserProjection(Message message, IEnumerable<EventStoreEntity> list)
//{
//    var userDataModel = new UserDataModel();

//    foreach (var item in list)
//    {
//        UserDataModel data;
//        switch (item.EventType)
//        {
//            case "User.Created":
//                userDataModel = JsonConvert.DeserializeObject<UserDataModel>(item.Payload);
//                break;
//            case "User.NameChanged":
//                data = JsonConvert.DeserializeObject<UserDataModel>(item.Payload);
//                userDataModel.Name = data.Name;
//                break;
//            case "User.AgeChanged":
//                data = JsonConvert.DeserializeObject<UserDataModel>(item.Payload);
//                userDataModel.Age = data.Age;
//                break;
//        }
//    }

//    var entity = new EventProjectionsEntity(message.Stream, message.Id)
//    {
//        Payload = JsonConvert.SerializeObject(userDataModel)
//    };

//    return entity;
//}