using System;
using System.Runtime.Serialization;

namespace SwarmApi.Exceptions
{
    public class EstablishConnectionException : Exception, ISerializable
    {
        public EstablishConnectionException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}