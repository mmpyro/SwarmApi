using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult> GetSecrets()
        {
            return await _secretService.GetSecretsAsync();
        }
    }
}