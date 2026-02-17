using OneInc.Processing.Api.Domain;

namespace OneInc.Processing.Api.Interfaces;

/// <summary>
/// Generates sorted unique-character statistics for an input.
/// </summary>
public interface ICharacterAggregationService
{
    /// <summary>
    /// Produces sorted unique characters with their counts.
    /// </summary>
    /// <param name="input">Text entered by the user.</param>
    /// <returns>A sorted list of character-count pairs.</returns>
    IReadOnlyList<CharacterCount> Aggregate(string input);
}
