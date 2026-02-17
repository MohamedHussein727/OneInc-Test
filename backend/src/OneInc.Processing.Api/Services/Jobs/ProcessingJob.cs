using OneInc.Processing.Api.Domain;

namespace OneInc.Processing.Api.Services.Jobs;

/// <summary>
/// In-memory representation of an accepted processing job.
/// </summary>
public sealed class ProcessingJob
{
    private int _streamStarted;

    /// <summary>
    /// Initializes a new job instance.
    /// </summary>
    /// <param name="id">Unique job identifier.</param>
    /// <param name="result">Prepared output payload and metadata.</param>
    public ProcessingJob(Guid id, ProcessingResult result)
    {
        Id = id;
        Result = result;
        CreatedAtUtc = DateTimeOffset.UtcNow;
    }

    /// <summary>
    /// Job identifier used across API calls.
    /// </summary>
    public Guid Id { get; }

    /// <summary>
    /// Cached output payload to stream.
    /// </summary>
    public ProcessingResult Result { get; }

    /// <summary>
    /// Timestamp useful for diagnostics and cleanup.
    /// </summary>
    public DateTimeOffset CreatedAtUtc { get; }

    /// <summary>
    /// Server-side cancellation source for this job.
    /// </summary>
    public CancellationTokenSource CancellationSource { get; } = new();

    /// <summary>
    /// Ensures exactly one stream execution can begin per job.
    /// </summary>
    public bool TryStartStreaming() => Interlocked.CompareExchange(ref _streamStarted, 1, 0) == 0;
}
