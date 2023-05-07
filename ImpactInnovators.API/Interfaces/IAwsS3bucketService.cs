namespace ImpactInnovators.API.Interfaces
{
    public interface IAwsS3bucketService
    {
        Task<bool> UploadFileToS3BucketAsync(IFormFile audioFile, string userName, string fileName);

        Task<string> GetObjectFromS3BucketAsync(string folder, string fileName);
    }
}
