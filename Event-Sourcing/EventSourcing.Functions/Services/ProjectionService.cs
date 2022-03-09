using EventSourcing.Common;
using Microsoft.Azure.Cosmos.Table;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventSourcing.Functions
{
    public interface IProjectionService
    {
        Task RebuildProjections();
        Task BuildProjection(Message message);
    }

    public class ProjectionService : IProjectionService
    {
        private readonly string _storageConnectionString;

        public ProjectionService(string storageConnectionString)
        {
            _storageConnectionString = storageConnectionString;
        }

        public static async Task<EventProjectionsEntity> CreateLookUpProjectionAsync(CloudTable eventProjectionsTable)
        {
            var allConferences = new EventProjectionsEntity
            {
                PartitionKey = "LookUps",
                RowKey = "AllConferences",
                Payload = string.Empty
            };

            var insertOrMergeOperation = TableOperation.InsertOrReplace(allConferences);

            var result = await eventProjectionsTable.ExecuteAsync(insertOrMergeOperation);

            return result.Result as EventProjectionsEntity;
        }

        public static async Task UpdateConferenceLookUpProjectionAsync(CloudTable eventProjectionsTable, EventStoreEntity entity)
        {
            var projection = eventProjectionsTable
                .CreateQuery<EventProjectionsEntity>()
                .Where(x => x.PartitionKey == "LookUps" && x.RowKey == "AllConferences")
                .Select(x => x)
                .SingleOrDefault() ?? await CreateLookUpProjectionAsync(eventProjectionsTable);

            var data = !string.IsNullOrEmpty(projection.Payload)
                ? JsonConvert.DeserializeObject<List<ConferenceDataModel>>(projection.Payload)
                : new List<ConferenceDataModel>();

            var eventData = JsonConvert.DeserializeObject<ConferenceDataModel>(entity?.Payload);

            var conferenceData = data.SingleOrDefault(x => x.Id == eventData.Id);

            switch (entity?.EventType)
            {
                case "Conference.Created":
                    data.Add(eventData);
                    break;
                case "Conference.SeatsAdded":
                    if (conferenceData != null) conferenceData.Seats += eventData.Seats;
                    break;
                case "Conference.SeatsRemoved":
                    if (conferenceData != null) conferenceData.Seats -= eventData.Seats;
                    break;
            }

            projection.Payload = JsonConvert.SerializeObject(data);

            var insertOrMergeOperation = TableOperation.InsertOrReplace(projection);

            await eventProjectionsTable.ExecuteAsync(insertOrMergeOperation);
        }

        public static EventProjectionsEntity CreateConferenceProjection(CloudTable eventProjectionsTable, Message message, IEnumerable<EventStoreEntity> list)
        {
            var projection = eventProjectionsTable
                .CreateQuery<EventProjectionsEntity>()
                .Where(x => x.PartitionKey == "Conference" && x.RowKey == message.Id)
                .Select(x => x)
                .SingleOrDefault() ?? new EventProjectionsEntity(message.Stream, message.Id);

            var dataModel = !string.IsNullOrEmpty(projection.Payload)
            ? JsonConvert.DeserializeObject<ConferenceDataModel>(projection.Payload)
            : new ConferenceDataModel();

            var lastSequenceRun = string.Empty;

            foreach (var item in list.OrderBy(x => x.Timestamp))
            {
                lastSequenceRun = item.RowKey;
                ConferenceDataModel data;

                switch (item.EventType)
                {
                    case "Conference.Created":
                        dataModel = JsonConvert.DeserializeObject<ConferenceDataModel>(item.Payload);
                        break;
                    case "Conference.SeatsAdded":
                        data = JsonConvert.DeserializeObject<ConferenceDataModel>(item.Payload);
                        dataModel.Seats += data.Seats;
                        break;
                    case "Conference.SeatsRemoved":
                        data = JsonConvert.DeserializeObject<ConferenceDataModel>(item.Payload);
                        dataModel.Seats -= data.Seats;
                        break;
                }
            }

            var entity = new EventProjectionsEntity(message.Stream, message.Id)
            {
                LastSequenceRun = lastSequenceRun,
                Payload = JsonConvert.SerializeObject(dataModel)
            };

            return entity;
        }

        public async Task RebuildProjections()
        {
            var storageAccount = CloudStorageAccount.Parse(_storageConnectionString);
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

                var entity = CreateConferenceProjection(eventProjectionsTable, new Message { Stream = "Conference", Id = partition }, events);

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
        }

        public async Task BuildProjection(Message message)
        {
            var storageAccount = CloudStorageAccount.Parse(_storageConnectionString);
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
                "Conference" => CreateConferenceProjection(eventProjectionsTable, message, linqQuery),
                _ => null
            };

            var insertOrMergeOperation = TableOperation.InsertOrReplace(entity);

            await eventProjectionsTable.ExecuteAsync(insertOrMergeOperation);

            await UpdateConferenceLookUpProjectionAsync(eventProjectionsTable, linqQuery.SingleOrDefault());
        }
    }
}
