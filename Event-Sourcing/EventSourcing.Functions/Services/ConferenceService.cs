using Azure.Storage.Queues;
using EventSourcing.Common;
using Microsoft.Azure.Cosmos.Table;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventSourcing.Functions
{
    public interface IConferenceService
    {
        Task<EventStoreEntity> InsertEntityAsync(string streamId, string sequence, ConferenceModel model);
        Task InsertQueueMessageAsync(string streamId, string sequenceNumber);
        string GetConferenceId(ConferenceModel model);
        string GetNext(string eventStream);
        int GetAvailableSeats(string conferenceId);
        ConferenceDataModel GetConferenceDetails(string conferenceId);
        List<ConferenceDataModel> GetAllConferences();
    }

    public class ConferenceService : IConferenceService
    {
        private readonly CloudTable _eventStoreTable;
        private readonly CloudTable _eventProjectionsTable;

        private readonly QueueClient _queue;

        public ConferenceService(string StorageConnectionString, string QueueName)
        {
            var storageAccount = CloudStorageAccount.Parse(StorageConnectionString);
            var tableClient = storageAccount.CreateCloudTableClient(new TableClientConfiguration());

            _eventStoreTable = tableClient.GetTableReference("EventStoreTable");
            _eventStoreTable.CreateIfNotExists();

            _eventProjectionsTable = tableClient.GetTableReference("EventProjectionsTable");
            _eventProjectionsTable.CreateIfNotExists();

            _queue = new QueueClient(StorageConnectionString, QueueName);
            _queue.CreateIfNotExists();
        }

        public async Task<EventStoreEntity> InsertEntityAsync(string streamId, string sequence, ConferenceModel model)
        {
            var entity = new EventStoreEntity(streamId, sequence)
            {
                EventType = model.Event,
                Payload = JsonConvert.SerializeObject(model.Data)
            };

            var insertOrMergeOperation = TableOperation.Insert(entity);

            var result = await _eventStoreTable.ExecuteAsync(insertOrMergeOperation);
            return result.Result as EventStoreEntity;
        }

        public async Task InsertQueueMessageAsync(string streamId, string sequenceNumber)
        {
            var message = JsonConvert.SerializeObject(new Message { Stream = "Conference", Id = streamId, SequenceNumber = sequenceNumber }, Formatting.None);
            var data = Encoding.ASCII.GetBytes(message);
            var base64Encoded = Convert.ToBase64String(data);

            await _queue.SendMessageAsync(base64Encoded);
        }

        public string GetConferenceId(ConferenceModel model)
        {
            var id = !string.IsNullOrEmpty(model.Data.Id) ? model.Data.Id : Guid.NewGuid().ToString();
            model.Data.Id = id;
            return $"conference-{id}";
        }

        public string GetNext(string eventStream)
        {
            var last = _eventStoreTable.CreateQuery<EventStoreEntity>()
                .Where(x => x.PartitionKey == eventStream)
                .Select(x => new { Key = int.Parse(x.RowKey) })
                .ToList()
                .OrderByDescending(x => x.Key)
                .FirstOrDefault();

            var sequence = last?.Key ?? 0;
            var next = (sequence + 1).ToString("D5");

            return next;
        }

        public int GetAvailableSeats(string conferenceId)
        {
            var linqQuery = _eventProjectionsTable.CreateQuery<EventProjectionsEntity>()
                .Where(x => x.PartitionKey == "Conference" && x.RowKey == conferenceId)
                .Select(x => x.Payload)
                .SingleOrDefault();

            var response = JsonConvert.DeserializeObject<ConferenceDataModel>(linqQuery);

            return response.Seats;
        }

        public ConferenceDataModel GetConferenceDetails(string conferenceId)
        {
            var linqQuery = _eventProjectionsTable.CreateQuery<EventProjectionsEntity>()
                .Where(x => x.PartitionKey == "Conference" && x.RowKey == conferenceId)
                .Select(x => x.Payload)
                .SingleOrDefault();

            var response = JsonConvert.DeserializeObject<ConferenceDataModel>(linqQuery);

            return response;
        }

        public List<ConferenceDataModel> GetAllConferences()
        {
            var linqQuery = _eventProjectionsTable.CreateQuery<EventProjectionsEntity>()
                .Where(x => x.PartitionKey == "LookUps" && x.RowKey == "AllConferences")
                .Select(x => x.Payload)
                .SingleOrDefault();

            var response = JsonConvert.DeserializeObject<List<ConferenceDataModel>>(linqQuery);

            return response;
        }
    }
}
