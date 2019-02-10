using System.Threading.Tasks;
using Docker.DotNet.Models;
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
        [ProducesResponseType(typeof(VersionResponse), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetVersion()
        {
            return await _systemService.GetVersionAsync();
        }

        [Route("systeminfo")]
        [HttpGet]
        [ProducesResponseType(typeof(SystemInfoResponse), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetSystemInfo()
        {
            return await _systemService.GetSystemInfoAsync();
        }
    }
}