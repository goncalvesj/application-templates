using System;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Converters;

namespace SignalR.Functions
{
    public class ProcessMessage
    {
        //private readonly ILogger<ProcessMessage> _logger;

        //public ProcessMessage(ILogger<ProcessMessage> log)
        //{
        //    _logger = log;
        //}

        //[FunctionName("ParseMessage")]
        //public async Task Run([ServiceBusTrigger("Customer1", "Parse", Connection = "ServiceBusConnection")] ServiceBusMessage mySbMsg, Binder binder)
        //{
        //    _logger.LogInformation($"C# ServiceBus topic trigger function processed message: {mySbMsg.ReplyTo}, {mySbMsg.To}");

        //    // Extract some fields and pass to next stage
        //    var attr = new ServiceBusAttribute(mySbMsg.ReplyTo)
        //    {
        //        Connection = "ServiceBusConnection"
        //    };

        //    var sbusMsg = new ServiceBusMessage
        //    {
        //        ReplyTo = mySbMsg.ReplyTo,
        //        To = mySbMsg.To,
        //        Subject = "Transform",
        //        Body = mySbMsg.Body
        //    };

        //    var collector = await binder.BindAsync<IAsyncCollector<ServiceBusMessage>>(attr);

        //    await collector.AddAsync(sbusMsg);
        //}

        //[FunctionName("TranformMessage")]
        //public async Task Run2([ServiceBusTrigger("Customer1", "Transform", Connection = "ServiceBusConnection")] ServiceBusMessage mySbMsg, Binder binder)
        //{
        //    _logger.LogInformation($"C# ServiceBus topic trigger function processed message: {mySbMsg.ReplyTo}, {mySbMsg.To}");

        //    // Augment with more data
        //    var attr = new ServiceBusAttribute(mySbMsg.ReplyTo)
        //    {
        //        Connection = "ServiceBusConnection"
        //    };

        //    var content = mySbMsg.Body.ToObjectFromJson<MessageObject>();

        //    var augmented = new AugmentedMessageObject
        //    {
        //        UserId = content.UserId,
        //        Text = content.Text,
        //        ExtraData = "More Data"
        //    };

        //    var sbusMsg = new ServiceBusMessage
        //    {
        //        ReplyTo = mySbMsg.ReplyTo,
        //        To = mySbMsg.To,
        //        Subject = "Complete",
        //        Body = BinaryData.FromObjectAsJson(augmented)
        //    };

        //    var collector = await binder.BindAsync<IAsyncCollector<ServiceBusMessage>>(attr);

        //    await collector.AddAsync(sbusMsg);
        //}

        //[FunctionName("CompleteMessage")]
        //public void Run3([ServiceBusTrigger("Customer1", "Complete", Connection = "ServiceBusConnection")] MessageObject mySbMsg)
        //{
        //    _logger.LogInformation($"C# ServiceBus topic trigger function processed message: {mySbMsg.UserId}, {mySbMsg.Text}");

        //    // Send to receiver client
        //}
    }
}
