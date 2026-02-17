namespace OneInc.Processing.Api.Domain.Contracts;

/// <summary>
/// Lightweight error payload returned by processing endpoints.
/// </summary>
/// <param name="Message">Human-readable error message.</param>
public sealed record ProblemResponse(string Message);
