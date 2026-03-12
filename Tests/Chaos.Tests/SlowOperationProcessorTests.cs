#region
using System.Diagnostics;
using Chaos.Definitions;
using Chaos.Networking.Abstractions.Definitions;
using FluentAssertions;
#endregion

namespace Chaos.Tests;

public sealed class SlowOperationProcessorTests : IDisposable
{
    private readonly ActivityListener Listener;
    private readonly ActivitySource PacketSource = new(NetworkingActivitySources.PACKET_SOURCE_NAME);
    private readonly ActivitySource UnknownSource = new("unknown.source.slow");
    private readonly ActivitySource UpdateSource = new(ChaosActivitySources.UPDATE_SOURCE_NAME);
    private readonly ActivitySource WorldScriptSource = new(ChaosActivitySources.WORLD_SCRIPT_SOURCE_NAME);

    public SlowOperationProcessorTests()
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
    ///     Creates a completed activity with a specific duration by manipulating start time
    /// </summary>
    private static Activity CreateActivity(ActivitySource source, string name, TimeSpan duration)
    {
        var activity = source.StartActivity(name)!;

        // SetEndTime will compute Duration = EndTimeUtc - StartTimeUtc
        activity.SetEndTime(activity.StartTimeUtc + duration);

        return activity;
    }

    /// <summary>
    ///     Creates a processor that considers anything over 1ms as slow
    /// </summary>
    private static SlowOperationProcessor CreateProcessor(double updateMs = 1, double packetMs = 1, double worldScriptMs = 1)
        => new(updateMs, packetMs, worldScriptMs);

    #region Not Slow, Not Recorded (no-op)
    [Test]
    public void OnEnd_ShouldNotAddAnyTags_WhenNotSlowAndNotRecorded()
    {
        var processor = CreateProcessor(1000);
        var activity = CreateActivity(UnknownSource, "notSlowNotRecorded", TimeSpan.FromMilliseconds(1));
        activity.ActivityTraceFlags &= ~ActivityTraceFlags.Recorded;

        processor.OnEnd(activity);

        activity.GetTagItem("slow_operation")
                .Should()
                .BeNull();

        activity.GetTagItem("duration_ms")
                .Should()
                .BeNull();
    }
    #endregion

    #region ShouldForceExport (via OnEnd behavior)
    [Test]
    public void OnEnd_ShouldMarkSlow_WhenUpdateSourceExceedsThreshold()
    {
        var processor = CreateProcessor(10);
        var activity = CreateActivity(UpdateSource, "slowUpdate", TimeSpan.FromMilliseconds(50));

        processor.OnEnd(activity);

        activity.GetTagItem("slow_operation")
                .Should()
                .Be(true);

        activity.GetTagItem("duration_ms")
                .Should()
                .NotBeNull();

        activity.Status
                .Should()
                .Be(ActivityStatusCode.Error);
    }

    [Test]
    public void OnEnd_ShouldNotMarkSlow_WhenUpdateSourceBelowThreshold()
    {
        var processor = CreateProcessor(1000);
        var activity = CreateActivity(UpdateSource, "fastUpdate", TimeSpan.FromMilliseconds(1));

        processor.OnEnd(activity);

        activity.GetTagItem("slow_operation")
                .Should()
                .BeNull();
    }

    [Test]
    public void OnEnd_ShouldMarkSlow_WhenPacketSourceExceedsThreshold()
    {
        var processor = CreateProcessor(packetMs: 10);
        var activity = CreateActivity(PacketSource, "slowPacket", TimeSpan.FromMilliseconds(50));

        processor.OnEnd(activity);

        activity.GetTagItem("slow_operation")
                .Should()
                .Be(true);
    }

    [Test]
    public void OnEnd_ShouldMarkSlow_WhenWorldScriptSourceExceedsThreshold()
    {
        var processor = CreateProcessor(worldScriptMs: 10);
        var activity = CreateActivity(WorldScriptSource, "slowScript", TimeSpan.FromMilliseconds(50));

        processor.OnEnd(activity);

        activity.GetTagItem("slow_operation")
                .Should()
                .Be(true);
    }

    [Test]
    public void OnEnd_ShouldNotMarkSlow_WhenUnknownSource()
    {
        var processor = CreateProcessor();
        var activity = CreateActivity(UnknownSource, "unknownOp", TimeSpan.FromMilliseconds(50));

        processor.OnEnd(activity);

        activity.GetTagItem("slow_operation")
                .Should()
                .BeNull();
    }
    #endregion

    #region Force Recording
    [Test]
    public void OnEnd_ShouldForceRecorded_WhenSlowAndNotRecorded()
    {
        var processor = CreateProcessor(10);
        var activity = CreateActivity(UpdateSource, "slowUnrecorded", TimeSpan.FromMilliseconds(50));

        // Ensure not recorded initially
        activity.ActivityTraceFlags &= ~ActivityTraceFlags.Recorded;

        activity.Recorded
                .Should()
                .BeFalse();

        processor.OnEnd(activity);

        activity.Recorded
                .Should()
                .BeTrue();
    }

    [Test]
    public void OnEnd_ShouldNotChangeRecordedFlag_WhenNotSlow()
    {
        var processor = CreateProcessor(1000);
        var activity = CreateActivity(UpdateSource, "fast", TimeSpan.FromMilliseconds(1));
        activity.ActivityTraceFlags &= ~ActivityTraceFlags.Recorded;

        processor.OnEnd(activity);

        activity.Recorded
                .Should()
                .BeFalse();
    }
    #endregion

    #region Duration Tag for Recorded Activities
    [Test]
    public void OnEnd_ShouldAddDurationTag_WhenRecordedAndNotSlow()
    {
        // Use AllDataAndRecorded for this test
        using var recordedListener = new ActivityListener
        {
            ShouldListenTo = _ => true,
            Sample = (ref _) => ActivitySamplingResult.AllDataAndRecorded
        };

        using var source = new ActivitySource("recorded.test.slow");
        ActivitySource.AddActivityListener(recordedListener);

        var processor = CreateProcessor(99999);
        var activity = source.StartActivity("recorded")!;
        activity.SetEndTime(activity.StartTimeUtc + TimeSpan.FromMilliseconds(5));

        processor.OnEnd(activity);

        activity.GetTagItem("duration_ms")
                .Should()
                .NotBeNull();
    }

    [Test]
    public void OnEnd_ShouldNotDuplicateDurationTag_WhenAlreadySet()
    {
        using var recordedListener = new ActivityListener
        {
            ShouldListenTo = _ => true,
            Sample = (ref _) => ActivitySamplingResult.AllDataAndRecorded
        };

        using var source = new ActivitySource("recorded.nodup.slow");
        ActivitySource.AddActivityListener(recordedListener);

        var processor = CreateProcessor(99999);
        var activity = source.StartActivity("pretagged")!;
        activity.SetEndTime(activity.StartTimeUtc + TimeSpan.FromMilliseconds(5));
        activity.SetTag("duration_ms", 999.0);

        processor.OnEnd(activity);

        // Should still have original value
        activity.GetTagItem("duration_ms")
                .Should()
                .Be(999.0);
    }
    #endregion
}