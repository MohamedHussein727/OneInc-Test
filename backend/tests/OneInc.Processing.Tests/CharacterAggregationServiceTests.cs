using OneInc.Processing.Api.Services;

namespace OneInc.Processing.Tests;

/// <summary>
/// Unit tests for <see cref="CharacterAggregationService"/>.
/// </summary>
public sealed class CharacterAggregationServiceTests
{
    [Fact]
    public void Aggregate_ReturnsSortedCharactersWithCounts()
    {
        var service = new CharacterAggregationService();

        var result = service.Aggregate("Hello, World!").ToArray();

        Assert.Collection(
            result,
            item => Assert.Equal((" ", 1), (item.Character, item.Count)),
            item => Assert.Equal(("!", 1), (item.Character, item.Count)),
            item => Assert.Equal((",", 1), (item.Character, item.Count)),
            item => Assert.Equal(("H", 1), (item.Character, item.Count)),
            item => Assert.Equal(("W", 1), (item.Character, item.Count)),
            item => Assert.Equal(("d", 1), (item.Character, item.Count)),
            item => Assert.Equal(("e", 1), (item.Character, item.Count)),
            item => Assert.Equal(("l", 3), (item.Character, item.Count)),
            item => Assert.Equal(("o", 2), (item.Character, item.Count)),
            item => Assert.Equal(("r", 1), (item.Character, item.Count)));
    }

    [Fact]
    public void Aggregate_HandlesUnicodeRunes()
    {
        var service = new CharacterAggregationService();

        var result = service.Aggregate("A🙂🙂").ToArray();

        Assert.Collection(
            result,
            item => Assert.Equal(("A", 1), (item.Character, item.Count)),
            item => Assert.Equal(("🙂", 2), (item.Character, item.Count)));
    }
}
