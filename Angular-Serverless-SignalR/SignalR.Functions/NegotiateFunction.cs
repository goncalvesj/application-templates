using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;

namespace SignalR.Functions
{
	public static class NegotiateFunction
	{
		// BASIC NEGOTIATE FUNCTION

		//[FunctionName("negotiate")]
		//public static SignalRConnectionInfo Negotiate(
		//	[HttpTrigger(AuthorizationLevel.Anonymous)]HttpRequest req,
		//	[SignalRConnectionInfo(HubName = "chat")]SignalRConnectionInfo connectionInfo)
		//{
		//	return connectionInfo;
		//}


		// CUSTOM NEGOTIATE WITH PARAMS FROM QUERY STRING
		[FunctionName("negotiate")]
		public static async Task<SignalRConnectionInfo> Negotiate(
			[HttpTrigger(AuthorizationLevel.Anonymous)]
			HttpRequest req, IBinder binder)
		{
			var userId = req.Query["oid"];

			if (string.IsNullOrEmpty(userId)) return null;

			var attribute = new SignalRConnectionInfoAttribute
			{
				HubName = "chat",
				UserId = userId
			};

			return await binder.BindAsync<SignalRConnectionInfo>(attribute).ConfigureAwait(false);
		}
	}
}