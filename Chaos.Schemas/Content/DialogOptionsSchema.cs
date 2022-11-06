using System.Text.Json.Serialization;

namespace Chaos.Schemas.Content;

public sealed record DialogOptionSchema
{
    [JsonRequired]
    public string DialogKey { get; init; } = null!;
    [JsonRequired]
    public string OptionText { get; init; } = null!;
}