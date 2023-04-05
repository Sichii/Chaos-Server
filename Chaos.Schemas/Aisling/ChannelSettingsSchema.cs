using System.Text.Json.Serialization;
using Chaos.Common.Definitions;

namespace Chaos.Schemas.Aisling;

/// <summary>
///     Represents the serializable schema of an Aisling's channel settings
/// </summary>
public sealed record ChannelSettingsSchema
{
    /// <summary>
    ///     The name of the channel
    /// </summary>
    [JsonRequired]
    public string ChannelName { get; set; } = null!;
    /// <summary>
    ///     The color that message to and from this channel will show up as, if specified
    /// </summary>
    public MessageColor? MessageColor { get; set; }
}