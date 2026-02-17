using OneInc.Processing.Api.Domain;

namespace OneInc.Processing.Api.Interfaces;

/// <summary>
/// Builds the final output that combines character statistics and Base64 payload.
/// </summary>
public interface IProcessingOutputComposer
{
    /// <summary>
    /// Creates the final output for a given input.
    /// </summary>
    /// <param name="input">Text entered by the user.</param>
    /// <returns>The final output and its Unicode character length.</returns>
    ProcessingResult Compose(string input);
}
