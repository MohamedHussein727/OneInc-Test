namespace OneInc.Processing.Api.Infrastructure;

/// <summary>
/// Configuration knobs that protect the system from abusive or accidental heavy requests.
/// </summary>
public sealed class ProcessingOptions
{
    /// <summary>
    /// The configuration section name.
    /// </summary>
    public const string SectionName = "Processing";

    /// <summary>
    /// Maximum accepted input length in UTF-16 code units for Unicode support.
    /// </summary>
    public int MaxInputLength { get; init; } = 2_000;
}
