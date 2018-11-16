using System;

namespace SwarmApi.Exceptions
{
    public class EstablishConnectionException : Exception
    {
        public EstablishConnectionException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}