namespace Chaos;

[SuppressMessage("ReSharper", "MemberHidesStaticFromOuterClass")]
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