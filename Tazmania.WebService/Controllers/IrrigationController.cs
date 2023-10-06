using Microsoft.AspNetCore.Mvc;
using Tazmania.Contracts.Requests;
using Tazmania.Contracts.Responses;
using Tazmania.Contracts;
using Tazmania.Interfaces.Services;
using Tazmania.Entities;

namespace Tazmania.WebService.Controllers
{
    public class IrrigationController : ApiControllerBase<IrrigationController>
    {
        public IrrigationController(IDatabankService databankService,
                                    ILogger<IrrigationController> logger) : base(logger, databankService)
        {
        }

        [HttpPost]
        [Route("Fetchs")]
        public async Task<IActionResult> Fetchs([FromBody] IrrigationFetchRequest request)
        {
            var settings = await DatabankService.FetchIrrigationSettings();
            var ios = await DatabankService.FetchIOs(IOMajor.RainSensor, IOMajor.Sprinkler);
            
            var response = new IrrigationFetchResponse()
            {
                Irrigations = (await DatabankService.FetchIrrigations()).Select(r => new IrrigationContract()
                {
                    Id = r.Id,
                    Description = request.FullData ? r.Description : null,
                    Minutes = r.Minutes,
                    IsActive = ios.Single(s => s.Id == r.OutputSprinklerId).IsActive
                }),
                Monday = settings.Monday,
                Tuesday = settings.Tuesday,
                Wednesday = settings.Wednesday,
                Thursday = settings.Thursday,
                Friday = settings.Friday,
                Saturday = settings.Saturday,
                Sunday = settings.Sunday,
                Mode = (int)settings.Mode,
                StartHour = settings.StartTime.Hours,
                RainSensor = ios.Single(s => s.Id == settings.RainSensorId).IsActive
            };

            return Ok(response);
        }


        [HttpPost]
        [Route("Days/Set")]
        public async Task<IActionResult> SetWeekDays([FromBody] RequestSetRequest request)
        {
            await DatabankService.SetRequest(RequestType.IrrigationSetWeekDays, request.EntityId, isActive: request.IsActive);

            return Ok();
        }

        [HttpPost]
        [Route("Mode/Set")]
        public async Task<IActionResult> SetMode([FromBody] RequestSetRequest request)
        {
            await DatabankService.SetRequest(RequestType.IrrigationSetMode, request.EntityId, value: request.Value);

            return Ok();
        }

        [HttpPost]
        [Route("Timer/Set")]
        public async Task<IActionResult> SetTimer([FromBody] RequestSetRequest request)
        {
            await DatabankService.SetRequest(RequestType.IrrigationSetTimer, request.EntityId, value: request.Value);

            return Ok();
        }

        [HttpPost]
        [Route("Watering/Set")]
        public async Task<IActionResult> SetWatering([FromBody] RequestSetRequest request)
        {
            await DatabankService.SetRequest(RequestType.IrrigationSetWatering, request.EntityId, isActive: request.IsActive);

            return Ok();
        }
    }
}
