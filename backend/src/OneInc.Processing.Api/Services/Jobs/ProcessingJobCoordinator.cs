using System.Collections.Concurrent;
using OneInc.Processing.Api.Domain;
using OneInc.Processing.Api.Interfaces;

namespace OneInc.Processing.Api.Services.Jobs;

/// <summary>
/// Thread-safe in-memory processing job coordinator.
/// </summary>
public sealed class ProcessingJobCoordinator(ILogger<ProcessingJobCoordinator> logger) : IProcessingJobCoordinator
{
    private readonly ConcurrentDictionary<Guid, ProcessingJob> _jobs = new();

    /// <inheritdoc />
    public ProcessingJob CreateJob(ProcessingResult result)
    {
        var job = new ProcessingJob(Guid.NewGuid(), result);

        if (!_jobs.TryAdd(job.Id, job))
        {
            throw new InvalidOperationException("Failed to register a new processing job.");
        }

        logger.LogInformation(
            "Created processing job {JobId} with {CharacterCount} characters.",
            job.Id,
            result.CharacterCount);

        return job;
    }

    /// <inheritdoc />
    public JobStreamStartResult TryStartStreaming(Guid jobId, out ProcessingJob? job)
    {
        if (!_jobs.TryGetValue(jobId, out job))
        {
            return JobStreamStartResult.NotFound;
        }

        if (!job.TryStartStreaming())
        {
            return JobStreamStartResult.AlreadyStarted;
        }

        return JobStreamStartResult.Started;
    }

    /// <inheritdoc />
    public bool TryCancel(Guid jobId)
    {
        if (!_jobs.TryGetValue(jobId, out var job))
        {
            return false;
        }

        job.CancellationSource.Cancel();
        logger.LogInformation("Cancellation requested for job {JobId}.", jobId);
        return true;
    }

    /// <inheritdoc />
    public void Complete(Guid jobId)
    {
        if (_jobs.TryRemove(jobId, out var job))
        {
            job.CancellationSource.Dispose();
            logger.LogInformation("Released processing job {JobId}.", jobId);
        }
    }
}
