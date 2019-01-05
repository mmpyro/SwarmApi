using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SwarmApi.Clients;
using System.Linq;
using SwarmApi.Filters;
using SwarmApi.Extensions;
using SwarmApi.Enums;
using SwarmApi.Dtos;
using System.Text;
using SwarmApi.Validators;
using Docker.DotNet.Models;
using Docker.DotNet;
using System.Net;

namespace SwarmApi.Services
{
    public interface ISecretService
    {
        Task<IActionResult> GetSecretsAsync();
        Task<IActionResult> GetSecretByNameAsync(string name);
        Task<IActionResult> GetSecretByIdAsync(string id);
        Task<IActionResult> CreateSecretAsync(SecretParameters secretDto);
        Task<IActionResult> DeleteSecretAsync(string id);
    }

    public class SecretService : Service, ISecretService
    {
        private readonly ISwarmClient _swarmClient;

        public SecretService(ISwarmClient swarmClient, ILoggerFactory loggerFactory) : base(loggerFactory)
        {
            _swarmClient = swarmClient;
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
                return CreateErrorResponse(ex, "Cannot fetch information about secrets.");
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

        public async Task<IActionResult> CreateSecretAsync(SecretParameters secretDto)
        {
            try
            {
                var validator = new SecretValidator();
                validator.Validate(secretDto);
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
                return CreateErrorResponse(ex, "Cannot create secret.");
            }
        }

        public async Task<IActionResult> DeleteSecretAsync(string id)
        {
            try
            {
                ValidateId(id);
                await _swarmClient.DeleteSecret(id);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Cannot delete secret with empty id.");
                return BadRequest(ex.Message);
            }
            catch(DockerApiException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                _logger.LogInformation($"Service with id: {id} not found.");
                return NotFound();
            }
            catch(Exception ex)
            {
                return CreateErrorResponse(ex, $"Cannot delete {id} secret.");
            }
        }

        private static void ValidateId(string id)
        {
            if (id.IsNullOrEmpty())
            {
                throw new ArgumentException("id cannot be null.");
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