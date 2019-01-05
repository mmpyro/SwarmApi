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
using SwarmApi.Controllers;

namespace WebApiSpec
{
    public class NodeControllerSpec
    {
        private readonly Fixture _any = new Fixture();
        private readonly ILoggerFactory _loggerFactory;
        private readonly ISwarmClient _swarmClient;

        public NodeControllerSpec()
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
            var nodeController = new NodeController(nodeService);

            //When
            var response = await nodeController.GetNode(null , SwarmRole.Unknown);
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
            var nodeController = new NodeController(nodeService);

            //When
            var response = await nodeController.GetNode(null,SwarmRole.Manager);
            var jsonResult = response as JsonResult;
            var value = jsonResult?.Value as IEnumerable<NodeListResponse>;

            //Then
            Assert.NotNull(jsonResult);
            Assert.NotNull(value);
            Assert.Equal(200, jsonResult.StatusCode);
            Assert.Equal(1, value.Count());
        }

        [Fact]
        public async Task ShouldReturnAllNodesWithSpecifiedNameWhenGetNodesCalled()
        {
            //Given
            const string hostname = "node1";
            var desc = new NodeDescription();
            desc.Hostname = hostname;
            _swarmClient.GetNodes().Returns(x => {
                return Task.FromResult<IEnumerable<NodeListResponse>>(new []{_any.Create<NodeListResponse>(),
                _any.Build<NodeListResponse>().With(t => t.Description, desc).Create() });
                
            });
            var nodeService = new NodeService(_swarmClient, _loggerFactory);
            var nodeController = new NodeController(nodeService);

            //When
            var response = await nodeController.GetNode(hostname, SwarmRole.Unknown);
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
            var nodeController = new NodeController(nodeService);

            //When
            var response = await nodeController.GetNode(null, SwarmRole.Unknown);
            var result = response as ContentResult;

            //Then
            Assert.NotNull(result);
            Assert.Equal(500, result.StatusCode);
        }
    }
}