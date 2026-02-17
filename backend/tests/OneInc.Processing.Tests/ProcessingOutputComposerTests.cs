using OneInc.Processing.Api.Services;

namespace OneInc.Processing.Tests;

/// <summary>
/// Unit tests for <see cref="ProcessingOutputComposer"/>.
/// </summary>
public sealed class ProcessingOutputComposerTests
{
    [Fact]
    public void Compose_GeneratesExpectedOutputForSample()
    {
        var composer = new ProcessingOutputComposer(new CharacterAggregationService());

        var result = composer.Compose("Hello, World!");

        Assert.Equal(" 1!1,1H1W1d1e1l3o2r1/SGVsbG8sIFdvcmxkIQ==", result.Output);
        Assert.Equal(result.Output.EnumerateRunes().Count(), result.CharacterCount);
    }
}
