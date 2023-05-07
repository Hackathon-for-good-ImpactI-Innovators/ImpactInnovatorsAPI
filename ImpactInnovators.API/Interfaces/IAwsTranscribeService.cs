using Amazon.Runtime;
using Amazon.TranscribeService;

namespace ImpactInnovators.API.Interfaces
{
    public interface IAwsTranscribeService
    {
        Task<AmazonWebServiceResponse> StartTranscriptionJobAsync(string jobName, string bucketPath, string fileName, string? userName, MediaFormat? mediaFormat, LanguageCode? languageCode, string? vocabularyName);
    }
}
