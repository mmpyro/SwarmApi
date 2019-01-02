using System.Collections.Generic;

namespace SwarmApi.Dtos
{
    public class SecretDto
    {
        public string Content { get; set; }
        public string Name { get; set; }
        public Dictionary<string, string> Labels { get; set; }

        public SecretDto() => Labels = new Dictionary<string, string>();
    }
}