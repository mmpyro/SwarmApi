using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SwarmApi.Clients;
using SwarmApi.Services;

namespace SwarmApi.Controllers
{
    [Route("api/[controller]")]
    public class ServiceController : Controller
    {
        private readonly ISwarmService _swarmService;

        public ServiceController(ISwarmService swarmService)
        {
            _swarmService = swarmService;
        }

        [HttpGet]
        public async Task<IActionResult> GetService()
        {
            return await _swarmService.GetServicesAsync();
        }
    }
}