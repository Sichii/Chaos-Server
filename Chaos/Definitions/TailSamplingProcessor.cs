#region
using System.Diagnostics;
using Chaos.Networking.Abstractions.Definitions;
using OpenTelemetry;
#endregion

namespace Chaos.Definitions;

/// <summary>
///     A tail-sampling processor that buffers spans per trace and makes trace-level export decisions. When a root span
///     ends, if any span in the trace was ratio-sampled or exceeded its duration threshold, the entire trace (all buffered
///     spans) is exported. Otherwise, the trace is discarded silently. This processor should be added before the exporter
///     in the pipeline.
/// </summary>
public sealed class TailSamplingProcessor : BaseProcessor<Activity>
{
    private readonly BaseProcessor<Activity> InnerProcessor;
    private readonly TimeSpan OrphanTimeout;
    private readonly TimeSpan PacketThreshold;

    // ReSharper disable once InconsistentlySynchronizedField (ConcurrentDictionary is thread-safe; locks protect TraceBuffer, not the dictionary)
    private readonly ConcurrentDictionary<ActivityTraceId, TraceBuffer> Traces = new();
    private readonly TimeSpan UpdateThreshold;
    private readonly TimeSpan WorldScriptThreshold;
    private Timer? CleanupTimer;

    /// <summary>
    ///     Initializes a new instance of the <see cref="TailSamplingProcessor" /> class.
    /// </summary>
    /// <param name="innerProcessor">
    ///     The inner processor (typically a BatchActivityExportProcessor) to which exported spans are forwarded.
    /// </param>
    /// <param name="updateThresholdMs">
    ///     Threshold in milliseconds for update operations.
    /// </param>
    /// <param name="packetThresholdMs">
    ///     Threshold in milliseconds for packet operations.
    /// </param>
    /// <param name="worldScriptThresholdMs">
    ///     Threshold in milliseconds for world script operations.
    /// </param>
    /// <param name="orphanTimeoutMinutes">
    ///     Timeout in minutes before orphaned trace buffers are evicted. Defaults to 5.
    /// </param>
    public TailSamplingProcessor(
        BaseProcessor<Activity> innerProcessor,
        double updateThresholdMs,
        double packetThresholdMs,
        double worldScriptThresholdMs,
        double orphanTimeoutMinutes = 5.0)
    {
        InnerProcessor = innerProcessor;
        UpdateThreshold = TimeSpan.FromMilliseconds(updateThresholdMs);
        PacketThreshold = TimeSpan.FromMilliseconds(packetThresholdMs);
        WorldScriptThreshold = TimeSpan.FromMilliseconds(worldScriptThresholdMs);
        OrphanTimeout = TimeSpan.FromMinutes(orphanTimeoutMinutes);

        CleanupTimer = new Timer(
            EvictOrphanedTraces,
            null,
            OrphanTimeout,
            OrphanTimeout);
    }

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            CleanupTimer?.Dispose();
            CleanupTimer = null;

            InnerProcessor.Dispose();
        }

        base.Dispose(disposing);
    }

    private void EvictOrphanedTraces(object? state)
    {
        var cutoff = DateTime.UtcNow - OrphanTimeout;

        // ReSharper disable once InconsistentlySynchronizedField
        foreach (var kvp in Traces)
        {
            TraceBuffer? buffer;

            lock (kvp.Value)
            {
                if (kvp.Value.LastActivity > cutoff)
                    continue;

                Traces.TryRemove(kvp.Key, out buffer);
            }

            // Buffer was already removed by another thread or root span completion
            if (buffer is null)
                continue;

            lock (buffer)
                if (buffer.IsSampled || buffer.HasSlowSpan)
                    FlushBuffer(buffer);
        }
    }

    private void FlushBuffer(TraceBuffer buffer)
    {
        foreach (var span in buffer.Spans)
        {
            span.ActivityTraceFlags |= ActivityTraceFlags.Recorded;

            if (IsSlowSpan(span))
            {
                span.SetTag("slow_operation", true);
                span.SetStatus(ActivityStatusCode.Error, "Operation exceeded duration threshold");
            }

            span.SetTag("duration_ms", span.Duration.TotalMilliseconds);

            InnerProcessor.OnEnd(span);
        }
    }

    private bool IsSlowSpan(Activity data)
    {
        var sourceName = data.Source.Name;
        var duration = data.Duration;

        return sourceName switch
        {
            ChaosActivitySources.UPDATE_SOURCE_NAME       => duration > UpdateThreshold,
            NetworkingActivitySources.PACKET_SOURCE_NAME  => duration > PacketThreshold,
            ChaosActivitySources.WORLD_SCRIPT_SOURCE_NAME => duration > WorldScriptThreshold,
            _                                             => false
        };
    }

    /// <inheritdoc />
    public override void OnEnd(Activity data)
    {
        var traceId = data.TraceId;

        // ReSharper disable once InconsistentlySynchronizedField
        var buffer = Traces.GetOrAdd(traceId, _ => new TraceBuffer());

        lock (buffer)
        {
            buffer.Spans.Add(data);
            buffer.LastActivity = DateTime.UtcNow;

            if (data.Recorded)
                buffer.IsSampled = true;

            if (IsSlowSpan(data))
                buffer.HasSlowSpan = true;

            // Root span: make the export decision for the entire trace.
            // Use ParentSpanId rather than Parent — children created with an explicit ActivityContext
            // (e.g. StartActivity(name, kind, parentContext)) have Parent == null but ParentSpanId
            // set to the parent's span ID. Only true roots have a default (all-zero) ParentSpanId.
            if (data.ParentSpanId == default)
            {
                Traces.TryRemove(traceId, out _);

                if (buffer.IsSampled || buffer.HasSlowSpan)
                    FlushBuffer(buffer);
            }
        }
    }

    /// <inheritdoc />
    protected override bool OnForceFlush(int timeoutMilliseconds) => InnerProcessor.ForceFlush(timeoutMilliseconds);

    /// <inheritdoc />
    protected override bool OnShutdown(int timeoutMilliseconds)
    {
        CleanupTimer?.Dispose();
        CleanupTimer = null;

        // Flush any remaining buffers that qualify
        // ReSharper disable once InconsistentlySynchronizedField
        foreach (var kvp in Traces)

            // ReSharper disable once InconsistentlySynchronizedField
            if (Traces.TryRemove(kvp.Key, out var buffer))
                lock (buffer)
                    if (buffer.IsSampled || buffer.HasSlowSpan)
                        FlushBuffer(buffer);

        return InnerProcessor.Shutdown(timeoutMilliseconds);
    }

    private sealed class TraceBuffer
    {
        public bool HasSlowSpan;
        public bool IsSampled;
        public DateTime LastActivity = DateTime.UtcNow;
        public List<Activity> Spans { get; } = [];
    }
}