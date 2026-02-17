namespace OneInc.Processing.Api.Domain;

/// <summary>
/// Stores how many times a single Unicode character appears in an input string.
/// </summary>
/// <param name="Character">The Unicode character represented as a string.</param>
/// <param name="Count">The number of occurrences in the input.</param>
public sealed record CharacterCount(string Character, int Count);
