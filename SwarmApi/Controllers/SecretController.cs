using System.Collections.Generic;
using System.Threading.Tasks;
using Docker.DotNet.Models;
using Microsoft.AspNetCore.Mvc;
using SwarmApi.Dtos;
using SwarmApi.Services;

namespace SwarmApi.Controllers
{
    [Route("api/[controller]")]
    public class SecretController : Controller
    {
        private readonly ISecretService _secretService;

        public SecretController(ISecretService secretService)
        {
            _secretService = secretService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Secret>), 200)]
        public async Task<IActionResult> GetSecrets()
        {
            return await _secretService.GetSecretsAsync();
        }

        [Route("{name}")]
        [HttpGet]
        [ProducesResponseType(typeof(Secret), 200)]
        public async Task<IActionResult> GetSecret(string name)
        {
            return await _secretService.GetSecretByNameAsync(name);
        }

        [Route("id/{id}")]
        [HttpGet]
        [ProducesResponseType(typeof(Secret), 200)]
        public async Task<IActionResult> GetSecretById(string id)
        {
            return await _secretService.GetSecretByIdAsync(id);
        }

        [HttpPost]
        [ProducesResponseType(typeof(SecretCreateResponse), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> CreateSecret([FromBody] SecretParameters secretDto)
        {
            return await _secretService.CreateSecretAsync(secretDto);
        }

        [Route("{id}")]
        [HttpDelete]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteSecret(string id)
        {
            return await _secretService.DeleteSecretAsync(id);
        }
    }
}