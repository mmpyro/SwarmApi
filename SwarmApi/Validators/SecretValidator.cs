using System;
using SwarmApi.Dtos;
using SwarmApi.Extensions;

namespace SwarmApi.Validators
{
    public class SecretValidator : IValidator<SecretParameters>
    {
        public void Validate(SecretParameters value)
        {
            if(value.Content.IsNullOrEmpty())
            {
                throw new ArgumentException("Content field cannot be empty.");
            }

            if(value.Name.IsNullOrEmpty())
            {
                throw new ArgumentException("Name field cannot be empty.");
            }

        }
    }
}