using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SwarmApi.Clients;
using System.Linq;
using static SwarmApi.Constants;
using SwarmApi.Filters;
using SwarmApi.Extensions;
using SwarmApi.Enums;

namespace SwarmApi.Services
{
    public interface INodeService
    {
        Task<IActionResult> GetNodeAsync(NodeFilterParameters filterParameters);
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

        public async Task<IActionResult> GetNodeAsync(NodeFilterParameters filterParameters)
        {
            try
            {
                var nodes = await _swarmClient.GetNodes();
                if(filterParameters.Hostname.IsNotNullOrEmpty())
                {
                    nodes = nodes.Where(t => t.Description.Hostname == filterParameters.Hostname);
                }
                if(filterParameters.Role != SwarmRole.Unknown)
                {
                    nodes = nodes.Where(t =>  t.Spec.Role.Equals(filterParameters.Role.ToString(), StringComparison.OrdinalIgnoreCase));
                }
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