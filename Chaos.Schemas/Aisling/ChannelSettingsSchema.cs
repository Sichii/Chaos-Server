using System.Text.Json.Serialization;
using Chaos.Common.Definitions;

namespace Chaos.Schemas.Aisling;

public sealed record ChannelSettingsSchema
{
    [JsonRequired]
    public string ChannelName { get; set; } = null!;
    public MessageColor? MessageColor { get; set; }
}