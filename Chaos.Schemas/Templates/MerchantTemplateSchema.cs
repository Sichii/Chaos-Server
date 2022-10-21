using System.Text.Json.Serialization;
using Chaos.Common.Definitions;
using Chaos.Geometry.Abstractions.Definitions;

namespace Chaos.Schemas.Templates;

public sealed record MerchantTemplateSchema
{
    [JsonRequired]
    public string TemplateKey { get; init; } = null!;
    [JsonRequired]
    public string Name { get; init; } = null!;
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public ushort Sprite { get; init; }
    public ICollection<string> ScriptKeys { get; init; } = Array.Empty<string>();
    public string? DialogKey { get; init; }
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public Direction Direction { get; init; }
}