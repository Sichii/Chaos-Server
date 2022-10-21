using System.Text.Json.Serialization;

namespace Chaos.Schemas.Aisling;

public sealed record EffectSchema
{
    [JsonRequired]
    public string Name { get; init; } = null!;
    public int? RemainingSecs { get; init; }
}