using ImpactInnovators.API.Interfaces;
using Amazon;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;

namespace ImpactInnovators.API.Services
{
    public class AwsSecretManager : IAwsSecretManager
    {
        public async Task<string> GetSecret()
        {
            string secretName = "webapi-access-secret";
            string region = "eu-south-2";

            IAmazonSecretsManager client = new AmazonSecretsManagerClient(RegionEndpoint.GetBySystemName(region));

            GetSecretValueRequest request = new GetSecretValueRequest
            {
                SecretId = secretName,
                VersionStage = "AWSCURRENT",
            };

            try
            {
                var response = await client.GetSecretValueAsync(request);
                return response.SecretString;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return string.Empty;
        }
    }
}
