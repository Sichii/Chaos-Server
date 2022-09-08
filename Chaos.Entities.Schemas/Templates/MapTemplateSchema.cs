using System.Collections;
using Chaos.Geometry;

namespace Chaos.Entities.Schemas.Templates;

public record MapTemplateSchema
{
    public required byte Height { get; init; }
    public required string TemplateKey { get; init; }
    public required ICollection<string> ScriptKeys { get; init; } = Array.Empty<string>();
    public required Point[] WarpPoints { get; init; } = Array.Empty<Point>();
    public required byte Width { get; init; }
}