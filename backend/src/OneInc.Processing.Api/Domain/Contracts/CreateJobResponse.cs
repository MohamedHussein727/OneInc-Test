namespace OneInc.Processing.Api.Domain.Contracts;

/// <summary>
/// Represents the metadata returned after the backend accepts a processing job.
/// </summary>
/// <param name="JobId">Unique server-side identifier for cancellation and streaming.</param>
/// <param name="TotalCharacters">Expected number of streamed Unicode characters.</param>
public sealed record CreateJobResponse(Guid JobId, int TotalCharacters);
