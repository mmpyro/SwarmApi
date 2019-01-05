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
using SwarmApi;

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
            MapperSetup.Init();
        }

        [Fact]
        public async Task ShouldReturnAllServicesInfoWhenGetServicesCalled()
        {
            //Given
            _swarmClient.GetServices().Returns(x => {
                return Task.FromResult<IEnumerable<SwarmService>>(new []{_any.Create<SwarmService>(), _any.Create<SwarmService>()});
                
            });
            var swarmService = new SwarmApi.Services.SwarmService(_swarmClient, _loggerFactory);
            var serviceController = new SwarmController(swarmService);

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
            var serviceController = new SwarmController(swarmService);

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
            var serviceController = new SwarmController(swarmService);

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
            var serviceController = new SwarmController(swarmService);

            //When
            var response = await serviceController.DeleteService(null);
            var result = response as ContentResult;

            //Then
            Assert.NotNull(result);
            Assert.Equal(400, result.StatusCode);
        }

        [Fact]
        public async Task ShouldReturnOkWhenLeaveClusterCalledWithoutErrors()
        {
            //Given
            var swarmService = new SwarmApi.Services.SwarmService(_swarmClient, _loggerFactory);
            var serviceController = new SwarmController(swarmService);

            //When
            var response = await serviceController.LeaveCluster();
            var result = response as ContentResult;

            //Then
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
        }
        
        [Fact]
        public async Task ShouldReturnInternalServerErrorWhenLeaveClusterCalledWithError()
        {
            //Given
            _swarmClient.When(x => x.LeaveCluster(Arg.Any<bool>())).Do( _ => throw new ArgumentException(""));
            var swarmService = new SwarmApi.Services.SwarmService(_swarmClient, _loggerFactory);
            var serviceController = new SwarmController(swarmService);

            //When
            var response = await serviceController.LeaveCluster();
            var result = response as ContentResult;

            //Then
            Assert.NotNull(result);
            Assert.Equal(500, result.StatusCode);
        }

        
        [Fact]
        public async Task ShouldReturnSystemInfoWhenInspectCluster()
        {
            //Given
            _swarmClient.GetSwarmInfo().Returns(Task.FromResult(_any.Create<SwarmInspectResponse>()));
            var swarmService = new SwarmApi.Services.SwarmService(_swarmClient, _loggerFactory);
            var serviceController = new SwarmController(swarmService);

            //When
            var response = await serviceController.InspectCluster();
            var result = response as JsonResult;

            //Then
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
        }
        
        [Fact]
        public async Task ShouldReturnInternalServerErrorWhenInspectClusterAndErrorOccur()
        {
            //Given
            _swarmClient.When( x => x.GetSwarmInfo()).Do(_ => throw new ArgumentException(""));
            var swarmService = new SwarmApi.Services.SwarmService(_swarmClient, _loggerFactory);
            var serviceController = new SwarmController(swarmService);

            //When
            var response = await serviceController.InspectCluster();
            var result = response as ContentResult;

            //Then
            Assert.NotNull(result);
            Assert.Equal(500, result.StatusCode);
        }

        [Fact]
        public async Task ShouldReturnClusterIdWhenInitClusterCalled()
        {
            //Given
            const string clusterId = "1234";
            _swarmClient.InitCluster(Arg.Any<SwarmInitParameters>()).Returns(Task.FromResult(clusterId));
            var swarmService = new SwarmApi.Services.SwarmService(_swarmClient, _loggerFactory);
            var serviceController = new SwarmController(swarmService);

            //When
            var response = await serviceController.InitCluster(new SwarmApi.Dtos.ClusterInitParameters{
                AdvertiseAddress = "192.168.0.101",
                ListenAddress = "192.168.0.101"
            });
            var result = response as JsonResult;
            var content = result.Value;

            //Then
            Assert.NotNull(result);
            string id = content.GetType().GetProperty("Id").GetValue(content, null).ToString();
            Assert.Equal(200, result.StatusCode);
            Assert.Equal(clusterId, id);
        }

        [Fact]
        public async Task ShouldReturnInternalServerErrorWhenInitClusterCalledWithError()
        {
            //Given
            _swarmClient.When(x =>  x.InitCluster(Arg.Any<SwarmInitParameters>())).Do(_ => throw new InvalidCastException());
            var swarmService = new SwarmApi.Services.SwarmService(_swarmClient, _loggerFactory);
            var serviceController = new SwarmController(swarmService);

            //When
            var response = await serviceController.InitCluster(new SwarmApi.Dtos.ClusterInitParameters{
                AdvertiseAddress = "192.168.0.101",
                ListenAddress = "192.168.0.101"
            });
            var result = response as ContentResult;

            //Then
            Assert.NotNull(result);
            Assert.Equal(500, result.StatusCode);
        }

        [Theory]
        [InlineData("a", "192.168.0.101", "AdvertiseAddress with value a is not valid ip adress.")]
        [InlineData("192.168.0.101", "b", "ListenAddress with value b is not valid ip adress.")]
        [InlineData("192.168.0.101", null, "Parameter ListenAddress is required.")]
        [InlineData("", "192.168.0.101","Parameter AdvertiseAddress is required.")]
        public async Task ShouldReturnBadRequestWhenIpIsNotValidAddress(string adverIp, string listenIp, string message)
        {
            //Given
            var swarmService = new SwarmApi.Services.SwarmService(_swarmClient, _loggerFactory);
            var serviceController = new SwarmController(swarmService);

            //When
            var response = await serviceController.InitCluster(new SwarmApi.Dtos.ClusterInitParameters{
                AdvertiseAddress = adverIp,
                ListenAddress = listenIp
            });
            var result = response as ContentResult;

            //Then
            Assert.NotNull(result);
            Assert.Equal(400, result.StatusCode);
            Assert.Equal(message, result.Content);
        }
    }
}