using System.Text;
using OneInc.Processing.Api.Domain;
using OneInc.Processing.Api.Interfaces;

namespace OneInc.Processing.Api.Services;

/// <summary>
/// Aggregates Unicode characters and sorts them by code point.
/// </summary>
public sealed class CharacterAggregationService : ICharacterAggregationService
{
    /// <inheritdoc />
    public IReadOnlyList<CharacterCount> Aggregate(string input)
    {
        ArgumentNullException.ThrowIfNull(input);

        // Rune was introduced in .net core 3.0, and it can handle emojis as well 😅
        // We are having a Dictionary with the Rune as the key to easily store and increment the counter
        
        var counts = new Dictionary<Rune, int>();
        foreach (var rune in input.EnumerateRunes())
        {
            counts[rune] = counts.GetValueOrDefault(rune) + 1;
        }
        
        // Returning a new array of CharacterCount which holds the string value and its count
        return counts
            .OrderBy(entry => entry.Key.Value)
            .Select(entry => new CharacterCount(entry.Key.ToString(), entry.Value))
            .ToArray();
    }
}
