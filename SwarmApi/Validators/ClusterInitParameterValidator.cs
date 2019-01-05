using System;
using System.Net;
using SwarmApi.Dtos;

namespace SwarmApi.Validators
{
    public class ClusterInitParameterValidator : IValidator<ClusterInitParameters>
    {
        public void Validate(ClusterInitParameters value)
        {
            CheckIP(value?.AdvertiseAddress, GetParameterName(() => nameof(value.AdvertiseAddress)));
            CheckIP(value?.ListenAddress, GetParameterName(() => nameof(value.ListenAddress)));
        }

        private void CheckIP(string ip, string parameterName)
        {
            if(string.IsNullOrEmpty(ip))
            {
                throw new ArgumentException($"Parameter ${parameterName} is required.");
            }

            if (!IPAddress.TryParse(ip, out IPAddress address))
            {
                throw new ArgumentException($"{parameterName} with value {ip} is not valid ip adress.");
            }
        }

        private string GetParameterName(Func<string> func)
        {
            try
            {
                return func.Invoke();
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}