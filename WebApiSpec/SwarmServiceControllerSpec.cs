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
using SwarmService = Docker.DotNet.Models.SwarmService;
using SwarmApi.Controllers;

namespace WebApiSpec
{
    public class SwarmServiceControllerSpec
    {
        private readonly Fixture _any = new Fixture();
        private readonly ILoggerFactory _loggerFactory;
        private readonly ISwarmClient _swarmClient;

        public SwarmServiceControllerSpec()
        {
            _loggerFactory = Substitute.For<ILoggerFactory>();
            _swarmClient = Substitute.For<ISwarmClient>();
        }

        [Fact]
        public async Task ShouldReturnAllServicesInfoWhenGetServicesCalled()
        {
            //Given
            _swarmClient.GetServices().Returns(x => {
                return Task.FromResult<IEnumerable<SwarmService>>(new []{_any.Create<SwarmService>(), _any.Create<SwarmService>()});
                
            });
            var swarmService = new SwarmApi.Services.SwarmService(_swarmClient, _loggerFactory);
            var serviceController = new ServiceController(swarmService);

            //When
            var response = await serviceController.GetService();
            var jsonResult = response as JsonResult;
            var value = jsonResult?.Value as IEnumerable<SwarmService>;

            //Then
            Assert.NotNull(jsonResult);
            Assert.NotNull(value);
            Assert.Equal(200, jsonResult.StatusCode);
            Assert.Equal(2, value.Count());
        }

        [Fact]
        public async Task ShouldReturnInternalServerErrorWhenGetServicesCalledAndErrorOccour()
        {
            //Given
            _swarmClient.When(x => {
                x.GetServices();
            }).Do(_ => { throw new Exception(); });
            var swarmService = new SwarmApi.Services.SwarmService(_swarmClient, _loggerFactory);
            var serviceController = new ServiceController(swarmService);

            //When
            var response = await serviceController.GetService();
            var result = response as ContentResult;

            //Then
            Assert.NotNull(result);
            Assert.Equal(500, result.StatusCode);
        }

        [Fact]
        public async Task ShouldDeleteServiceBasedOnIDWhenDeleteCalled()
        {
            //Given
            const string id = "1234";
            var swarmService = new SwarmApi.Services.SwarmService(_swarmClient, _loggerFactory);
            var serviceController = new ServiceController(swarmService);

            //When
            var response = await serviceController.DeleteService(id);
            var result = response as ContentResult;

            //Then
            Assert.NotNull(result);
            Assert.Equal(204, result.StatusCode);
        }

        [Fact]
        public async Task ShouldReturnBadRequestWhenDeleteMethodCalledWithEmptyId()
        {
            //Given
            var swarmService = new SwarmApi.Services.SwarmService(_swarmClient, _loggerFactory);
            var serviceController = new ServiceController(swarmService);

            //When
            var response = await serviceController.DeleteService(null);
            var result = response as ContentResult;

            //Then
            Assert.NotNull(result);
            Assert.Equal(400, result.StatusCode);
        }
    }
}