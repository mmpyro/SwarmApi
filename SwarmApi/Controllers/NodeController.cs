using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SwarmApi.Clients;
using System.Linq;
using System.Threading.Tasks;
using SwarmApi.Services;
using SwarmApi.Enums;

namespace SwarmApi.Controllers
{
    [Route("api/[controller]")]
    public class NodeController : Controller
    {
        private readonly INodeService nodeService;

        public NodeController(INodeService nodeService)
        {
            this.nodeService = nodeService;
        }

        [HttpGet]
        public async Task<IActionResult> GetNode([FromQuery] string hostname, [FromQuery] SwarmRole role)
        {
            return await nodeService.GetNodeAsync(new Filters.NodeFilterParameters{
                Hostname = hostname,
                Role = role
            });
        }
    }
}