using Azure.Messaging.ServiceBus;
using Azure.Storage.Blobs;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Newtonsoft.Json;

namespace NetCore.ServiceBus.Worker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private TelemetryClient _telemetryClient;

        private BlobContainerClient _inputBlobContainerClient;
        private BlobContainerClient _outputBlobContainerClient;
        private double _seconds;

        protected IConfiguration Configuration { get; }

        public Worker(ILogger<Worker> logger, TelemetryClient tc, IConfiguration configuration)
        {
            _logger = logger;
            _telemetryClient = tc;
            Configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var inputQueueName = Configuration.GetValue<string>("SERVICEBUS_QUEUE_NAME");
            var inputQueueConnectionString = Configuration.GetValue<string>("SERVICEBUS_QUEUE_CONNECTIONSTRING");
            var storageConnectionString = Configuration.GetValue<string>("STORAGE_CONNECTIONSTRING");
            _seconds = double.Parse(Configuration.GetValue<string>("PROCESSING_TIME_SECONDS"));

            //_logger.LogInformation("Authentication by using connection string");

            var blobServiceClient = new BlobServiceClient(storageConnectionString);
            _inputBlobContainerClient = blobServiceClient.GetBlobContainerClient("input");
            _outputBlobContainerClient = blobServiceClient.GetBlobContainerClient("output");


            var serviceBusClient = new ServiceBusClient(inputQueueConnectionString);
            var messageProcessor = serviceBusClient.CreateProcessor(inputQueueName);

            messageProcessor.ProcessMessageAsync += HandleMessageAsync;
            messageProcessor.ProcessErrorAsync += HandleReceivedExceptionAsync;

            //_logger.LogInformation($"Starting message pump on queue {inputQueueName} in namespace {messageProcessor.FullyQualifiedNamespace}");

            await messageProcessor.StartProcessingAsync(stoppingToken);

            //_logger.LogInformation("Message pump started");

            while (!stoppingToken.IsCancellationRequested)
            {
                //_logger.LogInformation($"Worker running at: {DateTimeOffset.Now}");
                await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
            }

            //_logger.LogInformation("Closing message pump");

            await messageProcessor.CloseAsync(cancellationToken: stoppingToken);

            //_logger.LogInformation($"Message pump closed : {DateTimeOffset.UtcNow}");
        }

        private Task HandleReceivedExceptionAsync(ProcessErrorEventArgs arg)
        {
            _logger.LogError(arg.Exception, "Unable to process message");
            return Task.CompletedTask;
        }

        private async Task HandleMessageAsync(ProcessMessageEventArgs arg)
        {
            try
            {
                using (_telemetryClient.StartOperation<RequestTelemetry>("workeroperation"))
                {
                    var rawMessageBody = arg.Message.Body.ToString();

                    _logger.LogInformation($"Received message {arg.Message.MessageId} with body {rawMessageBody}");

                    var order = JsonConvert.DeserializeObject<Root>(rawMessageBody);

                    if (order != null)
                    {
                        //_logger.LogInformation($"Processing {order.subject}");

                        // Download File from Input Container

                        var blobSplit = order.data.url.Split("/");
                        var name = blobSplit[blobSplit.Length - 1];
                        var inputBlobClient = _inputBlobContainerClient.GetBlobClient(name);

                        var file = await inputBlobClient.DownloadContentAsync();

                        // Simulate work on file
                        await Task.Delay(TimeSpan.FromSeconds(_seconds), arg.CancellationToken);

                        // Upload File to Destination Container
                        var outputBlobClient = _outputBlobContainerClient.GetBlobClient($"{Guid.NewGuid()}.png");
                        await outputBlobClient.UploadAsync(file.Value.Content, true);

                        // Delete File from Input Container
                        await inputBlobClient.DeleteIfExistsAsync();

                        //_logger.LogInformation($"{order.subject} processed");
                    }
                    else
                    {
                        _logger.LogError(
                            "Unable to deserialize to message contract {ContractName} for message {MessageBody}",
                            typeof(Root), rawMessageBody);
                    }

                    var completeMsg = $"Message {arg.Message.MessageId} processed";

                    _logger.LogInformation(completeMsg);

                    await arg.CompleteMessageAsync(arg.Message);

                    _telemetryClient.TrackEvent(new EventTelemetry
                    {
                        Name = completeMsg
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unable to handle message");
            }
        }
    }

    public class Data
    {
        public string api { get; set; }
        public string clientRequestId { get; set; }
        public string requestId { get; set; }
        public string eTag { get; set; }
        public string contentType { get; set; }
        public int contentLength { get; set; }
        public string blobType { get; set; }
        public string url { get; set; }
        public string sequencer { get; set; }
        public StorageDiagnostics storageDiagnostics { get; set; }
    }

    public class Root
    {
        public string topic { get; set; }
        public string subject { get; set; }
        public string eventType { get; set; }
        public string id { get; set; }
        public Data data { get; set; }
        public string dataVersion { get; set; }
        public string metadataVersion { get; set; }
        public DateTime eventTime { get; set; }
    }

    public class StorageDiagnostics
    {
        public string batchId { get; set; }
    }
}