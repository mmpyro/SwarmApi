using AutoMapper;
using Docker.DotNet.Models;
using SwarmApi;
using SwarmApi.Dtos;
using Xunit;

namespace WebApiSpec
{
    public class MapperSpec
    {
        public MapperSpec()
        {
            MapperSetup.Init();
        }

        [Fact]
        public void ShouldMapClusterInitParametersToSwarmInitParameters()
        {
            //Given
            const string advertiseAddress = "192.168.0.1";
            const string listenAddress = "0.0.0.0";
            var clusterInitParameters = new ClusterInitParameters
            {
                ForceNewCluster = true,
                AdvertiseAddress = advertiseAddress,
                ListenAddress = listenAddress
            };

            //When
            var swarmInitParameters = Mapper.Map<SwarmInitParameters>(clusterInitParameters);

            //Then
            Assert.True(swarmInitParameters.ForceNewCluster);
            Assert.Equal(advertiseAddress, swarmInitParameters.AdvertiseAddr);
            Assert.Equal(listenAddress, swarmInitParameters.ListenAddr);
        }
    }
}