using System.Collections.Immutable;
using Chaos.Models.World.Abstractions;

namespace Chaos.Scripting.Behaviors;

public class BlindBehavior
{
    private readonly ImmutableList<string> BlindEffects = ImmutableList.Create("blind");

    public virtual bool IsBlind(Creature creature) => BlindEffects.Any(key => creature.Effects.Contains(key));
}