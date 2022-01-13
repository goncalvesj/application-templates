//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Azure.Storage.Queues;
//using EventSourcing.Common;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.Azure.Cosmos.Table;
//using Microsoft.Azure.Documents;
//using Microsoft.Extensions.Logging;
//using Newtonsoft.Json;

//namespace EventSourcing.Controllers
//{
//    [ApiController]
//    [Route("eventsourcing")]
//    public class WeatherForecastController : ControllerBase
//    {
//        //private const string storageConnectionString = "DefaultEndpointsProtocol=https;AccountName=v1joaostaticsites;AccountKey=mtcmYFg1Rg1h8m7o3sYlJE4ARKUu7H8iREciOdlKvPLDnGsuef2hljgV5kQCBn031anpANaHFWKYCImZ69rrQQ==;EndpointSuffix=core.windows.net";

//        private readonly CloudTable _eventStoreTable;
//        private readonly CloudTable _eventProjectionsTable;

//        private CloudTable EventStreamSequence;
//        private readonly QueueClient _queue;

//        public WeatherForecastController()
//        {
//            var storageAccount = CloudStorageAccount.DevelopmentStorageAccount;
//            var tableClient = storageAccount.CreateCloudTableClient(new TableClientConfiguration());

//            //EventStreamSequence = tableClient.GetTableReference("EventStreamSequence");

//            _eventStoreTable = tableClient.GetTableReference("EventStoreTable");
//            _eventStoreTable.CreateIfNotExists();

//            _eventProjectionsTable = tableClient.GetTableReference("EventProjectionsTable");
//            _eventProjectionsTable.CreateIfNotExists();

//            var connectionString = "UseDevelopmentStorage=true";
//            var queueName = "eventsourcing-queue";

//            // Get a reference to a queue and then create it
//            _queue = new QueueClient(connectionString, queueName);
//            _queue.CreateIfNotExists();

//            // Send a message to our queue
//        }

//        [HttpGet, Route("user")]
//        public IActionResult Get(string id)
//        {
//            var linqQuery = _eventStoreTable.CreateQuery<EventStoreEntity>()
//                .Where(x => x.PartitionKey == id)
//                .Select(x => new { Key = int.Parse(x.RowKey), Event = x.EventType, Data = x.Payload })
//                .ToList();

//            var userDataModel = new UserDataModel();

//            foreach (var item in linqQuery)
//            {
//                UserDataModel data;
//                switch (item.Event)
//                {
//                    case "User.Created":
//                        userDataModel = JsonConvert.DeserializeObject<UserDataModel>(item.Data);
//                        break;
//                    case "User.NameChanged":
//                        data = JsonConvert.DeserializeObject<UserDataModel>(item.Data);
//                        userDataModel.Name = data.Name;
//                        break;
//                    case "User.AgeChanged":
//                        data = JsonConvert.DeserializeObject<UserDataModel>(item.Data);
//                        userDataModel.Age = data.Age;
//                        break;
//                }
//            }

//            return Ok(userDataModel);
//        }

//        //[HttpGet, Route("user/{userId}/next")]
//        //public async Task<IActionResult> Get(string userId)
//        //{
//        //    var retrieveOperation = TableOperation.Retrieve<EventStreamEntity>("user", userId);
//        //    var result = await EventStreamSequence.ExecuteAsync(retrieveOperation);
//        //    var entity = result.Result as EventStreamEntity;

//        //    var sequence = int.Parse(entity?.LastSequenceNumber ?? throw new InvalidOperationException());
//        //    var next = (sequence + 1).ToString("D5");

//        //    return Ok(new
//        //    {
//        //        next
//        //    });
//        //}

//        //[HttpGet, Route("sequence/{stream}/{id}/next")]
//        //public async Task<IActionResult> Get(string stream, string id)
//        //{
//        //    var retrieveOperation = TableOperation.Retrieve<EventStreamEntity>(stream, id);
//        //    var result = await EventStreamSequence.ExecuteAsync(retrieveOperation);
//        //    var entity = result.Result as EventStreamEntity;

//        //    var sequence = int.Parse(entity?.LastSequenceNumber ?? throw new InvalidOperationException());
//        //    var next = (sequence + 1).ToString("D5");

//        //    return Ok(new
//        //    {
//        //        next
//        //    });
//        //}

//        [HttpPost, Route("project")]
//        public async Task<IActionResult> Post(ProjectModel model)
//        {
//            if (model == null) return BadRequest();

//            var id = !string.IsNullOrEmpty(model.Data.Id) ? model.Data.Id : Guid.NewGuid().ToString();
//            var newUserStream = $"project-{id}";

//            var sequence = GetNext(newUserStream);

//            var entity = new EventStoreEntity(newUserStream, sequence)
//            {
//                EventType = model.Event,
//                Payload = JsonConvert.SerializeObject(model.Data)
//            };

//            var insertOrMergeOperation = TableOperation.Insert(entity);

//            var result = await _eventStoreTable.ExecuteAsync(insertOrMergeOperation);
//            var insertedEntity = result.Result as EventStoreEntity;

//            return Ok(insertedEntity);
//        }

//        [HttpPost, Route("user")]
//        public async Task<IActionResult> Post(UserModel model)
//        {
//            if (model == null) return BadRequest();

//            var id = !string.IsNullOrEmpty(model.Data.Id) ? model.Data.Id : Guid.NewGuid().ToString();
//            var newUserStream = $"user-{id}";

//            var sequence = GetNext(newUserStream);

//            var entity = new EventStoreEntity(newUserStream, sequence)
//            {
//                EventType = model.Event,
//                Payload = JsonConvert.SerializeObject(model.Data)
//            };

//            var insertOrMergeOperation = TableOperation.Insert(entity);

//            var result = await _eventStoreTable.ExecuteAsync(insertOrMergeOperation);
//            var insertedEntity = result.Result as EventStoreEntity;

//            var message = JsonConvert.SerializeObject(new Message { Stream = "users", Id = newUserStream });
//            await _queue.SendMessageAsync(message);

//            return Ok(insertedEntity);
//        }

//        private string GetNext(string eventStream)
//        {
//            var last = _eventStoreTable.CreateQuery<EventStoreEntity>()
//                .Where(x => x.PartitionKey == eventStream)
//                .Select(x => new { Key = int.Parse(x.RowKey) })
//                .ToList()
//                .OrderByDescending(x => x.Key)
//                .FirstOrDefault();

//            var sequence = last?.Key ?? 0;
//            var next = (sequence + 1).ToString("D5");

//            return next;
//        }

//        //private async Task<string> GetNextSequenceNumber(string stream, string id)
//        //{
//        //    var retrieveOperation = TableOperation.Retrieve<EventStreamEntity>(stream, id);
//        //    var result = await EventStreamSequence.ExecuteAsync(retrieveOperation);

//        //    var sequence = result.Result is EventStreamEntity entity ? int.Parse(entity?.LastSequenceNumber) : 0;
//        //    var next = (sequence + 1).ToString("D5");

//        //    return next;
//        //}

//        //private async Task<string> AddSequenceNumber(string stream, string id)
//        //{
//        //    var retrieveOperation = TableOperation.Retrieve<EventStreamEntity>(stream, id);
//        //    var result = await EventStreamSequence.ExecuteAsync(retrieveOperation);

//        //    var sequence = result.Result is EventStreamEntity entity ? int.Parse(entity?.LastSequenceNumber) : 0;
//        //    var next = (sequence + 1).ToString("D5");

//        //    return next;
//        //}
//    }
//}
