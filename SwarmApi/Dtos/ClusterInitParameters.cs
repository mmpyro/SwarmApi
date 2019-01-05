namespace SwarmApi.Dtos
{
    public class ClusterInitParameters
    {
        public bool ForceNewCluster { get; set; }
        public string ListenAddress { get; set; }
        public string AdvertiseAddress { get; set; }
    }
}