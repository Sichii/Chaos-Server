using System.Text.Json.Serialization;

namespace Chaos.Schemas.Data;

public sealed record ItemDetailsSchema
{
    [JsonRequired]
    public string ItemTemplateKey { get; set; } = null!;
    [JsonRequired]
    public int Stock { get; set; }
}