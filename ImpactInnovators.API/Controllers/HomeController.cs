using System.Net;
using ImpactInnovators.API.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace ImpactInnovators.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly IAwsS3bucketService _awsS3BucketService;
        private readonly IAwsTranscribeService _awsTranscribeService;
        private readonly Serilog.ILogger _logger;

        public HomeController(IAwsS3bucketService awsS3BucketService, IAwsTranscribeService awsTranscribeService, Serilog.ILogger logger)
        {
            _awsS3BucketService = awsS3BucketService;
            _awsTranscribeService = awsTranscribeService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok("This api is up and running!");
        }

        [HttpGet("{folder}/{fileName}")]
        public async Task<IActionResult> Get(string folder, string fileName)
        {
            var file = await _awsS3BucketService.GetObjectFromS3BucketAsync(folder, fileName);

            return string.IsNullOrWhiteSpace(file) ? NotFound("File not found") : Ok(file);
        }

        [HttpPost]
        public async Task<IActionResult> Post(IFormFile audioFile, string userName)
        {
            if (audioFile == null) 
                return BadRequest("File must not be null");

            var audioFilename = $"audio-{DateTime.Now.ToString("ddMMyyyy-HHmm")}";

            try
            {
                var fileUploaded = await _awsS3BucketService.UploadFileToS3BucketAsync(audioFile, userName, audioFilename);

                if (fileUploaded)
                {
                    _logger.Information($"{audioFilename} has been uploaded correctly");

                    var transcriptionJobResponse = await _awsTranscribeService.StartTranscriptionJobAsync
                        ($"transcription-job-{DateTime.Now.ToString("ddMMyyyy-HHmm")}", $"https://{Constants.parisBucketName}.{Constants.S3ParisEndpoint}", audioFilename, userName, null, null, null);

                    if (transcriptionJobResponse.HttpStatusCode == HttpStatusCode.OK)
                    {
                        _logger.Information("The transcription file is being processed");

                        return Ok("The transcription file is being processed");
                    }

                    _logger.Information("something went wrong with the transcription file");

                    return BadRequest();
                }

                _logger.Information($"{audioFilename} has not been uploaded");

                return BadRequest();
            }
            catch (Exception ex)
            {
                _logger.Error($"ERROR: {ex.Message}");
                return BadRequest(ex.Message);
            }
        }
    }
}
