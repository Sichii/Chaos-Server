namespace Chaos.Messaging;

/// <summary>
///     Contains configuration options for the <see cref="ChannelService" />
/// </summary>
public sealed class ChannelServiceOptions
{
    /// <summary>
    ///     A collection of phrases that can not be used as part of a channel name
    /// </summary>
    public required string[] BlacklistedChannelNamePhrases { get; init; } = Array.Empty<string>();

    /// <summary>
    ///     The prefix used to identify channels
    /// </summary>
    public required string ChannelPrefix { get; init; }

    /// <summary>
    ///     The maximum length of a channel name
    /// </summary>
    public required int MaxChannelNameLength { get; init; }

    /// <summary>
    ///     The minimum length of a channel name
    /// </summary>
    public required int MinChannelNameLength { get; init; }

    /// <summary>
    ///     A collection of reserved channel names. Generally these should be the default channels.
    /// </summary>
    public required string[] ReservedChannelNames { get; init; } = Array.Empty<string>();
}