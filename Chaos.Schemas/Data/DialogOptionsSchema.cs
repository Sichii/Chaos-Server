using System.Text.Json.Serialization;

namespace Chaos.Schemas.Data;

public sealed record DialogOptionSchema
{
    [JsonRequired]
    public string DialogKey { get; set; } = null!;
    [JsonRequired]
    public string OptionText { get; set; } = null!;
}