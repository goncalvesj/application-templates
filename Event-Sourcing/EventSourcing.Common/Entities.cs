using System;
using Microsoft.Azure.Cosmos.Table;

namespace EventSourcing.Common
{
    public class EventStoreEntity : TableEntity
    {
        public EventStoreEntity()
        {
        }

        public EventStoreEntity(string eventStream, string sequenceNumber)
        {
            PartitionKey = eventStream;
            RowKey = sequenceNumber;
        }

        public string EventType { get; set; }
        public string Payload { get; set; }
    }

    public class EventProjectionsEntity : TableEntity
    {
        public EventProjectionsEntity()
        {
        }

        public EventProjectionsEntity(string eventStream, string id)
        {
            PartitionKey = eventStream;
            RowKey = id;
        }
        public string LastSequenceRun { get; set; }
        public string Payload { get; set; }
    }

    public class Message
    {
        public string Stream { get; set; }
        public string SequenceNumber { get; set; }
        public string Id { get; set; }
    }
    
    //public class EventStreamEntity : TableEntity
    //{
    //    public EventStreamEntity()
    //    {
    //    }

    //    public EventStreamEntity(string aggregateName, string aggregateId)
    //    {
    //        PartitionKey = aggregateName;
    //        RowKey = aggregateId;
    //    }

    //    public string LastSequenceNumber { get; set; }

    //}

    //public class UserProjection
    //{
    //    public string Name { get; set; }
    //    public int Age { get; set; }
    //}

    //public class ProjectModel
    //{
    //    public string Event { get; set; }
    //    public ProjectDataModel Data { get; set; }
    //}

    //public class ProjectDataModel
    //{
    //    public string Id { get; set; }
    //    public string Name { get; set; }
    //}

    //public class UserModel
    //{
    //    public string Event { get; set; }
    //    public UserDataModel Data { get; set; }
    //}

    //public class UserDataModel
    //{
    //    public string Id { get; set; }
    //    public string Name { get; set; }
    //    public int Age { get; set; }
    //}

    public class ConferenceModel
    {
        public string Event { get; set; }
        public ConferenceDataModel Data { get; set; }
    }

    public class ConferenceDataModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Seats { get; set; }
    }
}
