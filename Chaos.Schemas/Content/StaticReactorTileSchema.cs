using System.Text.Json.Serialization;
using Chaos.Geometry;

namespace Chaos.Schemas.Content;

public sealed record StaticReactorTileSchema : ReactorTileSchema
{
    public string? OwnerMonsterTemplateKey { get; init; }
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public Point Source { get; init; }
}