using System.Collections.Generic;
using System.Threading.Tasks;
using Docker.DotNet.Models;
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
        [ProducesResponseType(typeof(IEnumerable<Docker.DotNet.Models.SwarmService>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetService()
        {
            return await _swarmService.GetServicesAsync();
        }

        [Route("service/{id}")]
        [HttpDelete]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteService(string id)
        {
            return await _swarmService.DeleteServicesAsync(id);
        }

        [Route("init")]
        [HttpPost]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> InitCluster([FromBody] ClusterInitParameters clusterInitParameters)
        {
            return await _swarmService.InitSwarmAsync(clusterInitParameters);
        }

        [Route("inspect")]
        [HttpGet]
        [ProducesResponseType(typeof(SwarmInspectResponse), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> InspectCluster()
        {
            return await _swarmService.InspectSwarmAsync();
        }

        [Route("leave")]
        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> LeaveCluster([FromQuery] bool force = false)
        {
            return await _swarmService.LeaveSwarmAsync(force);
        }
    }
}