using System.Threading.RateLimiting;
using OneInc.Processing.Api.Interfaces;
using OneInc.Processing.Api.Services.Jobs;
using OneInc.Processing.Api.Infrastructure;
using OneInc.Processing.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.Configure<ProcessingOptions>(
    builder.Configuration.GetSection(ProcessingOptions.SectionName));

//These are stateless services which only compute values so it's safe to have them as Singleton
builder.Services.AddSingleton<ICharacterAggregationService, CharacterAggregationService>();
builder.Services.AddSingleton<IProcessingOutputComposer, ProcessingOutputComposer>();
builder.Services.AddSingleton<ICharacterDelayStrategy, RandomCharacterDelayStrategy>();
builder.Services.AddScoped<IProcessingStreamService, ProcessingStreamService>();
builder.Services.AddSingleton<IProcessingJobCoordinator, ProcessingJobCoordinator>();

// Trying it, if it works, it works :D
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddPolicy("processing", context =>
    {
        var partitionKey = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        return RateLimitPartition.GetFixedWindowLimiter(
            partitionKey,
            static _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 10,
                Window = TimeSpan.FromMinutes(1),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 2
            });
    });
});

var app = builder.Build();

app.UseExceptionHandler();
app.UseRateLimiter();
app.MapControllers();

app.Run();

/// <summary>
/// Entry-point marker type used by integration tests.
/// </summary>
public partial class Program;
