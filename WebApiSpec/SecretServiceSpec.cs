using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using Docker.DotNet.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using SwarmApi.Clients;
using SwarmApi.Services;
using Xunit;

namespace WebApiSpec
{
    public class SecretServiceSpec
    {
         private readonly Fixture _any = new Fixture();
        private readonly ILoggerFactory _loggerFactory;
        private readonly ISwarmClient _swarmClient;

        public SecretServiceSpec()
        {
            _loggerFactory = Substitute.For<ILoggerFactory>();
            _swarmClient = Substitute.For<ISwarmClient>();
        }

        [Fact]
        public async Task ShouldReturnAllSecretInfoWhenGetSecretsCalled()
        {
            //Given
            _swarmClient.GetSecrets().Returns(x => {
                return Task.FromResult<IEnumerable<Secret>>(new []{_any.Create<Secret>(), _any.Create<Secret>()});
                
            });
            var secretService = new SecretService(_swarmClient, _loggerFactory);

            //When
            var response = await secretService.GetSecretsAsync();
            var jsonResult = response as JsonResult;
            var value = jsonResult?.Value as IEnumerable<Secret>;

            //Then
            Assert.NotNull(jsonResult);
            Assert.NotNull(value);
            Assert.Equal(200, jsonResult.StatusCode);
            Assert.Equal(2, value.Count());
        }

        [Fact]
        public async Task ShouldReturnInternalServerErrorWhenGetSecretsCalledAndErrorOccour()
        {
            //Given
            _swarmClient.When(x => {
                x.GetSecrets();
            }).Do(_ => { throw new Exception(); });
            var secretService = new SecretService(_swarmClient, _loggerFactory);

            //When
            var response = await secretService.GetSecretsAsync();
            var result = response as ContentResult;

            //Then
            Assert.NotNull(result);
            Assert.Equal(500, result.StatusCode);
        }
    }
}