using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Docker.DotNet;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SwarmApi.Clients;
using SwarmApi.Extensions;
using static SwarmApi.Constants;

namespace SwarmApi.Services
{
    public interface ISwarmService
    {
        Task<IActionResult> GetServicesAsync();
        Task<IActionResult> DeleteServicesAsync(string id);
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

        public async Task<IActionResult> DeleteServicesAsync(string id)
        {
            try
            {
                if(id.IsNullOrEmpty())
                {
                    throw new ArgumentException("");
                }
                await _swarmClient.DeleteService(id);
                _logger.LogInformation($"Delete service {id}.");
                return NoContent();
            }
            catch(ArgumentException ex)
            {
                _logger.LogError(ex, "Cannot delete service with empty id.");
                return BadRequest(ex.Message);
            }
            catch(DockerApiException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                _logger.LogInformation($"Service with id: {id} not found.");
                return NotFound();
            }
            catch(Exception ex)
            {
                var errorMessage = "Cannot delete service.";
                _logger.LogError(ex, errorMessage);
                return InternalServerError(errorMessage);
            }
        }
    }
}