using EventSourcing.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace EventSourcing.Functions
{
    public class GetAllConferences
    {
        private readonly ILogger<GetAllConferences> _logger;
        private readonly IConferenceService _conferenceService;

        public GetAllConferences(ILogger<GetAllConferences> log, IConferenceService conferenceService)
        {
            _logger = log;
            _conferenceService = conferenceService;
        }

        [FunctionName("GetAllConferences")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "Queries" })]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(List<ConferenceDataModel>), Description = "The OK response")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            return new OkObjectResult(_conferenceService.GetAllConferences());
        }
    }
}

