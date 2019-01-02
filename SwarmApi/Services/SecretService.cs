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
using SwarmApi.Dtos;
using System.Text;

namespace SwarmApi.Services
{
    public interface ISecretService
    {
        Task<IActionResult> GetSecretsAsync();
        Task<IActionResult> GetSecretAsync(string name);
        Task<IActionResult> CreateSecretAsync(SecretDto secretDto);
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

        public async Task<IActionResult> GetSecretAsync(string name)
        {
            try
            {
                var secrets = await _swarmClient.GetSecrets();
                var secret = secrets.FirstOrDefault(t => t.Spec?.Name == name);
                if(secret == null)
                {
                    return NotFound();
                }
                _logger.LogInformation("Fetch swarm secrets.");
                return Json(secret);
            }
            catch(Exception ex)
            {
                var errorMessage = "Cannot fetch information about secrets.";
                _logger.LogError(ex, errorMessage);
                return InternalServerError(errorMessage);
            }
        }

        public async Task<IActionResult> CreateSecretAsync(SecretDto secretDto)
        {
            try
            {
                var secretSpec = new Docker.DotNet.Models.SecretSpec();
                var bytes = Encoding.UTF8.GetBytes(secretDto.Content).ToList();
                secretSpec.Data = bytes;
                secretSpec.Name = secretDto.Name;
                var secretCreateResponse = await _swarmClient.CreateSecret(secretSpec);
                return Created($"api/secret/{secretDto.Name}", secretCreateResponse);
            }
            catch(Exception ex)
            {
                var errorMessage = "Cannot create secret.";
                _logger.LogError(ex, errorMessage);
                return InternalServerError(errorMessage);
            }
        }

    }
}