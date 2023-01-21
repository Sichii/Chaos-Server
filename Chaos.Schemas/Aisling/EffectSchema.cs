using System.Text.Json.Serialization;

namespace Chaos.Schemas.Aisling;

public sealed record EffectSchema
{
    [JsonRequired]
    public string EffectKey { get; set; } = null!;
    public int RemainingSecs { get; set; }
}