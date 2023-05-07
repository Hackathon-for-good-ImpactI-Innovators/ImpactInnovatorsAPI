using Amazon.Runtime;
using Amazon.TranscribeService;
using AWS.Logger;
using AWS.Logger.SeriLog;
using ImpactInnovators.API;
using ImpactInnovators.API.Interfaces;
using ImpactInnovators.API.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

AWSLoggerConfig configuration = new AWSLoggerConfig("ImpactInnovatorAPI/Logs");
configuration.Region = Constants.RegionSpain.SystemName;
var logger = new LoggerConfiguration().WriteTo.AWSSeriLog(configuration).CreateLogger();
builder.Services.AddSingleton<Serilog.ILogger>(logger);

builder.Services.AddTransient<IAwsS3bucketService, AwsS3bucketService>();
builder.Services.AddTransient<IAwsTranscribeService, AwsTranscribeService>();
builder.Services.AddTransient<IAwsSecretManager, AwsSecretManager>();
builder.Services.AddTransient<IAmazonTranscribeService>(s => new AmazonTranscribeServiceClient(
    new BasicAWSCredentials(Environment.GetEnvironmentVariable("API_ACCESS_KEY"), Environment.GetEnvironmentVariable("API_ACCESS_PASSWORD")),
    new AmazonTranscribeServiceConfig { RegionEndpoint = Constants.RegionParis }
    ));


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAnyOrigin",
        builder => builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAnyOrigin");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
