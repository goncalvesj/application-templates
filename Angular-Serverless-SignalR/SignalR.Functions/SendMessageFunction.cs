using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;

namespace SignalR.Functions
{
	public static class SendMessageFunction
	{
		[FunctionName("SendMessage")]
		public static Task SendMessage(
			[HttpTrigger(AuthorizationLevel.Anonymous, "post")]
			MessageObject message,
			[SignalR(HubName = "chat")] IAsyncCollector<SignalRMessage> signalRMessages)
		{
			if (message == null) return null;

			if (string.IsNullOrEmpty(message.UserId)) return null;

			return signalRMessages.AddAsync(
				new SignalRMessage
				{
					// the message will only be sent to this user ID
					UserId = message.UserId,
					Target = "ReceiveMessage",
					Arguments = new object[] { message }
				});
		}
	}
}