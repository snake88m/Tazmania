using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tazmania.Interfaces.Repositories;
using Tazmania.Interfaces.Services;

namespace Tazmania.WebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ApiControllerBase<T> : ControllerBase
    {
        protected readonly ILogger<T> Logger;
        protected readonly IDatabankService DatabankService;

        protected ApiControllerBase(ILogger<T> logger, IDatabankService databankService) 
        {
            Logger = logger;
            DatabankService = databankService;
        }
    }
}
