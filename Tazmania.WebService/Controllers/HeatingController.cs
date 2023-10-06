using Microsoft.AspNetCore.Mvc;
using Tazmania.Contracts;
using Tazmania.Contracts.Requests;
using Tazmania.Contracts.Responses;
using Tazmania.Entities;
using Tazmania.Interfaces.Services;

namespace Tazmania.WebService.Controllers
{
    public class HeatingController : ApiControllerBase<HeatingController>
    {
        public HeatingController(IDatabankService databankService,
                                 ILogger<HeatingController> logger) : base(logger, databankService)
        {
        }

        [HttpPost]
        [Route("Fetchs")]
        public async Task<IActionResult> Fetchs([FromBody] HeatingFetchRequest request)
        {
            var ios = await DatabankService.FetchIOs(IOMajor.TemperatureSensor, IOMajor.Valve);
            var settings = await DatabankService.FetchHeatingSettings();

            var response = new HeatingFetchResponse()
            {
                Heatings = (await DatabankService.FetchHeatings()).Select(r => new HeatingContract()
                {
                    Id = r.Id,
                    Description = request.FullData ? r.Description : null,
                    TargetTemperature = r.Temperature,
                    CurrentTemperature = ios.Single(s => s.Id == r.InputSensorId).ValueCorrected,
                    IsActive = ios.Single(s => s.Id == r.OutputValveId).IsActive
                }),
                Times = settings.Times.Select(r => new HeatingTimeContract()
                {
                    StartTime = r.StartTime,
                    EndTime = r.EndTime
                }),
                Mode = (int)settings.Mode,
                IsActive = settings.IsStarted
            };

            return Ok(response);
        }

        [HttpPost]
        [Route("Temperature/Set")]
        public async Task<IActionResult> SetTemperature([FromBody] RequestSetRequest request)
        {
            await DatabankService.SetRequest(RequestType.HeatingSetTemperature, request.EntityId, value: request.Value);

            return Ok();
        }

        [HttpPost]
        [Route("Mode/Set")]
        public async Task<IActionResult> SetMode([FromBody] RequestSetRequest request)
        {
            await DatabankService.SetRequest(RequestType.HeatingSetMode, request.EntityId, value: request.Value);

            return Ok();
        }
    }
}
