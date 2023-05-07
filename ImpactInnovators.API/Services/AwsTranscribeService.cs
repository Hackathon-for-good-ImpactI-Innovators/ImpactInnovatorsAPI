using ImpactInnovators.API.Interfaces;
using Amazon.TranscribeService.Model;
using Amazon.TranscribeService;
using Amazon.Runtime;
using System.Text;

namespace ImpactInnovators.API.Services
{
    public class AwsTranscribeService : IAwsTranscribeService
    {
        private readonly IAmazonTranscribeService _amazonTranscribeService;

        public AwsTranscribeService(IAmazonTranscribeService amazonTranscribeService)
        {
            _amazonTranscribeService = amazonTranscribeService;
        }

        public async Task<AmazonWebServiceResponse> StartTranscriptionJobAsync(string jobName, string bucketPath, string fileName
            , string? userName, MediaFormat? mediaFormat, LanguageCode? languageCode, string? vocabularyName)
        {
            try
            {
                var response = await _amazonTranscribeService.StartTranscriptionJobAsync(
                new StartTranscriptionJobRequest()
                {
                    TranscriptionJobName = jobName,
                    
                    Media = new Media()
                    {
                        MediaFileUri = $"{bucketPath}/{userName}/{fileName}"
                    },
                    MediaFormat = mediaFormat != null ? mediaFormat : MediaFormat.Webm,
                    LanguageCode = languageCode != null ? languageCode : LanguageCode.EsES,
                    Settings = vocabularyName != null ? new Settings()
                    {
                        VocabularyName = vocabularyName
                    } : null,
                    OutputBucketName = Constants.parisBucketName,
                    OutputKey = await CreateOutputKeyPath(userName, fileName)
                });
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private async Task<string> CreateOutputKeyPath(string? subfolder, string fileName)
        {
            StringBuilder sb = new StringBuilder();

            if (!string.IsNullOrWhiteSpace(subfolder))
                sb.Append($"{subfolder}/");

            sb.Append(fileName.Replace("audio", "transcription"));

            return sb.ToString();
        }
    }
}
