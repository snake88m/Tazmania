using Microsoft.AspNetCore.Mvc;
using Tazmania.Contracts.Requests;
using Tazmania.Contracts.Responses;
using Tazmania.Contracts;
using Tazmania.Entities;
using Tazmania.Interfaces.Services;
using Tazmania.Extensions;

namespace Tazmania.WebService.Controllers
{
    public class SchedulerController : ApiControllerBase<SchedulerController>
    {
        public SchedulerController(IDatabankService databankService,
                                   ILogger<SchedulerController> logger) : base(logger, databankService)
        {
        }

        [HttpPost]
        [Route("Fetchs")]
        public async Task<IActionResult> Fetchs([FromBody] SchedulerFetchRequest request)
        {
            var response = new SchedulerFetchResponse()
            {
                Schedulers = (await DatabankService.FetchSchedulers()).Select(r => new SchedulerContract()
                {
                    Id = r.Id,
                    Description = request.FullData ? r.Description : null,
                    Mode = (int)r.Mode,
                    StartDate = r.Start.AsDateString(),
                    EndDate = r.End.AsDateString(),
                    StartTime = r.Start.AsTimeString(),
                    EndTime = r.End.AsTimeString()
                })
            };

            return Ok(response);
        }

        [HttpPost]
        [Route("Mode/Set")]
        public async Task<IActionResult> SetMode([FromBody] RequestSetRequest request)
        {
            await DatabankService.SetRequest(RequestType.SchedulerSetMode, request.EntityId, value: request.Value);

            return Ok();
        }
    }
}
