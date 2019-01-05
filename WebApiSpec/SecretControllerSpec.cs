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
using SwarmApi.Controllers;
using SwarmApi.Dtos;
using SwarmApi.Services;
using SwarmApi.Validators;
using Xunit;

namespace WebApiSpec
{
    public class SecretControllerSpec
    {
        private readonly Fixture _any = new Fixture();
        private readonly ILoggerFactory _loggerFactory;
        private readonly ISwarmClient _swarmClient;

        public SecretControllerSpec()
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
            var secretController = new SecretController(secretService);

            //When
            var response = await secretController.GetSecrets();
            var jsonResult = response as JsonResult;
            var value = jsonResult?.Value as IEnumerable<Secret>;

            //Then
            Assert.NotNull(jsonResult);
            Assert.NotNull(value);
            Assert.Equal(200, jsonResult.StatusCode);
            Assert.Equal(2, value.Count());
        }

        [Fact]
        public async Task ShouldReturnSingleSecretInfoWhenGetSecretCalled()
        {
            //Given
            const string secretName = "api";
            var spec = new SecretSpec{Name = secretName};
            _swarmClient.GetSecrets().Returns(x => {
                return Task.FromResult<IEnumerable<Secret>>(new []{_any.Create<Secret>(),
                _any.Build<Secret>().With(t => t.Spec, spec).Create()
                 
                });
            });
            var secretService = new SecretService(_swarmClient, _loggerFactory);
            var secretController = new SecretController(secretService);

            //When
            var response = await secretController.GetSecret(secretName);
            var jsonResult = response as JsonResult;
            var value = jsonResult?.Value as Secret;

            //Then
            Assert.NotNull(jsonResult);
            Assert.NotNull(value);
            Assert.Equal(200, jsonResult.StatusCode);
        }

        [Fact]
        public async Task ShouldReturnSingleSecretInfoWhenGetSecretByIdCalled()
        {
            //Given
            const string id = "1234";
            _swarmClient.GetSecrets().Returns(x => {
                return Task.FromResult<IEnumerable<Secret>>(new []{_any.Create<Secret>(),
                _any.Build<Secret>().With(t => t.ID, id).Create()
                 
                });
            });
            var secretService = new SecretService(_swarmClient, _loggerFactory);
            var secretController = new SecretController(secretService);

            //When
            var response = await secretController.GetSecretById(id);
            var jsonResult = response as JsonResult;
            var value = jsonResult?.Value as Secret;

            //Then
            Assert.NotNull(jsonResult);
            Assert.NotNull(value);
            Assert.Equal(200, jsonResult.StatusCode);
        }

        [Fact]
        public async Task ShouldReturnNotFoundWhenGetSecretCalledDoesNotMatchPredicate()
        {
            //Given
            const string secretName = "api";
            var spec = new SecretSpec{Name = secretName};
            _swarmClient.GetSecrets().Returns(x => {
                return Task.FromResult<IEnumerable<Secret>>(new []{_any.Create<Secret>(),
                _any.Build<Secret>().With(t => t.Spec, spec).Create()
                 
                });
            });
            var secretService = new SecretService(_swarmClient, _loggerFactory);
            var secretController = new SecretController(secretService);

            //When
            var response = await secretController.GetSecret("user-service");
            var result = response as ContentResult;

            //Then
            Assert.NotNull(result);
            Assert.Equal(404, result.StatusCode);
        }

        [Fact]
        public async Task ShouldCreateNewSecretWhenCreateSecretCalled()
        {
            //Given
            const string secretName = "test-secret";
            _swarmClient.CreateSecret(Arg.Any<SecretSpec>()).Returns(x => _any.Create<SecretCreateResponse>());
            var secretService = new SecretService(_swarmClient, _loggerFactory);
            var secretController = new SecretController(secretService);

            //When
            var response = await secretController.CreateSecret(new SwarmApi.Dtos.SecretParameters{
                Content = _any.Create<string>(),
                Name = secretName
            });
            var result = response as CreatedResult;

            //Then
            Assert.NotNull(result);
            Assert.Equal($"/api/secret/{secretName}", result.Location);
        }

        [Fact]
        public async Task ShouldReturnBadRequestWhenContentIsEmpty()
        {
            //Given
            const string secretName = "test-secret";
            _swarmClient.CreateSecret(Arg.Any<SecretSpec>()).Returns(x => _any.Create<SecretCreateResponse>());
            var secretService = new SecretService(_swarmClient, _loggerFactory);
            var secretController = new SecretController(secretService);

            //When
            var response = await secretController.CreateSecret(new SwarmApi.Dtos.SecretParameters{
                Content = string.Empty,
                Name = secretName
            });
            var result = response as ContentResult;

            //Then
            Assert.NotNull(result);
            Assert.Equal(400, result.StatusCode);
            Assert.Equal("Content field cannot be empty.", result.Content);
        }

        [Fact]
        public async Task ShouldReturnBadRequestWhenNameIsEmpty()
        {
            //Given
            _swarmClient.CreateSecret(Arg.Any<SecretSpec>()).Returns(x => _any.Create<SecretCreateResponse>());
            var secretService = new SecretService(_swarmClient, _loggerFactory);
            var secretController = new SecretController(secretService);

            //When
            var response = await secretController.CreateSecret(new SwarmApi.Dtos.SecretParameters{
                Content = _any.Create<string>(),
                Name = string.Empty
            });
            var result = response as ContentResult;

            //Then
            Assert.NotNull(result);
            Assert.Equal(400, result.StatusCode);
            Assert.Equal("Name field cannot be empty.", result.Content);
        }

        [Fact]
        public async Task ShouldDeleteSecretWhenDeleteSecretCalled()
        {
            //Given
            _swarmClient.DeleteSecret(Arg.Any<string>()).Returns(x => Task.CompletedTask);
            var secretService = new SecretService(_swarmClient, _loggerFactory);
            var secretController = new SecretController(secretService);

            //When
            var response = await secretController.DeleteSecret(_any.Create<string>());
            var result = response as ContentResult;

            //Then
            Assert.NotNull(result);
            Assert.Equal(204, result.StatusCode);
            await _swarmClient.Received(1).DeleteSecret(Arg.Any<string>());
        }

        [Fact]
        public async Task ShouldReturnBadRequestWhenDeleteSecretCalledWithEmptyId()
        {
            //Given
            _swarmClient.DeleteSecret(Arg.Any<string>()).Returns(x => Task.CompletedTask);
            var secretService = new SecretService(_swarmClient, _loggerFactory);
            var secretController = new SecretController(secretService);

            //When
            var response = await secretController.DeleteSecret(string.Empty);
            var result = response as ContentResult;

            //Then
            Assert.NotNull(result);
            Assert.Equal(400, result.StatusCode);
        }

        [Fact]
        public async Task ShouldReturnInternalServerErrorWhenGetSecretsCalledAndErrorOccour()
        {
            //Given
            _swarmClient.When(x => {
                x.GetSecrets();
            }).Do(_ => { throw new Exception(); });
            var secretService = new SecretService(_swarmClient, _loggerFactory);
            var secretController = new SecretController(secretService);

            //When
            var response = await secretController.GetSecret("");
            var result = response as ContentResult;

            //Then
            Assert.NotNull(result);
            Assert.Equal(500, result.StatusCode);
        }
    }
}