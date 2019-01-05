using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SwarmApi.Clients;

namespace SwarmApi.Services
{
    public interface ITaskService
    {
        Task<IActionResult> GetTasksAsync();
    }

    public class TaskService : Service, ITaskService
    {
        private readonly ISwarmClient _swarmClient;

        public TaskService(ISwarmClient swarmClient, ILoggerFactory loggerFactory) : base(loggerFactory)
        {
            _swarmClient = swarmClient;
        }

        public async Task<IActionResult> GetTasksAsync()
        {
            try
            {
                var tasks = await _swarmClient.GetTasksAsync();
                return Json(tasks);
            }
            catch(Exception ex)
            {
                return CreateErrorResponse(ex, "Cannot fetch information about tasks.");
            }
        }
    }
}