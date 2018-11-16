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
    }
}