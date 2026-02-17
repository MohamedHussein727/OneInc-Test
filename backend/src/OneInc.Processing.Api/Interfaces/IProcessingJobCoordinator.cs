using OneInc.Processing.Api.Domain;
using OneInc.Processing.Api.Services.Jobs;

namespace OneInc.Processing.Api.Interfaces;

/// <summary>
/// Manages the lifecycle of processing jobs for creation, streaming, and cancellation.
/// </summary>
public interface IProcessingJobCoordinator
{
    /// <summary>
    /// Creates a new in-memory processing job.
    /// </summary>
    ProcessingJob CreateJob(ProcessingResult result);

    /// <summary>
    /// Tries to claim a job for streaming.
    /// </summary>
    JobStreamStartResult TryStartStreaming(Guid jobId, out ProcessingJob? job);

    /// <summary>
    /// Requests cancellation for a job.
    /// </summary>
    bool TryCancel(Guid jobId);

    /// <summary>
    /// Removes a terminal job from memory.
    /// </summary>
    void Complete(Guid jobId);
}
