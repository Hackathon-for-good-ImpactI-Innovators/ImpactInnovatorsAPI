using System.Net;
using ImpactInnovators.API.Interfaces;
using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;
using ImpactInnovators.API.Models;
using System.Text.Json;
using System.Text;
using Amazon.S3.Model;

namespace ImpactInnovators.API.Services
{
    public class AwsS3bucketService : IAwsS3bucketService
    {
        private readonly IAwsSecretManager _awsSecretManager;

        public AwsS3bucketService(IAwsSecretManager awsSecretManager)
        {
            _awsSecretManager = awsSecretManager;
        }

        public async Task<bool> UploadFileToS3BucketAsync(IFormFile audioFile, string? userName, string fileName)
        {
            using (var memorySrteam = new MemoryStream())
            {
                await audioFile.CopyToAsync(memorySrteam);

                var stringSecret = await _awsSecretManager.GetSecret();

                ApiAccessData apiAccessData = JsonSerializer.Deserialize<ApiAccessData>(stringSecret);

                if (apiAccessData == null) return false;

                var s3Client = new AmazonS3Client(apiAccessData.API_ACCESS_KEY, apiAccessData.API_ACCESS_PASSWORD, Constants.RegionParis);

                var transferUtility = new TransferUtility(s3Client);

                var fileTransferUtilityRequest = new TransferUtilityUploadRequest
                {
                    BucketName = Constants.parisBucketName,
                    InputStream = memorySrteam,
                    Key = await CreateFilePath(userName, fileName)
                };

                try
                {
                    transferUtility.Upload(fileTransferUtilityRequest);
                    return true;
                }
                catch (AmazonS3Exception ex)
                {
                    throw new Exception(ex.Message);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }

        public async Task<string> GetObjectFromS3BucketAsync(string folder, string fileName)
        {
            var stringSecret = await _awsSecretManager.GetSecret();

            ApiAccessData apiAccessData = JsonSerializer.Deserialize<ApiAccessData>(stringSecret);

            if (apiAccessData == null) return string.Empty;

            var s3Client = new AmazonS3Client(apiAccessData.API_ACCESS_KEY, apiAccessData.API_ACCESS_PASSWORD, Constants.RegionParis);

            GetObjectRequest request = new GetObjectRequest
            {
                BucketName = Constants.parisBucketName,
                Key = await CreateFilePath(folder, fileName)
            };

            using (GetObjectResponse response = await s3Client.GetObjectAsync(request))
            {
                using (StreamReader reader = new StreamReader(response.ResponseStream))
                {
                    return reader.ReadToEnd();
                }
            }
        }

        private async Task<string> CreateFilePath(string? subfolder, string fileName)
        {
            StringBuilder sb = new StringBuilder();

            if (!string.IsNullOrWhiteSpace(subfolder))
                sb.Append($"{subfolder}/");

            sb.Append(fileName);

            return sb.ToString();
        }
    }
}
