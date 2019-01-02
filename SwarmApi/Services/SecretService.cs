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
using SwarmApi.Validators;
using Docker.DotNet.Models;

namespace SwarmApi.Services
{
    public interface ISecretService
    {
        Task<IActionResult> GetSecretsAsync();
        Task<IActionResult> GetSecretByNameAsync(string name);
        Task<IActionResult> GetSecretByIdAsync(string id);
        Task<IActionResult> CreateSecretAsync(SecretDto secretDto);
        Task<IActionResult> DeleteSecretAsync(string id);
    }

    public class SecretService : Service, ISecretService
    {
        private readonly ISwarmClient _swarmClient;
        private readonly IValidator<SecretDto> _validator;
        private readonly ILogger _logger;

        public SecretService(ISwarmClient swarmClient, ILoggerFactory loggerFactory, IValidator<SecretDto> validator)
        {
            _swarmClient = swarmClient;
            _validator = validator;
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

        public async Task<IActionResult> GetSecretByNameAsync(string name)
        {
            return await GetSecretAsync(s => s?.Spec?.Name == name);
        }

        public async Task<IActionResult> GetSecretByIdAsync(string id)
        {
            return await GetSecretAsync(s => s?.ID == id);
        }

        public async Task<IActionResult> CreateSecretAsync(SecretDto secretDto)
        {
            try
            {
                _validator.Validate(secretDto);
                var secretSpec = new Docker.DotNet.Models.SecretSpec();
                var bytes = Encoding.UTF8.GetBytes(secretDto.Content).ToList();
                secretSpec.Data = bytes;
                secretSpec.Name = secretDto.Name;
                secretSpec.Labels = secretDto.Labels;
                var secretCreateResponse = await _swarmClient.CreateSecret(secretSpec);
                return Created($"/api/secret/{secretDto.Name}", secretCreateResponse);
            }
            catch(ArgumentException ex)
            {
                var errorMessage = "Cannot create secret.";
                _logger.LogError(ex, errorMessage);
                return BadRequest(ex.Message);  
            }
            catch(Exception ex)
            {
                var errorMessage = "Cannot create secret.";
                _logger.LogError(ex, errorMessage);
                return InternalServerError(errorMessage);
            }
        }

        public async Task<IActionResult> DeleteSecretAsync(string id)
        {
            try
            {
                if(id.IsNullOrEmpty())
                {
                    throw new ArgumentException();
                }
                await _swarmClient.DeleteSecret(id);
                return NoContent();
            }
            catch(ArgumentException ex)
            {
                _logger.LogError(ex, "Cannot delete secret with empty id.");
                return BadRequest(ex.Message);
            }
            catch(Exception ex)
            {
                var errorMessage = $"Cannot delete {id} secret.";
                _logger.LogError(ex, errorMessage);
                return InternalServerError(errorMessage);
            }
        }

        private async Task<IActionResult> GetSecretAsync(Predicate<Secret> pred)
        {
            try
            {
                var secrets = await _swarmClient.GetSecrets();
                var secret = secrets.FirstOrDefault(s => pred(s));
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

    }
}