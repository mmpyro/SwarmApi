using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Docker.DotNet;
using Docker.DotNet.Models;
using Microsoft.Extensions.Options;
using SwarmApi.Dtos;
using SwarmApi.Exceptions;

namespace SwarmApi.Clients
{
    public interface ISwarmClient
    {
        Task<IEnumerable<NodeListResponse>> GetNodes();
        Task<IEnumerable<SwarmService>> GetServices();
        Task<IEnumerable<Secret>> GetSecrets();
        Task<SecretCreateResponse> CreateSecret(SecretSpec body);
        Task DeleteSecret(string id);
    }

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
    }
}