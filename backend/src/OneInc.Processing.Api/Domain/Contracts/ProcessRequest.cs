namespace OneInc.Processing.Api.Domain.Contracts;

/// <summary>
/// Represents a user request to process a text input.
/// </summary>
/// <param name="Input">Raw user text that should be transformed by the backend service.</param>
public sealed record ProcessRequest(string Input);
