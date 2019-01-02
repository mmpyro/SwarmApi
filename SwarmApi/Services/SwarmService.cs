using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SwarmApi.Clients;
using static SwarmApi.Constants;

namespace SwarmApi.Services
{
    public interface ISwarmService
    {
        Task<IActionResult> GetServicesAsync();
    }

    public class SwarmService : Service, ISwarmService
    {
        private readonly ISwarmClient _swarmClient;
        private readonly ILogger _logger;

        public SwarmService(ISwarmClient swarmClient, ILoggerFactory loggerFactory)
        {
            _swarmClient = swarmClient;
            _logger = loggerFactory.CreateLogger(ConsoleLogCategory);
        }

        public async Task<IActionResult> GetServicesAsync()
        {
            try
            {
                var services = await _swarmClient.GetServices();
                _logger.LogInformation("Fetch swarm services");
                return Json(services.ToArray());
            }
            catch(Exception ex)
            {
                var errorMessage = "Cannot fetch information about services.";
                _logger.LogError(ex, errorMessage);
                return InternalServerError(errorMessage);
            }
        }
    }
}