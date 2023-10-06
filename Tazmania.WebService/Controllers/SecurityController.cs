using Microsoft.AspNetCore.Mvc;
using Tazmania.Contracts;
using Tazmania.Contracts.Requests;
using Tazmania.Contracts.Responses;
using Tazmania.Entities;
using Tazmania.Extensions;
using Tazmania.Interfaces.Services;

namespace Tazmania.WebService.Controllers
{
    public class SecurityController : ApiControllerBase<SecurityController>
    {
        public SecurityController(IDatabankService databankService,
                                  ILogger<SecurityController> logger) : base(logger, databankService)
        {

        }

        [HttpPost]
        [Route("Fetchs")]
        public async Task<IActionResult> Fetchs([FromBody] SecurityFetchRequest request)
        {
            var settings = await DatabankService.FetchSecuritySettings();

            return Ok(new SecurityFetchResponse()
            {
                AntiFireActive = settings.AntiFireActive,
                SOSActive = settings.SOSActive,
                AntitheftActive = settings.AntitheftActive,
                NotifyCall = settings.NotifyCall,
                NotifySiren = settings.NotifySiren,
                AntiFireDetail = settings.AntiFireDetail,
                SOSDetail = settings.SOSDetail,
                AntitheftDetail = settings.AntitheftDetail,
                AntitheftMode = (int)settings.AntitheftMode,
                OpenGates = (await DatabankService.FetchIOs(IOMajor.WindowSensor, IOMajor.DoorSensor)).Where(r => r.IsActive).Select(s => new IOContract()
                {
                    Id = s.Id,
                    Description = s.Description,
                    GroupName = s.IOGroup.Name,
                    IsActive = s.IsActive,
                    Major = s.Major.ToString()
                })
            });
        }

        [HttpPost]
        [Route("Alarm/SOS/Active")]
        public async Task<IActionResult> ActiveSOS([FromBody] RequestSetRequest request)
        {
            await DatabankService.SetRequest(RequestType.SecurityActiveSOS, request.EntityId);

            return Ok();
        }

        [HttpPost]
        [Route("Alarm/AntiFire/Active")]
        public async Task<IActionResult> ActiveAntiFire([FromBody] RequestSetRequest request)
        {
            await DatabankService.SetRequest(RequestType.SecurityActiveAntiFire, request.EntityId);

            return Ok();
        }

        [HttpPost]
        [Route("Alarm/Deactive")]
        public async Task<IActionResult> DeactiveAlarms([FromBody] RequestSetRequest request)
        {
            await DatabankService.SetRequest(RequestType.SecurityDeactiveAlarms, request.EntityId);

            return Ok();
        }

        [HttpPost]
        [Route("Alarm/Antitheft/Set")]
        public async Task<IActionResult> SetAntitheft([FromBody] RequestSetRequest request)
        {
            await DatabankService.SetRequest(RequestType.SecurityAntitheftSetMode, request.EntityId, value: request.Value);

            return Ok();
        }

        [HttpPost]
        [Route("Siren/Set")]
        public async Task<IActionResult> SetNotifySiren([FromBody] RequestSetRequest request)
        {
            await DatabankService.SetRequest(RequestType.SecurityNotifySirenSet, request.EntityId, isActive: request.IsActive);

            return Ok();
        }

        [HttpPost]
        [Route("Call/Set")]
        public async Task<IActionResult> SetNotifyCall([FromBody] RequestSetRequest request)
        {
            await DatabankService.SetRequest(RequestType.SecurityNotifyCallSet, request.EntityId, isActive: request.IsActive);

            return Ok();
        }
    }
}
