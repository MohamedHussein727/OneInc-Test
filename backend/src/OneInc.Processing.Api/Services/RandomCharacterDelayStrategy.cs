using OneInc.Processing.Api.Interfaces;

namespace OneInc.Processing.Api.Services;

/// <summary>
/// Produces random delays between one and five seconds.
/// </summary>
public sealed class RandomCharacterDelayStrategy : ICharacterDelayStrategy
{
    /// <inheritdoc />
    public TimeSpan NextDelay()
    {
        var milliseconds = Random.Shared.Next(1_000, 5_000);
        return TimeSpan.FromMilliseconds(milliseconds);
    }
}
