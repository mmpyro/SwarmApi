using System;
using System.Threading.Tasks;
using AutoFixture;
using Docker.DotNet.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using SwarmApi;
using SwarmApi.Clients;
using SwarmApi.Controllers;
using Xunit;

namespace WebApiSpec
{
    public class SystemControllerSpec
    {
        private readonly Fixture _any = new Fixture();
        private readonly ILoggerFactory _loggerFactory;
        private readonly ISwarmClient _swarmClient;

        public SystemControllerSpec()
        {
            _loggerFactory = Substitute.For<ILoggerFactory>();
            _swarmClient = Substitute.For<ISwarmClient>();
            MapperSetup.Init();
        }

        [Fact]
        public async Task ShouldReturnSystemInfoResponseWhenGetSystemInfoCalled()
        {
            //Given
            _swarmClient.GetSystemInfo().Returns(Task.FromResult(_any.Create<SystemInfoResponse>()));
            var systemService = new SwarmApi.Services.SystemService(_swarmClient, _loggerFactory);
            var serviceController = new SystemController(systemService);

            //When
            var response = await serviceController.GetSystemInfo();
            var result = response as JsonResult;

            //Then
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
        }

        [Fact]
        public async Task ShouldReturnInternalServerErrorWhenGetSystemInfoCalledWithError()
        {
            //Given
            _swarmClient.When(x =>  x.GetSystemInfo()).Do(_ => throw new ArgumentException());
            var systemService = new SwarmApi.Services.SystemService(_swarmClient, _loggerFactory);
            var serviceController = new SystemController(systemService);

            //When
            var response = await serviceController.GetSystemInfo();
            var result = response as ContentResult;

            //Then
            Assert.NotNull(result);
            Assert.Equal(500, result.StatusCode);
        }

        [Fact]
        public async Task ShouldReturnInternalServerErrorWhenGetVersionCalledWithError()
        {
            //Given
            _swarmClient.When(x =>  x.GetVersion()).Do(_ => throw new ArgumentException());
            var systemService = new SwarmApi.Services.SystemService(_swarmClient, _loggerFactory);
            var serviceController = new SystemController(systemService);

            //When
            var response = await serviceController.GetVersion();
            var result = response as ContentResult;

            //Then
            Assert.NotNull(result);
            Assert.Equal(500, result.StatusCode);
        }

        [Fact]
        public async Task ShouldReturnVersionResponseWhenGetVersionCalled()
        {
            //Given
            _swarmClient.GetVersion().Returns(Task.FromResult(_any.Create<VersionResponse>()));
            var systemService = new SwarmApi.Services.SystemService(_swarmClient, _loggerFactory);
            var serviceController = new SystemController(systemService);

            //When
            var response = await serviceController.GetVersion();
            var result = response as JsonResult;

            //Then
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
        }
    }
}