using Chaos.Containers;
using Chaos.Geometry.Abstractions;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;

namespace Chaos.Objects;

public sealed record SpellContext(Creature Target, Creature Source, string? Prompt = null)
{
    public MapInstance Map { get; } = Target.MapInstance;
    public IPoint SourcePoint { get; } = Point.From(Source);
    public IPoint TargetPoint { get; } = Point.From(Target);
    public Aisling? TargetAisling { get; } = Target as Aisling;
    public Aisling? SourceAisling { get; } = Source as Aisling;
}