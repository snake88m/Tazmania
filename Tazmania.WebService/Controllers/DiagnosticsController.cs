using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace Tazmania.WebService.Controllers
{
    public class DiagnosticsController : ControllerBase
    {
        public ContentResult Index()
        {
            return base.Content($"{Assembly.GetEntryAssembly()?.GetName().Name ?? "unknow name"} {Assembly.GetEntryAssembly()?.GetName().Version?.ToString() ?? "unknow version"} is running...", "text/html");
        }
    }
}
