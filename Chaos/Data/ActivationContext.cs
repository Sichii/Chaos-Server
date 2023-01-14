using Chaos.Containers;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;

namespace Chaos.Data;

public class ActivationContext
{
    public MapInstance Map { get; }
    public Creature Source { get; }
    public Aisling? SourceAisling { get; }
    public Point SourcePoint { get; }
    public Creature Target { get; }
    public Aisling? TargetAisling { get; }
    public Point TargetPoint { get; }

    public ActivationContext(Creature source, Creature target)
    {
        Source = source;
        Target = target;
        SourcePoint = Point.From(source);
        TargetPoint = Point.From(target);
        SourceAisling = Source as Aisling;
        TargetAisling = Target as Aisling;
        Map = Target.MapInstance;
    }
}