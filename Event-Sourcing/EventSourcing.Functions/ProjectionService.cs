using EventSourcing.Common;
using Microsoft.Azure.Cosmos.Table;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventSourcing.Functions
{
    public static class ProjectionService
    {
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
    }
}
