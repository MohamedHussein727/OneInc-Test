using Microsoft.Extensions.Logging.Abstractions;
using OneInc.Processing.Api.Domain;
using OneInc.Processing.Api.Services.Jobs;

namespace OneInc.Processing.Tests;

/// <summary>
/// Unit tests for <see cref="ProcessingJobCoordinator"/>.
/// </summary>
public sealed class ProcessingJobCoordinatorTests
{
    [Fact]
    public void TryStartStreaming_AllowsOnlySingleConsumer()
    {
        var coordinator = new ProcessingJobCoordinator(NullLogger<ProcessingJobCoordinator>.Instance);
        var created = coordinator.CreateJob(new ProcessingResult("payload", 7));

        var first = coordinator.TryStartStreaming(created.Id, out _);
        var second = coordinator.TryStartStreaming(created.Id, out _);

        Assert.Equal(JobStreamStartResult.Started, first);
        Assert.Equal(JobStreamStartResult.AlreadyStarted, second);
    }

    [Fact]
    public void TryCancel_CancelsActiveJob()
    {
        var coordinator = new ProcessingJobCoordinator(NullLogger<ProcessingJobCoordinator>.Instance);
        var created = coordinator.CreateJob(new ProcessingResult("payload", 7));

        var cancelled = coordinator.TryCancel(created.Id);

        Assert.True(cancelled);
        Assert.True(created.CancellationSource.IsCancellationRequested);

        coordinator.Complete(created.Id);
    }
}
