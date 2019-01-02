using System;
using Xunit;
using AutoFixture;
using SwarmApi.Services;
using Microsoft.Extensions.Logging;
using NSubstitute;
using SwarmApi.Clients;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Docker.DotNet.Models;
using SwarmApi.Filters;
using SwarmApi.Enums;

namespace WebApiSpec
{
    public class NodeServiceSpec
    {
        private readonly Fixture _any = new Fixture();
        private readonly ILoggerFactory _loggerFactory;
        private readonly ISwarmClient _swarmClient;

        public NodeServiceSpec()
        {
            _loggerFactory = Substitute.For<ILoggerFactory>();
            _swarmClient = Substitute.For<ISwarmClient>();
        }

        [Fact]
        public async Task ShouldReturnAllNodesInfoWhenGetNodesCalled()
        {
            //Given
            _swarmClient.GetNodes().Returns(x => {
                return Task.FromResult<IEnumerable<NodeListResponse>>(new []{_any.Create<NodeListResponse>(), _any.Create<NodeListResponse>()});
                
            });
            var nodeService = new NodeService(_swarmClient, _loggerFactory);

            //When
            var response = await nodeService.GetNodeAsync(new NodeFilterParameters());
            var jsonResult = response as JsonResult;
            var value = jsonResult?.Value as IEnumerable<NodeListResponse>;

            //Then
            Assert.NotNull(jsonResult);
            Assert.NotNull(value);
            Assert.Equal(200, jsonResult.StatusCode);
            Assert.Equal(2, value.Count());
        }

        [Fact]
        public async Task ShouldReturnAllManagerNodesInfoWhenGetNodesCalled()
        {
            //Given
            var spec = new NodeUpdateParameters();
            spec.Role = "manager";
            _swarmClient.GetNodes().Returns(x => {
                return Task.FromResult<IEnumerable<NodeListResponse>>(new []{_any.Create<NodeListResponse>(),
                _any.Build<NodeListResponse>().With(t => t.Spec, spec).Create() });
                
            });
            var nodeService = new NodeService(_swarmClient, _loggerFactory);

            //When
            var response = await nodeService.GetNodeAsync(new NodeFilterParameters{
                Role = SwarmRole.Manager
            });
            var jsonResult = response as JsonResult;
            var value = jsonResult?.Value as IEnumerable<NodeListResponse>;

            //Then
            Assert.NotNull(jsonResult);
            Assert.NotNull(value);
            Assert.Equal(200, jsonResult.StatusCode);
            Assert.Equal(1, value.Count());
        }

        [Fact]
        public async Task ShouldReturnInternalServerErrorWhenGetNodesCalledAndErrorOccour()
        {
            //Given
            _swarmClient.When(x => {
                x.GetNodes();
            }).Do(_ => { throw new Exception(); });
            var nodeService = new NodeService(_swarmClient, _loggerFactory);

            //When
            var response = await nodeService.GetNodeAsync(new NodeFilterParameters());
            var result = response as ContentResult;

            //Then
            Assert.NotNull(result);
            Assert.Equal(500, result.StatusCode);
        }
    }
}