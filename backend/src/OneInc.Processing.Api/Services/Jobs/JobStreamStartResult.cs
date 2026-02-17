namespace OneInc.Processing.Api.Services.Jobs;

/// <summary>
/// Outcome of attempting to start a stream for a job.
/// </summary>
public enum JobStreamStartResult
{
    /// <summary>
    /// The job id is unknown or already released.
    /// </summary>
    NotFound,
    /// <summary>
    /// The job was already claimed by another stream request.
    /// </summary>
    AlreadyStarted,
    /// <summary>
    /// Streaming can begin.
    /// </summary>
    Started
}
