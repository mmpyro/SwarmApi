namespace SwarmApi.Validators
{
    public interface IValidator<T>
    {
         void Validate(T value);
    }
}