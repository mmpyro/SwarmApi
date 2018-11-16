using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SwarmApi.Clients;
using System.Linq;
using static SwarmApi.Constants;

namespace SwarmApi.Services
{
    public interface INodeService
    {
        Task<IActionResult> GetNodeAsync();
    }

    public class NodeService : Service, INodeService
    {
        private readonly ISwarmClient _swarmClient;
        private readonly ILogger _logger;

        public NodeService(ISwarmClient swarmClient, ILoggerFactory loggerFactory)
        {
            _swarmClient = swarmClient;
            _logger = loggerFactory.CreateLogger(ConsoleLogCategory);
        }

        public async Task<IActionResult> GetNodeAsync()
        {
            try
            {
                var nodes = await _swarmClient.GetNodes();
                _logger.LogInformation("Fetch swarm nodes.");
                return Json(nodes.ToArray());
            }
            catch(Exception ex)
            {
                var errorMessage = "Cannot fetch information about nodes.";
                _logger.LogError(ex, errorMessage);
                return InternalServerError(errorMessage);
            }
        }

    }
}