namespace OneInc.Processing.Api.Interfaces;

/// <summary>
/// Produces per-character delay values for simulated long-running work.
/// </summary>
public interface ICharacterDelayStrategy
{
    /// <summary>
    /// Returns the next delay duration.
    /// </summary>
    TimeSpan NextDelay();
}
