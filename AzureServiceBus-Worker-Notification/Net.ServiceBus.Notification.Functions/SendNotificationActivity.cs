using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Net.ServiceBus.Notification.Functions
{
    public class SendNotificationActivity
    {
        private readonly ILogger<SendNotificationActivity> _log;

        public SendNotificationActivity(ILogger<SendNotificationActivity> injectedLogger)
        {
            _log = injectedLogger;
        }

        [FunctionName(nameof(SendNotificationActivity))]
        public async Task Run([ActivityTrigger] string name,
            [SignalR(HubName = "notification")] IAsyncCollector<SignalRMessage> signalRMessages
            )
        {
            await signalRMessages.AddAsync(
                new SignalRMessage
                {
                    Target = "ReceiveMessage",
                    Arguments = new[] { name }
                });

            _log.LogInformation("Mock Email sent");
        }
    }
}