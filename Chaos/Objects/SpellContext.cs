using Chaos.Containers;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;

namespace Chaos.Objects;

public sealed record SpellContext(Creature Target, Creature Source, string? Prompt = null)
{
    public MapInstance Map { get; } = Target.MapInstance;
    public Aisling? SourceAisling { get; } = Source as Aisling;
    public Point SourcePoint { get; } = Point.From(Source);
    public Aisling? TargetAisling { get; } = Target as Aisling;
    public Point TargetPoint { get; } = Point.From(Target);
}