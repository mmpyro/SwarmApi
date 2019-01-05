using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Docker.DotNet;
using Docker.DotNet.Models;
using Microsoft.Extensions.Options;
using SwarmApi.Dtos;
using SwarmApi.Exceptions;

namespace SwarmApi.Clients
{
    public class SwarmClient : ISwarmClient
    {
        private readonly DockerClient _client;

        public SwarmClient(IOptions<SwarmConfiguration> configurationProvider)
        {
            try
            {
                var configuration = configurationProvider.Value;
                _client = new DockerClientConfiguration(
                        new Uri(configuration.ApiUrl))
                        .CreateClient();
            }
            catch(Exception ex)
            {
                throw new EstablishConnectionException($"{nameof(SwarmClient)} cannot establish connection.", ex);
            }
        }

        public async Task<IEnumerable<NodeListResponse>> GetNodes()
        {
            return await _client.Swarm.ListNodesAsync();
        }

        public async Task<IEnumerable<SwarmService>> GetServices()
        {
            return await _client.Swarm.ListServicesAsync();
        }

        public async Task DeleteService(string id)
        {
            await _client.Swarm.RemoveServiceAsync(id);
        }

        public async Task<IEnumerable<Secret>> GetSecrets()
        {
            return await _client.Secrets.ListAsync();
        }

        public async Task<SecretCreateResponse> CreateSecret(SecretSpec body)
        {
            return await _client.Secrets.CreateAsync(body);
        }

        public async Task DeleteSecret(string id)
        {
            await _client.Secrets.DeleteAsync(id);
        }

        public async Task<VersionResponse> GetVersion()
        {
            return await _client.System.GetVersionAsync();
        }

        public async Task<SystemInfoResponse> GetSystemInfo()
        {
            return await _client.System.GetSystemInfoAsync();
        }

        public async Task<string> InitCluster(SwarmInitParameters initParameters)
        {
            return await _client.Swarm.InitSwarmAsync(initParameters);
        }

        public async Task<SwarmInspectResponse> GetSwarmInfo()
        {
            return await _client.Swarm.InspectSwarmAsync();
        }

        public async Task LeaveCluster(bool force = false)
        {
            await _client.Swarm.LeaveSwarmAsync(new SwarmLeaveParameters{
                Force = force
            });
        }
    }
}