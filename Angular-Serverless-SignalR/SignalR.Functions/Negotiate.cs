using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using System.Threading.Tasks;

namespace SignalR.Functions
{
    public static class Negotiate
    {
        //[FunctionName("negotiate")]
        //public static SignalRConnectionInfo NegotiateFunction(
        //    [HttpTrigger(AuthorizationLevel.Anonymous)] HttpRequest req,
        //    [SignalRConnectionInfo(HubName = "HealthHub")] SignalRConnectionInfo connectionInfo)
        //{
        //    return connectionInfo;
        //}

        //CUSTOM NEGOTIATE WITH PARAMS FROM QUERY STRING
        [FunctionName("negotiate")]
        public static async Task<SignalRConnectionInfo> NegotiateFunction(
             [HttpTrigger(AuthorizationLevel.Anonymous)]
            HttpRequest req, IBinder binder)
        {
            var userId = req.Query["oid"];

            if (string.IsNullOrEmpty(userId)) return null;

            var attribute = new SignalRConnectionInfoAttribute
            {
                HubName = "HealthHub",
                UserId = userId
            };

            return await binder.BindAsync<SignalRConnectionInfo>(attribute).ConfigureAwait(false);
        }
    }
}