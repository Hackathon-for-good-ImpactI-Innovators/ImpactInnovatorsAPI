namespace ImpactInnovators.API.Interfaces
{
    public interface IAwsSecretManager
    {
        Task<string> GetSecret();
    }
}
