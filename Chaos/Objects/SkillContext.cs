using Chaos.Containers;
using Chaos.Geometry.Abstractions;
using Chaos.Objects.World.Abstractions;

namespace Chaos.Objects;

public sealed record SkillContext(Creature Source)
{
    public MapInstance Map { get; } = Source.MapInstance;
    public IPoint SourcePoint { get; } = Point.From(Source);
}