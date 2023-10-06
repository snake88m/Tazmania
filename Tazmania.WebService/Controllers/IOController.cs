using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tazmania.Contracts;
using Tazmania.Contracts.Requests;
using Tazmania.Contracts.Responses;
using Tazmania.Entities;
using Tazmania.Interfaces.Repositories;
using Tazmania.Interfaces.Services;
using Tazmania.Services;

namespace Tazmania.WebService.Controllers
{
    public class IOController : ApiControllerBase<IOController>
    {
        public IOController(IDatabankService databankService, 
                                    ILogger<IOController> logger) : base(logger, databankService)
        {
        }

        [HttpPost]
        [Route("Fetchs")]
        public async Task<IActionResult> Fetchs([FromBody] IOFetchRequest request)
        {
            var ios = await DatabankService.FetchIOs(IOMajor.Light, IOMajor.Socket, IOMajor.Shutter, IOMajor.ShutterUp);

            var response = new IOFetchResponse()
            {
                Groups = ios.Where(r => r.Major != IOMajor.ShutterUp)
                .GroupBy(key => key.IOGroupId, val => val, (key, val) => new IOFetchGroupResponse()
                {
                    Id = key,
                    Name = request.FullData ? val.First().IOGroup.Name : null,
                    Order = val.First().IOGroup.Order,
                    IOs = val.Select(s => new IOContract()
                    {
                        Id = s.Id,
                        Type = (int)s.Type,
                        Description = request.FullData ? s.Description : null,
                        GroupName = request.FullData ? s.IOGroup.Name : null,
                        IsActive = s.IsActive,
                        Value = s.Value,
                        ValueCorrected = s.ValueCorrected,
                        Major = request.FullData ? s.Major.ToString() : null,
                        ShutterUpId = s.ParentId ?? 0,
                        ShutterUpIsActive = s.ParentId.HasValue ? ios.Single(r => r.Id == s.ParentId).IsActive : false
                    })
                })
            };

            return Ok(response);
        }

        [HttpPost]
        [Route("Gate/Fetchs")]
        public async Task<IActionResult> GateFetchs([FromBody] IOFetchRequest request)
        {
            var response = new IOFetchResponse()
            {
                Groups = (await DatabankService.FetchIOs(IOMajor.DoorSensor, IOMajor.WindowSensor, IOMajor.VasistasSensor)).Where(r => r.IsActive)
                .GroupBy(key => key.IOGroupId, val => val, (key, val) => new IOFetchGroupResponse()
                {
                    Id = key,
                    Name = request.FullData ? val.First().IOGroup.Name : null,
                    Order = val.First().IOGroup.Order,
                    IOs = val.Select(s => new IOContract()
                    {
                        Id = s.Id,
                        Type = (int)s.Type,
                        Description = request.FullData ? s.Description : null,
                        GroupName = request.FullData ? s.IOGroup.Name : null,
                        IsActive = s.IsActive,
                        Major = request.FullData ? s.Major.ToString() : null
                    })
                })
            };

            return Ok(response);
        }

        [HttpPost]
        [Route("Set")]
        public async Task<IActionResult> Set([FromBody] RequestSetRequest request)
        {
            await DatabankService.SetRequest(RequestType.AutomationSetOutput, request.EntityId, isActive: request.IsActive);

            return Ok();
        }
    }
}
