using Chaos.Definitions;
using Chaos.Objects.World.Abstractions;

namespace Chaos.Extensions;

public static class TargetFilterExtensions
{
    public static bool IsValidTarget(this TargetFilter filter, Creature c1, Creature c2) => filter switch
    {
        TargetFilter.None         => true,
        TargetFilter.FriendlyOnly => c1.IsFriendlyTo(c2),
        TargetFilter.HostileOnly  => !c1.IsFriendlyTo(c2),
        _                         => throw new ArgumentOutOfRangeException(nameof(filter), filter, null)
    };
}