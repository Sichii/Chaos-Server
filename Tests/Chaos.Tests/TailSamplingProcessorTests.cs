#region
using System.Diagnostics;
using Chaos.Definitions;
using Chaos.Networking.Abstractions.Definitions;
using FluentAssertions;
using Moq;
using OpenTelemetry;
#endregion

namespace Chaos.Tests;

public sealed class TailSamplingProcessorTests : IDisposable
{
    private readonly ActivityListener Listener;
    private readonly ActivitySource PacketSource = new(NetworkingActivitySources.PACKET_SOURCE_NAME);
    private readonly ActivitySource UnknownSource = new("unknown.source.tail");
    private readonly ActivitySource UpdateSource = new(ChaosActivitySources.UPDATE_SOURCE_NAME);
    private readonly ActivitySource WorldScriptSource = new(ChaosActivitySources.WORLD_SCRIPT_SOURCE_NAME);

    public TailSamplingProcessorTests()
    {
        Listener = new ActivityListener
        {
            ShouldListenTo = _ => true,
            Sample = (ref _) => ActivitySamplingResult.AllData
        };

        ActivitySource.AddActivityListener(Listener);
    }

    public void Dispose()
    {
        Listener.Dispose();
        UpdateSource.Dispose();
        PacketSource.Dispose();
        WorldScriptSource.Dispose();
        UnknownSource.Dispose();
    }

    /// <summary>
    ///     Creates a child activity under a parent context
    /// </summary>
    private Activity CreateChildActivity(
        ActivitySource source,
        string name,
        TimeSpan duration,
        ActivityContext parentContext)
    {
        var activity = source.StartActivity(name, ActivityKind.Internal, parentContext)!;
        activity.SetEndTime(activity.StartTimeUtc + duration);

        return activity;
    }

    private static Mock<BaseProcessor<Activity>> CreateInnerProcessor() => new();

    private static TailSamplingProcessor CreateProcessor(
        Mock<BaseProcessor<Activity>>? innerMock = null,
        double updateMs = 10,
        double packetMs = 10,
        double worldScriptMs = 10,
        double orphanTimeoutMinutes = 60)
    {
        innerMock ??= CreateInnerProcessor();

        return new TailSamplingProcessor(
            innerMock.Object,
            updateMs,
            packetMs,
            worldScriptMs,
            orphanTimeoutMinutes);
    }

    /// <summary>
    ///     Creates a root activity (no parent) with a specific duration
    /// </summary>
    private Activity CreateRootActivity(ActivitySource source, string name, TimeSpan duration)
    {
        var activity = source.StartActivity(name)!;
        activity.SetEndTime(activity.StartTimeUtc + duration);

        return activity;
    }

    #region Dispose
    [Test]
    public void Dispose_ShouldNotThrow()
    {
        var innerMock = CreateInnerProcessor();
        var processor = CreateProcessor(innerMock);

        var act = () => processor.Dispose();

        act.Should()
           .NotThrow();
    }
    #endregion

    #region FlushBuffer - Error status on slow spans
    [Test]
    public void FlushBuffer_ShouldSetErrorStatus_OnSlowSpans()
    {
        var innerMock = CreateInnerProcessor();
        using var processor = CreateProcessor(innerMock);

        var root = CreateRootActivity(UpdateSource, "slowRoot", TimeSpan.FromMilliseconds(50));

        processor.OnEnd(root);

        root.Status
            .Should()
            .Be(ActivityStatusCode.Error);

        root.StatusDescription
            .Should()
            .Contain("exceeded");
    }
    #endregion

    #region OnEnd - Sampled child propagation
    [Test]
    public void OnEnd_ShouldFlush_WhenChildIsSampledEvenIfRootIsFast()
    {
        var innerMock = CreateInnerProcessor();
        using var processor = CreateProcessor(innerMock, 1000);

        var root = CreateRootActivity(UpdateSource, "fastRoot", TimeSpan.FromMilliseconds(1));

        // Mark child as recorded (sampled)
        var child = CreateChildActivity(
            UpdateSource,
            "sampledChild",
            TimeSpan.FromMilliseconds(1),
            root.Context);
        child.ActivityTraceFlags |= ActivityTraceFlags.Recorded;

        processor.OnEnd(child);
        processor.OnEnd(root);

        // Should flush because child was sampled
        innerMock.Verify(p => p.OnEnd(child), Times.Once);
        innerMock.Verify(p => p.OnEnd(root), Times.Once);
    }
    #endregion

    #region FlushBuffer - Recorded flag
    [Test]
    public void OnEnd_ShouldSetRecordedFlag_OnAllFlushedSpans()
    {
        var innerMock = CreateInnerProcessor();
        using var processor = CreateProcessor(innerMock);

        var root = CreateRootActivity(UpdateSource, "root", TimeSpan.FromMilliseconds(50));
        root.ActivityTraceFlags &= ~ActivityTraceFlags.Recorded;

        processor.OnEnd(root);

        root.Recorded
            .Should()
            .BeTrue();
    }
    #endregion

    #region OnEnd - Buffering
    [Test]
    public void OnEnd_ShouldBufferNonRootSpan_WithoutFlushing()
    {
        var innerMock = CreateInnerProcessor();
        using var processor = CreateProcessor(innerMock);

        var root = CreateRootActivity(UpdateSource, "root", TimeSpan.FromMilliseconds(1));

        var child = CreateChildActivity(
            UpdateSource,
            "child",
            TimeSpan.FromMilliseconds(1),
            root.Context);

        // End child only (non-root)
        processor.OnEnd(child);

        // Should NOT flush to inner processor yet
        innerMock.Verify(p => p.OnEnd(It.IsAny<Activity>()), Times.Never);
    }

    [Test]
    public void OnEnd_ShouldFlush_WhenRootSpanEndsAndTraceIsSampled()
    {
        var innerMock = CreateInnerProcessor();
        using var processor = CreateProcessor(innerMock);

        // Create a recorded (sampled) root span
        var root = CreateRootActivity(UpdateSource, "sampledRoot", TimeSpan.FromMilliseconds(1));
        root.ActivityTraceFlags |= ActivityTraceFlags.Recorded;

        processor.OnEnd(root);

        // Should flush the root span to inner processor
        innerMock.Verify(p => p.OnEnd(root), Times.Once);
    }

    [Test]
    public void OnEnd_ShouldFlush_WhenRootSpanEndsAndTraceHasSlowSpan()
    {
        var innerMock = CreateInnerProcessor();
        using var processor = CreateProcessor(innerMock);

        // Create a slow root span (exceeds threshold)
        var root = CreateRootActivity(UpdateSource, "slowRoot", TimeSpan.FromMilliseconds(50));

        processor.OnEnd(root);

        // Should flush because root is slow
        innerMock.Verify(p => p.OnEnd(root), Times.Once);
    }

    [Test]
    public void OnEnd_ShouldNotFlush_WhenRootSpanEndsAndNotSampledNorSlow()
    {
        var innerMock = CreateInnerProcessor();
        using var processor = CreateProcessor(innerMock, 1000);

        var root = CreateRootActivity(UpdateSource, "fastRoot", TimeSpan.FromMilliseconds(1));
        root.ActivityTraceFlags &= ~ActivityTraceFlags.Recorded;

        processor.OnEnd(root);

        // Should NOT flush — not sampled and not slow
        innerMock.Verify(p => p.OnEnd(It.IsAny<Activity>()), Times.Never);
    }
    #endregion

    #region OnEnd - Multi-span Traces
    [Test]
    public void OnEnd_ShouldFlushAllSpans_WhenTraceHasSlowChild()
    {
        var innerMock = CreateInnerProcessor();
        using var processor = CreateProcessor(innerMock);

        var root = CreateRootActivity(UpdateSource, "root", TimeSpan.FromMilliseconds(1));

        var slowChild = CreateChildActivity(
            UpdateSource,
            "slowChild",
            TimeSpan.FromMilliseconds(50),
            root.Context);

        // End child first (marks trace as having slow span)
        processor.OnEnd(slowChild);

        // End root (triggers flush)
        processor.OnEnd(root);

        // Both spans should be flushed
        innerMock.Verify(p => p.OnEnd(slowChild), Times.Once);
        innerMock.Verify(p => p.OnEnd(root), Times.Once);
    }

    [Test]
    public void OnEnd_ShouldSetSlowOperationTag_OnSlowSpans()
    {
        var innerMock = CreateInnerProcessor();
        using var processor = CreateProcessor(innerMock);

        var root = CreateRootActivity(UpdateSource, "slowRoot", TimeSpan.FromMilliseconds(50));

        processor.OnEnd(root);

        root.GetTagItem("slow_operation")
            .Should()
            .Be(true);

        root.GetTagItem("duration_ms")
            .Should()
            .NotBeNull();
    }

    [Test]
    public void OnEnd_ShouldNotSetSlowTag_OnFastSpansInSlowTrace()
    {
        var innerMock = CreateInnerProcessor();
        using var processor = CreateProcessor(innerMock);

        // Root is sampled (recorded) but fast
        var root = CreateRootActivity(UpdateSource, "fastRoot", TimeSpan.FromMilliseconds(1));
        root.ActivityTraceFlags |= ActivityTraceFlags.Recorded;

        processor.OnEnd(root);

        root.GetTagItem("slow_operation")
            .Should()
            .BeNull();
    }
    #endregion

    #region IsSlowSpan branches
    [Test]
    public void OnEnd_ShouldDetectSlowPacketSpan()
    {
        var innerMock = CreateInnerProcessor();
        using var processor = CreateProcessor(innerMock, packetMs: 10);

        var root = CreateRootActivity(PacketSource, "slowPacket", TimeSpan.FromMilliseconds(50));

        processor.OnEnd(root);

        // Flushed because slow
        innerMock.Verify(p => p.OnEnd(root), Times.Once);

        root.GetTagItem("slow_operation")
            .Should()
            .Be(true);
    }

    [Test]
    public void OnEnd_ShouldDetectSlowWorldScriptSpan()
    {
        var innerMock = CreateInnerProcessor();
        using var processor = CreateProcessor(innerMock, worldScriptMs: 10);

        var root = CreateRootActivity(WorldScriptSource, "slowScript", TimeSpan.FromMilliseconds(50));

        processor.OnEnd(root);

        innerMock.Verify(p => p.OnEnd(root), Times.Once);

        root.GetTagItem("slow_operation")
            .Should()
            .Be(true);
    }

    [Test]
    public void OnEnd_ShouldNotDetectSlowSpan_WhenUnknownSource()
    {
        var innerMock = CreateInnerProcessor();
        using var processor = CreateProcessor(innerMock);

        var root = CreateRootActivity(UnknownSource, "unknownOp", TimeSpan.FromMilliseconds(50));

        processor.OnEnd(root);

        // Not flushed — unknown source is never "slow"
        innerMock.Verify(p => p.OnEnd(It.IsAny<Activity>()), Times.Never);
    }
    #endregion

    #region OnShutdown
    [Test]
    public void Shutdown_ShouldFlushQualifyingBuffers()
    {
        var innerMock = CreateInnerProcessor();
        using var processor = CreateProcessor(innerMock);

        // Add a slow span first (non-root, so it stays buffered)
        var root = CreateRootActivity(UpdateSource, "buffered", TimeSpan.FromMilliseconds(50));

        var child = CreateChildActivity(
            UpdateSource,
            "slowChild",
            TimeSpan.FromMilliseconds(50),
            root.Context);

        processor.OnEnd(child);

        // Shutdown should flush the slow child to inner processor
        processor.Shutdown();

        // The slow child should have been flushed
        innerMock.Verify(p => p.OnEnd(child), Times.Once);
    }

    [Test]
    public void Shutdown_ShouldNotFlushNonQualifyingBuffers()
    {
        var innerMock = CreateInnerProcessor();
        using var processor = CreateProcessor(innerMock, 1000);

        // Add a fast, unsampled span (non-root)
        var root = CreateRootActivity(UpdateSource, "fastBuffered", TimeSpan.FromMilliseconds(1));
        root.ActivityTraceFlags &= ~ActivityTraceFlags.Recorded;

        var child = CreateChildActivity(
            UpdateSource,
            "fastChild",
            TimeSpan.FromMilliseconds(1),
            root.Context);
        child.ActivityTraceFlags &= ~ActivityTraceFlags.Recorded;

        processor.OnEnd(child);

        processor.Shutdown();

        // The fast child should NOT have been flushed
        innerMock.Verify(p => p.OnEnd(child), Times.Never);
    }
    #endregion
}