using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using System.Threading.Tasks;

namespace SignalR.Functions
{
    public static class SendMessage
    {
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
    }
}