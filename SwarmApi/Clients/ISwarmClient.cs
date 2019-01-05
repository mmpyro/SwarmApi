using System.Collections.Generic;
using System.Threading.Tasks;
using Docker.DotNet.Models;

namespace SwarmApi.Clients
{
    public interface ISwarmClient
    {
        Task<IEnumerable<NodeListResponse>> GetNodes();
        Task<IEnumerable<SwarmService>> GetServices();
        Task DeleteService(string id);
        Task<IEnumerable<Secret>> GetSecrets();
        Task<SecretCreateResponse> CreateSecret(SecretSpec body);
        Task DeleteSecret(string id);
        Task<VersionResponse> GetVersion();
        Task<SystemInfoResponse> GetSystemInfo();
        Task<string> InitCluster(SwarmInitParameters initParameters);
        Task<SwarmInspectResponse> GetSwarmInfo();
        Task LeaveCluster(bool force = false);
    }
}