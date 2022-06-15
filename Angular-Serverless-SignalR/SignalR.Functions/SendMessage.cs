using Azure.Messaging.ServiceBus;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace SignalR.Functions
{
    public class SendMessage
    {
        private readonly ServiceBusOptions _options;
        public SendMessage(IOptions<ServiceBusOptions> options)
        {
            _options = options.Value;
        }


        [FunctionName("SendMessage")]
        public static Task SendMessageFunction(
             [HttpTrigger(AuthorizationLevel.Anonymous, "post")]
            MessageObject message,
             [SignalR(HubName = "HealthHub")] IAsyncCollector<SignalRMessage> signalRMessages)
        {
            if (message == null) return null;

            if (string.IsNullOrEmpty(message.UserId)) return null;

            return signalRMessages.AddAsync(
                new SignalRMessage
                {
                    // the message will only be sent to this user ID
                    UserId = message.UserId,
                    Target = "ReceivedMessage",
                    Arguments = new object[] { message }
                });
        }

        [FunctionName("SendMessageToSbus")]
        public async Task<IActionResult> SendMessageToSbus(
       [HttpTrigger(AuthorizationLevel.Anonymous, "post")] MessageObject message, Binder binder)
        {
            // Validate some stuff
            if (message == null) return null;

            if (string.IsNullOrEmpty(message.UserId)) return null;

            // Send Message to next Topic

            var attr = new ServiceBusAttribute(message.UserId)
            {
                Connection = "ServiceBusConnection"
            };

            var sbusMsg = new ServiceBusMessage
            {
                ReplyTo = "Customer1",
                To = "Customer2",
                Body = BinaryData.FromObjectAsJson(message)
            };

            var collector = await binder.BindAsync<IAsyncCollector<ServiceBusMessage>>(attr);

            await collector.AddAsync(sbusMsg);

            return new OkObjectResult("OK");
        }
    }
}