using SwarmApi.Enums;

namespace SwarmApi.Filters
{
    public class NodeFilterParameters
    {
        public string Hostname { get; set; }
        public SwarmRole Role { get; set; }
    }
}