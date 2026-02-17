namespace OneInc.Processing.Api.Interfaces;

/// <summary>
/// Streams output characters over an HTTP response stream with per-character delays.
/// </summary>
public interface IProcessingStreamService
{
    /// <summary>
    /// Writes output to the stream one Unicode character at a time.
    /// </summary>
    /// <param name="output">Final processed output payload.</param>
    /// <param name="destination">HTTP response stream.</param>
    /// <param name="cancellationToken">Request cancellation token.</param>
    Task StreamAsync(string output, Stream destination, CancellationToken cancellationToken);
}
