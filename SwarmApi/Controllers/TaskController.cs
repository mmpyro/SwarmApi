using System.Collections.Generic;
using System.Threading.Tasks;
using Docker.DotNet.Models;
using Microsoft.AspNetCore.Mvc;
using SwarmApi.Services;

namespace SwarmApi.Controllers
{
    [Route("api/[controller]")]
    public class TaskController : Controller
    {
        private readonly ITaskService _taskService;

        public TaskController(ITaskService taskService)
        {
            _taskService = taskService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<TaskResponse>), 200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetTasks()
        {
            return await _taskService.GetTasksAsync();
        }
    }
}