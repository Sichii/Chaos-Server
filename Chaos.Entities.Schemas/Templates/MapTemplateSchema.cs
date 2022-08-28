using Chaos.Geometry;

namespace Chaos.Entities.Schemas.Templates;

public record MapTemplateSchema
{
    public byte Height { get; init; }
    public string TemplateKey { get; init; } = null!;
    public Point[] WarpPoints { get; init; } = Array.Empty<Point>();
    public byte Width { get; init; }
}