using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Docker.DotNet;
using Docker.DotNet.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SwarmApi.Clients;
using SwarmApi.Dtos;
using SwarmApi.Extensions;
using SwarmApi.Validators;

namespace SwarmApi.Services
{
    public interface ISwarmService
    {
        Task<IActionResult> GetServicesAsync();
        Task<IActionResult> DeleteServicesAsync(string id);
        Task<IActionResult> InitSwarmAsync(ClusterInitParameters clusterInit);
        Task<IActionResult> InspectSwarmAsync();
        Task<IActionResult> LeaveSwarmAsync(bool force = false);
    }

    public class SwarmService : Service, ISwarmService
    {
        private readonly ISwarmClient _swarmClient;

        public SwarmService(ISwarmClient swarmClient, ILoggerFactory loggerFactory) : base(loggerFactory)
        {
            _swarmClient = swarmClient;
        }

        public async Task<IActionResult> GetServicesAsync()
        {
            try
            {
                var services = await _swarmClient.GetServices();
                _logger.LogInformation("Fetch swarm services");
                return Json(services.ToArray());
            }
            catch(Exception ex)
            {
                return CreateErrorResponse(ex, "Cannot fetch information about services.");
            }
        }

        public async Task<IActionResult> DeleteServicesAsync(string id)
        {
            try
            {
                if(id.IsNullOrEmpty())
                {
                    throw new ArgumentException("");
                }
                await _swarmClient.DeleteService(id);
                _logger.LogInformation($"Delete service {id}.");
                return NoContent();
            }
            catch(ArgumentException ex)
            {
                _logger.LogError(ex, "Cannot delete service with empty id.");
                return BadRequest(ex.Message);
            }
            catch(DockerApiException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
            {
                _logger.LogInformation($"Service with id: {id} not found.");
                return NotFound();
            }
            catch(Exception ex)
            {
                return CreateErrorResponse(ex, "Cannot delete service.");
            }
        }

        public async Task<IActionResult> InitSwarmAsync(ClusterInitParameters clusterInit)
        {
            try
            {
                var validator = new ClusterInitParameterValidator();
                validator.Validate(clusterInit);
                var parameters = Mapper.Map<SwarmInitParameters>(clusterInit);
                var nodeId = await _swarmClient.InitCluster(parameters);
                return Json(new {Id = nodeId});
            }
            catch(ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch(Exception ex)
            {
                return CreateErrorResponse(ex, "Cannot initialize swarm cluster.");
            }
        }

        public async Task<IActionResult> InspectSwarmAsync()
        {
            try
            {
                var response = await _swarmClient.GetSwarmInfo();
                return Json(response);
            }
            catch(Exception ex)
            {
                return CreateErrorResponse(ex, "Cannot fetch information about swarm status.");
            }
        }

        public async Task<IActionResult> LeaveSwarmAsync(bool force = false)
        {
            try
            {
                await _swarmClient.LeaveCluster(force);
                return Ok();
            }
            catch(Exception ex)
            {
                return CreateErrorResponse(ex, "Cannot leave swarm cluster.");
            }
        }
    }
}