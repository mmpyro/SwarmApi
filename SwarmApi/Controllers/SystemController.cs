using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SwarmApi.Services;

namespace SwarmApi.Controllers
{
    [Route("api/[controller]")]
    public class SystemController : Controller
    {
        private readonly ISystemService _systemService;

        public SystemController(ISystemService systemService)
        {
            _systemService = systemService;
        }

        [Route("version")]
        [HttpGet]
        public async Task<IActionResult> GetVersion()
        {
            return await _systemService.GetVersionAsync();
        }

        [Route("systeminfo")]
        [HttpGet]
        public async Task<IActionResult> GetSystemInfo()
        {
            return await _systemService.GetSystemInfoAsync();
        }
    }
}