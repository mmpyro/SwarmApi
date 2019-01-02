namespace SwarmApi.Validators
{
    public interface IValidator<in T>
    {
         void Validate(T value);
    }
}