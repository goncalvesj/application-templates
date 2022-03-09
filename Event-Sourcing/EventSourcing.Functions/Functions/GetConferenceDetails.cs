using EventSourcing.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System.Net;
using System.Threading.Tasks;

namespace EventSourcing.Functions
{
    public class GetConferenceDetails
    {
        private readonly ILogger<GetConferenceDetails> _logger;
        private readonly IConferenceService _conferenceService;

        public GetConferenceDetails(ILogger<GetConferenceDetails> log, IConferenceService conferenceService)
        {
            _logger = log;
            _conferenceService = conferenceService;
        }

        [FunctionName("GetConferenceDetails")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "Queries" })]
        [OpenApiParameter(name: "id", In = ParameterLocation.Query, Required = true, Type = typeof(string), Description = "The **Conference ID** parameter")]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(ConferenceDataModel), Description = "The OK response")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var id = req.Query["id"];

            return new OkObjectResult(_conferenceService.GetConferenceDetails(id));
        }
    }
}

