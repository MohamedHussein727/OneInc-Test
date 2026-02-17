namespace OneInc.Processing.Api.Domain;

/// <summary>
/// Holds the final processed output and metadata used by the streaming endpoint.
/// </summary>
/// <param name="Output">The generated output string that will be streamed to the client.</param>
/// <param name="CharacterCount">The number of Unicode characters that the client should expect.</param>
public sealed record ProcessingResult(string Output, int CharacterCount);
