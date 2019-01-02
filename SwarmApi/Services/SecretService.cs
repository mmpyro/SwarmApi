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
    public interface ISecretService
    {
        Task<IActionResult> GetSecretsAsync();
    }

    public class SecretService : Service, ISecretService
    {
        private readonly ISwarmClient _swarmClient;
        private readonly ILogger _logger;

        public SecretService(ISwarmClient swarmClient, ILoggerFactory loggerFactory)
        {
            _swarmClient = swarmClient;
            _logger = loggerFactory.CreateLogger(ConsoleLogCategory);
        }

        public async Task<IActionResult> GetSecretsAsync()
        {
            try
            {
                var secrets = await _swarmClient.GetSecrets();
                _logger.LogInformation("Fetch swarm secrets.");
                return Json(secrets.ToArray());
            }
            catch(Exception ex)
            {
                var errorMessage = "Cannot fetch information about secrets.";
                _logger.LogError(ex, errorMessage);
                return InternalServerError(errorMessage);
            }
        }

    }
}