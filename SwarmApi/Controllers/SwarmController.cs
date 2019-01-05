using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SwarmApi.Clients;
using SwarmApi.Dtos;
using SwarmApi.Services;

namespace SwarmApi.Controllers
{
    [Route("api/[controller]")]
    public class SwarmController : Controller
    {
        private readonly ISwarmService _swarmService;

        public SwarmController(ISwarmService swarmService)
        {
            _swarmService = swarmService;
        }

        [Route("service")]
        [HttpGet]
        public async Task<IActionResult> GetService()
        {
            return await _swarmService.GetServicesAsync();
        }

        [Route("service/{id}")]
        [HttpDelete]
        public async Task<IActionResult> DeleteService(string id)
        {
            return await _swarmService.DeleteServicesAsync(id);
        }

        [Route("init")]
        [HttpPost]
        public async Task<IActionResult> InitCluster([FromBody] ClusterInitParameters clusterInitParameters)
        {
            return await _swarmService.InitSwarmAsync(clusterInitParameters);
        }

        [Route("inspect")]
        [HttpGet]
        public async Task<IActionResult> InspectCluster()
        {
            return await _swarmService.InspectSwarmAsync();
        }

        [Route("leave")]
        [HttpPost]
        public async Task<IActionResult> LeaveCluster([FromQuery] bool force = false)
        {
            return await _swarmService.LeaveSwarmAsync(force);
        }
    }
}