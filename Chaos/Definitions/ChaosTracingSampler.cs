#region
using System.Diagnostics;
using Chaos.Extensions.Common;
using OpenTelemetry.Trace;
#endregion

namespace Chaos.Definitions;

/// <summary>
///     A custom sampler that applies different sampling ratios based on the activity source. Handles parent context
///     directly so that children of unsampled roots are still created (as RecordOnly) to allow the TailSamplingProcessor
///     to evaluate their duration.
/// </summary>
public sealed class ChaosTracingSampler : Sampler
{
    private readonly Sampler DefaultSampler;
    private readonly Sampler PacketSampler;
    private readonly Sampler UpdateSampler;
    private readonly Sampler WorldScriptSampler;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ChaosTracingSampler" /> class.
    /// </summary>
    /// <param name="defaultRatio">
    ///     The sampling ratio for general traces (0.0 to 1.0).
    /// </param>
    /// <param name="updateRatio">
    ///     The sampling ratio for update loop traces (0.0 to 1.0).
    /// </param>
    /// <param name="packetRatio">
    ///     The sampling ratio for packet processing traces (0.0 to 1.0).
    /// </param>
    /// <param name="worldScriptRatio">
    ///     The sampling ratio for world script execution traces (0.0 to 1.0).
    /// </param>
    public ChaosTracingSampler(
        double defaultRatio,
        double updateRatio,
        double packetRatio,
        double worldScriptRatio)
    {
        DefaultSampler = CreateSampler(defaultRatio);
        UpdateSampler = CreateSampler(updateRatio);
        PacketSampler = CreateSampler(packetRatio);
        WorldScriptSampler = CreateSampler(worldScriptRatio);
    }

    private static Sampler CreateSampler(double ratio)
        => ratio switch
        {
            <= 0.0 => new AlwaysOffSampler(),
            >= 1.0 => new AlwaysOnSampler(),
            _      => new TraceIdRatioBasedSampler(ratio)
        };

    /// <summary>
    ///     Ensures the activity is always created by converting Drop to RecordOnly. This allows the TailSamplingProcessor to
    ///     evaluate duration and force-export slow traces.
    /// </summary>
    private static SamplingResult EnsureActivityCreated(SamplingResult result)
        => result.Decision == SamplingDecision.Drop ? new SamplingResult(SamplingDecision.RecordOnly) : result;

    private SamplingResult GetRoutedSamplingResult(in SamplingParameters samplingParameters)
    {
        var activityName = samplingParameters.Name;

        if (activityName.StartsWithI("Update") || activityName.StartsWithI("Map."))
            return UpdateSampler.ShouldSample(samplingParameters);

        if (activityName.StartsWithI("Packet") || activityName.StartsWithI("Recv") || activityName.StartsWithI("Send"))
            return PacketSampler.ShouldSample(samplingParameters);

        if (activityName.StartsWithI("WorldScript"))
            return WorldScriptSampler.ShouldSample(samplingParameters);

        return DefaultSampler.ShouldSample(samplingParameters);
    }

    /// <inheritdoc />
    public override SamplingResult ShouldSample(in SamplingParameters samplingParameters)
    {
        var parentContext = samplingParameters.ParentContext;

        // Child span: inherit parent's sampling decision
        if (parentContext.TraceId != default)
        {
            var parentIsRecorded = (parentContext.TraceFlags & ActivityTraceFlags.Recorded) != 0;

            // Parent was ratio-sampled → child should also be sampled
            if (parentIsRecorded)
                return new SamplingResult(SamplingDecision.RecordAndSample);

            // Parent is RecordOnly (unsampled root) → create child for slow detection
            return new SamplingResult(SamplingDecision.RecordOnly);
        }

        // Root span: apply ratio-based routing, then ensure activity is created
        return EnsureActivityCreated(GetRoutedSamplingResult(samplingParameters));
    }
}