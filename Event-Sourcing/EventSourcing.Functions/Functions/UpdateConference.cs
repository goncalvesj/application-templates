using EventSourcing.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace EventSourcing.Functions
{
    public class UpdateConference
    {
        private readonly ILogger<UpdateConference> _logger;
        private readonly IConferenceService _conferenceService;

        public UpdateConference(ILogger<UpdateConference> log, IConferenceService conferenceService)
        {
            _logger = log;
            _conferenceService = conferenceService;
        }

        [FunctionName("UpdateConference")]
        [OpenApiOperation(operationId: "Run", tags: new[] { "Commands" })]
        [OpenApiRequestBody(contentType: "application/json", bodyType: typeof(ConferenceModel), Required = true)]
        [OpenApiResponseWithBody(statusCode: HttpStatusCode.OK, contentType: "application/json", bodyType: typeof(ConferenceModel), Description = "The OK response")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = null)] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var model = JsonConvert.DeserializeObject<ConferenceModel>(requestBody);

            var streamId = _conferenceService.GetConferenceId(model);

            if (model.Data.Seats > 0)
            {
                var availableSeats = _conferenceService.GetAvailableSeats(streamId);
                if (model.Event.Equals("Conference.SeatsRemoved") && model.Data.Seats > availableSeats)
                    return new BadRequestObjectResult("Not enough seats available");
            }

            var sequence = _conferenceService.GetNext(streamId);

            var insertedEntity = await _conferenceService.InsertEntityAsync(streamId, sequence, model);

            await _conferenceService.InsertQueueMessageAsync(streamId, sequence);

            return new OkObjectResult(insertedEntity);
        }
    }
}

