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
    public class RebuildProjections
    {
        private readonly ILogger<RebuildProjections> _logger;
        private readonly IProjectionService _projectionService;

        public RebuildProjections(ILogger<RebuildProjections> log, IProjectionService projectionService)
        {
            _logger = log;
            _projectionService = projectionService;
        }

        [FunctionName("RebuildProjections")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "Commands" })]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "text/plain", bodyType: typeof(string), Description = "The OK response")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            await _projectionService.RebuildProjections();

            return new OkObjectResult("Projections rebuilt.");
        }
    }
}

