using System.Text;
using OneInc.Processing.Api.Interfaces;

namespace OneInc.Processing.Api.Services;

/// <summary>
/// Simulates heavy backend work by delaying before each streamed character.
/// </summary>
public sealed class ProcessingStreamService(
    ICharacterDelayStrategy delayStrategy,
    ILogger<ProcessingStreamService> logger) : IProcessingStreamService
{
    /// <inheritdoc />
    public async Task StreamAsync(string output, Stream destination, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(output);
        ArgumentNullException.ThrowIfNull(destination);

        foreach (var rune in output.EnumerateRunes())
        {
            await Task.Delay(delayStrategy.NextDelay(), cancellationToken);

            var bytes = Encoding.UTF8.GetBytes(rune.ToString());
            await destination.WriteAsync(bytes, cancellationToken);
            await destination.FlushAsync(cancellationToken);
        }

        logger.LogInformation("Completed streaming {CharacterCount} characters.", output.EnumerateRunes().Count());
    }
}
