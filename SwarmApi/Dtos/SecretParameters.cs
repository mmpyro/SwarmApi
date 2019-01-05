using System.Collections.Generic;

namespace SwarmApi.Dtos
{
    public class SecretParameters
    {
        public string Content { get; set; }
        public string Name { get; set; }
        public Dictionary<string, string> Labels { get; set; }

        public SecretParameters() => Labels = new Dictionary<string, string>();
    }
}