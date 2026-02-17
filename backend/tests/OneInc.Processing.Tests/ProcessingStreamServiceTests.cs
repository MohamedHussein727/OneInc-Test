using System.Text;
using Microsoft.Extensions.Logging.Abstractions;
using OneInc.Processing.Api.Interfaces;
using OneInc.Processing.Api.Services;

namespace OneInc.Processing.Tests;

/// <summary>
/// Unit tests for <see cref="ProcessingStreamService"/>.
/// </summary>
public sealed class ProcessingStreamServiceTests
{
    [Fact]
    public async Task StreamAsync_WritesCharactersInOrder()
    {
        var streamService = new ProcessingStreamService(new NoDelayStrategy(), NullLogger<ProcessingStreamService>.Instance);
        await using var destination = new MemoryStream();

        await streamService.StreamAsync("Ab🙂", destination, CancellationToken.None);

        destination.Position = 0;
        var payload = Encoding.UTF8.GetString(destination.ToArray());
        Assert.Equal("Ab🙂", payload);
    }

    private sealed class NoDelayStrategy : ICharacterDelayStrategy
    {
        public TimeSpan NextDelay() => TimeSpan.Zero;
    }
}
