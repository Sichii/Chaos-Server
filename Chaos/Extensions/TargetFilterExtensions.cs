using Chaos.Definitions;
using Chaos.Objects.World.Abstractions;

namespace Chaos.Extensions;

public static class TargetFilterExtensions
{
    public static bool IsValidTarget(this TargetFilter filter, Creature source, Creature target)
    {
        if (filter == TargetFilter.None)
            return true;

        var isValid = true;

        if (filter.HasFlag(TargetFilter.FriendlyOnly))
            isValid &= target.IsFriendlyTo(source);

        if (filter.HasFlag(TargetFilter.HostileOnly))
            isValid &= target.IsHostileTo(source);

        if (filter.HasFlag(TargetFilter.AliveOnly))
            isValid &= target.IsAlive;

        if (filter.HasFlag(TargetFilter.DeadOnly))
            isValid &= target.IsDead;

        return isValid;
    }
}