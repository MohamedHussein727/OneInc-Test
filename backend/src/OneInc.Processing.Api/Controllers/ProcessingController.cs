using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using OneInc.Processing.Api.Interfaces;
using OneInc.Processing.Api.Services.Jobs;
using OneInc.Processing.Api.Domain.Contracts;
using OneInc.Processing.Api.Infrastructure;

namespace OneInc.Processing.Api.Controllers;

/// <summary>
/// Controller entry point for processing operations.
/// </summary>
[ApiController]
[Route("api/processing")]
[EnableRateLimiting("processing")]
public sealed class ProcessingController(
    IOptions<ProcessingOptions> options,
    IProcessingOutputComposer outputComposer,
    IProcessingJobCoordinator jobCoordinator,
    IProcessingStreamService processingStreamService,
    ILogger<ProcessingController> logger) : ControllerBase
{
    /// <summary>
    /// Creates a processing job and returns its unique identifier.
    /// </summary>
    [HttpPost("jobs")]
    [ProducesResponseType(typeof(CreateJobResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<CreateJobResponse> CreateJob([FromBody] ProcessRequest request)
    {
        var input = request.Input ?? string.Empty;
        if (string.IsNullOrWhiteSpace(input))
        {
            return ValidationProblem(new ValidationProblemDetails(new Dictionary<string, string[]>
            {
                [nameof(ProcessRequest.Input)] = ["Input is required."]
            }));
        }

        if (input.Length > options.Value.MaxInputLength)
        {
            return ValidationProblem(new ValidationProblemDetails(new Dictionary<string, string[]>
            {
                [nameof(ProcessRequest.Input)] = [$"Input length must not exceed {options.Value.MaxInputLength} characters."]
            }));
        }

        var result = outputComposer.Compose(input);
        // Now we have created the output string so we will create a Job that holds this information
        var job = jobCoordinator.CreateJob(result);

        return CreatedAtAction(
            nameof(StreamJob),
            new { jobId = job.Id },
            new CreateJobResponse(job.Id, result.CharacterCount));
    }

    /// <summary>
    /// Streams job output one character at a time.
    /// </summary>
    [HttpGet("jobs/{jobId:guid}/stream")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemResponse), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemResponse), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> StreamJob(Guid jobId, CancellationToken cancellationToken)
    {
        // Check what is the status of the Job
        var startResult = jobCoordinator.TryStartStreaming(jobId, out var job);

        if (startResult is JobStreamStartResult.NotFound)
        {
            return NotFound(new ProblemResponse("Processing job was not found."));
        }

        if (startResult is JobStreamStartResult.AlreadyStarted)
        {
            return Conflict(new ProblemResponse("Processing job already started streaming."));
        }

        ArgumentNullException.ThrowIfNull(job);

        Response.ContentType = "text/plain; charset=utf-8";
        // It tells the client if the page was refreshed to request GET again
        Response.Headers.CacheControl = "no-store";

        using var linkedSource = CancellationTokenSource.CreateLinkedTokenSource(
            cancellationToken,
            job.CancellationSource.Token);

        try
        {
            await processingStreamService.StreamAsync(job.Result.Output, Response.Body, linkedSource.Token);
            return new EmptyResult();
        }
        catch (OperationCanceledException) when (job.CancellationSource.IsCancellationRequested)
        {
            logger.LogInformation("Job {JobId} canceled by explicit cancel request.", jobId);
            return new EmptyResult();
        }
        finally
        {
            jobCoordinator.Complete(jobId);
        }
    }

    /// <summary>
    /// Cancels an existing processing job.
    /// </summary>
    [HttpDelete("jobs/{jobId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemResponse), StatusCodes.Status404NotFound)]
    public IActionResult CancelJob(Guid jobId)
    {
        if (!jobCoordinator.TryCancel(jobId))
        {
            return NotFound(new ProblemResponse("Processing job was not found."));
        }

        return NoContent();
    }
}
