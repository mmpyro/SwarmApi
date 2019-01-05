using Docker.DotNet.Models;
using SwarmApi.Dtos;

namespace SwarmApi
{
    public static class MapperSetup
    {
        private static object _locker = new object();
        private static bool _initialized = false;

        public static void Init()
        {
            if(_initialized == false)
            {
                lock(_locker)
                {
                    if(_initialized == false)
                    {
                        AutoMapper.Mapper.Initialize(config => {
                            config.CreateMap<ClusterInitParameters, SwarmInitParameters>()
                                .ForMember(dest => dest.AdvertiseAddr, opts => opts.MapFrom(src => src.AdvertiseAddress))
                                .ForMember(dest => dest.ListenAddr, opts => opts.MapFrom(src => src.ListenAddress))
                                .ForMember(dest => dest.ForceNewCluster, opts => opts.MapFrom(src => src.ForceNewCluster));
                        });
                        _initialized = true;
                    }
                }
            }
        }
    }
}