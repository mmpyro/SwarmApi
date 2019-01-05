using Microsoft.AspNetCore.Mvc;
using SwarmApi.Services;

namespace SwarmApi.Controllers
{
    public class ClusterController : Controller
    {
        private readonly ISwarmService _swarmService;

        public ClusterController(ISwarmService swarmService)
        {
            _swarmService = swarmService;
        }
    }
}