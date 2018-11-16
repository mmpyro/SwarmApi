using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SwarmApi.Clients;
using System.Linq;
using System.Threading.Tasks;
using SwarmApi.Services;

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

        public async Task<IActionResult> GetNode()
        {
            return await nodeService.GetNodeAsync();
        }
    }
}