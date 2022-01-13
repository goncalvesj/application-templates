using EventSourcing.Common;
using EventSourcing.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EventSourcing.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ConferenceController : ControllerBase
    {
        private readonly IConferenceService _conferenceService;

        public ConferenceController(IConferenceService conferenceService)
        {
            _conferenceService = conferenceService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_conferenceService.GetAllConferences());
        }

        [HttpGet, Route("{id}")]
        public IActionResult Get(string id)
        {
            return Ok(_conferenceService.GetConferenceDetails(id));
        }

        [HttpPut]
        public async Task<IActionResult> Put(ConferenceModel model)
        {
            if (model == null || string.IsNullOrEmpty(model.Data.Id)) return BadRequest();

            var streamId = _conferenceService.GetConferenceId(model);

            if (model.Data.Seats > 0)
            {
                var availableSeats = _conferenceService.GetAvailableSeats(streamId);
                if (model.Event.Equals("Conference.SeatsRemoved") && model.Data.Seats > availableSeats)
                    return BadRequest("Not enough seats available");
            }

            var sequence = _conferenceService.GetNext(streamId);

            var insertedEntity = await _conferenceService.InsertEntityAsync(streamId, sequence, model);

            await _conferenceService.InsertQueueMessageAsync(streamId, sequence);

            return Ok(insertedEntity);
        }

        [HttpPost]
        public async Task<IActionResult> Post(ConferenceModel model)
        {
            if (model == null) return BadRequest();

            var streamId = _conferenceService.GetConferenceId(model);

            var sequence = _conferenceService.GetNext(streamId);

            var insertedEntity = await _conferenceService.InsertEntityAsync(streamId, sequence, model);

            await _conferenceService.InsertQueueMessageAsync(streamId, sequence);

            return Ok(insertedEntity);
        }

    }
}
