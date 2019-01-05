using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SwarmApi.Clients;

namespace SwarmApi.Services
{
    public interface ISystemService
    {
        Task<IActionResult> GetVersionAsync();
        Task<IActionResult> GetSystemInfoAsync();
    }

    public class SystemService : Service, ISystemService
    {
        private readonly ISwarmClient _swarmClient;

        public SystemService(ISwarmClient swarmClient, ILoggerFactory loggerFactory) : base(loggerFactory)
        {
            _swarmClient = swarmClient;
        }

        public async Task<IActionResult> GetVersionAsync()
        {
            try
            {
                var response = await _swarmClient.GetVersion();
                _logger.LogInformation("Fetch system version.");
                return Json(response);
            }
            catch(Exception ex)
            {
                return CreateErrorResponse(ex, "Cannot fetch information about system version.");
            }
        }

        public async Task<IActionResult> GetSystemInfoAsync()
        {
            try
            {
                var response = await _swarmClient.GetSystemInfo();
                _logger.LogInformation("Fetch system info.");
                return Json(response);
            }
            catch(Exception ex)
            {
                return CreateErrorResponse(ex, "Cannot fetch information about system.");
            }
        }
    }
}