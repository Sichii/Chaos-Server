namespace Chaos;

[SuppressMessage("ReSharper", "MemberHidesStaticFromOuterClass")]
[ExcludeFromCodeCoverage]
public static class ConfigKeys
{
    public static class Logging
    {
        public static string Key => nameof(Logging);
        public static string UseSeq => $"{Key}:{nameof(UseSeq)}";

        public static class NLog
        {
            public static string Key => $"{Logging.Key}:{nameof(NLog)}";
        }
    }

    public static class NLog
    {
        public static string Key => nameof(NLog);
    }

    public static class OpenTelemetry
    {
        public static string Key => nameof(OpenTelemetry);
        public static string OtlpEndpoint => $"{Key}:{nameof(OtlpEndpoint)}";
        public static string PacketSamplingRatio => $"{Key}:{nameof(PacketSamplingRatio)}";
        public static string SamplingRatio => $"{Key}:{nameof(SamplingRatio)}";
        public static string ServiceName => $"{Key}:{nameof(ServiceName)}";
        public static string SlowPacketThresholdMs => $"{Key}:{nameof(SlowPacketThresholdMs)}";
        public static string SlowUpdateThresholdMs => $"{Key}:{nameof(SlowUpdateThresholdMs)}";
        public static string SlowWorldScriptThresholdMs => $"{Key}:{nameof(SlowWorldScriptThresholdMs)}";
        public static string UpdateSamplingRatio => $"{Key}:{nameof(UpdateSamplingRatio)}";
        public static string WorldScriptSamplingRatio => $"{Key}:{nameof(WorldScriptSamplingRatio)}";
    }

    public static class Options
    {
        public static string Key => nameof(Options);

        public static class SiteOptions
        {
            public static string EnableSite => $"{Key}:{nameof(EnableSite)}";
            public static string Key => $"{Options.Key}:{nameof(SiteOptions)}";
        }
    }
}