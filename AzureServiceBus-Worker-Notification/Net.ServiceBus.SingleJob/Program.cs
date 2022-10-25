using Azure.Messaging.ServiceBus;
using Azure.Storage.Blobs;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.WorkerService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.ApplicationInsights;
using Net.ServiceBus.Entities;
using Newtonsoft.Json;

var secretsPath = Environment.GetEnvironmentVariable("CONFIG_FILES_PATH");

var configuration = new ConfigurationBuilder()
    .AddEnvironmentVariables()
    .AddKeyPerFile(directoryPath: $"{secretsPath}", optional: true)
    .Build();

var services = new ServiceCollection();

services.AddLogging(loggingBuilder => loggingBuilder.AddFilter<ApplicationInsightsLoggerProvider>("Category", LogLevel.Information));

services.AddApplicationInsightsTelemetryWorkerService(new ApplicationInsightsServiceOptions
{
    ConnectionString = configuration.GetValue<string>("APPLICATIONINSIGHTS_CONNECTION_STRING")
});

IServiceProvider serviceProvider = services.BuildServiceProvider();

var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
var telemetryClient = serviceProvider.GetRequiredService<TelemetryClient>();

var inputQueueName = configuration.GetValue<string>("SERVICEBUS_QUEUE_NAME");
var inputQueueConnectionString = configuration.GetValue<string>("SERVICEBUS_QUEUE_CONNECTIONSTRING");
var storageConnectionString = configuration.GetValue<string>("STORAGE_CONNECTIONSTRING");
var _seconds = double.Parse(configuration.GetValue<string>("PROCESSING_TIME_SECONDS"));

using (telemetryClient.StartOperation<RequestTelemetry>("singlejob-operation"))
{
    await using var client = new ServiceBusClient(inputQueueConnectionString);
    
    var receiver = client.CreateReceiver(inputQueueName, new ServiceBusReceiverOptions
    {
        ReceiveMode = ServiceBusReceiveMode.PeekLock
    });

    try
    {
        var blobServiceClient = new BlobServiceClient(storageConnectionString);
        var _inputBlobContainerClient = blobServiceClient.GetBlobContainerClient("input");
        var _outputBlobContainerClient = blobServiceClient.GetBlobContainerClient("output");
        
        // the received message is a different type as it contains some service set properties
        var receivedMessage = await receiver.ReceiveMessageAsync();

        var body = receivedMessage.Body.ToString();
        logger.LogWarning("Response from queue is:" + body); // this will be captured by Application Insights.

        var data = JsonConvert.DeserializeObject<Root>(body);
        if (data != null)
        {
            // Download File from Input Container

            var blobSplit = data.data.url.Split("/");
            var name = blobSplit[^1];
            var inputBlobClient = _inputBlobContainerClient.GetBlobClient(name);

            var file = await inputBlobClient.DownloadContentAsync();

            // Simulate work on file
            await Task.Delay(TimeSpan.FromSeconds(_seconds));

            // Upload File to Destination Container
            var outputBlobClient = _outputBlobContainerClient.GetBlobClient($"{Guid.NewGuid()}.png");
            
            await outputBlobClient.UploadAsync(file.Value.Content, true);

            // Delete File from Input Container
            await inputBlobClient.DeleteIfExistsAsync();
        }
        else
        {
            logger.LogError(
                "Unable to deserialize to message contract {ContractName} for message {MessageBody}",
                typeof(Root), body);
        }        

        var completeMsg = $"Single Job V3 - Message {body} processed";
        telemetryClient.TrackEvent(completeMsg);

        await receiver.CompleteMessageAsync(receivedMessage);
    }
    catch (Exception ex)
    {
        telemetryClient.TrackException(new ExceptionTelemetry(ex));
        throw;
    }
    finally
    {
        telemetryClient.Flush();
        await receiver.CloseAsync();
        await Task.Delay(TimeSpan.FromSeconds(1));
    }
}