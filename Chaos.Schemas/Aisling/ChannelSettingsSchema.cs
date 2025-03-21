#region
using System.Text.Json.Serialization;
using Chaos.DarkAges.Definitions;
#endregion

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
    ///     Whether or not this channel is a custom channel
    /// </summary>
    public bool CustomChannel { get; set; }

    /// <summary>
    ///     The color that message to and from this channel will show up as, if specified
    /// </summary>
    public MessageColor? MessageColor { get; set; }
}