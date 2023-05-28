using Chaos.Definitions;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;

namespace Chaos.Extensions;

public static class TargetFilterExtensions
{
    public static bool IsValidTarget(this TargetFilter filter, Creature? source, Creature target)
    {
        if (filter == TargetFilter.None)
            return true;

        if (source != null)
        {
            if (filter.HasFlag(TargetFilter.SelfOnly) && !source.Equals(target))
                return false;

            if (filter.HasFlag(TargetFilter.GroupOnly))
            {
                if (source is not Aisling aisling)
                    return false;

                if (!source.Equals(target))
                {
                    if (aisling.Group == null)
                        return false;

                    if (!aisling.Group.Contains(target, WorldEntity.IdComparer))
                        return false;
                }
            }

            if (filter.HasFlag(TargetFilter.FriendlyOnly) && !target.IsFriendlyTo(source))
                return false;

            if (filter.HasFlag(TargetFilter.HostileOnly) && !target.IsHostileTo(source))
                return false;
        }

        if (filter.HasFlag(TargetFilter.AliveOnly) && !target.IsAlive)
            return false;

        if (filter.HasFlag(TargetFilter.DeadOnly) && !target.IsDead)
            return false;

        if (filter.HasFlag(TargetFilter.AislingsOnly) && target is not Aisling)
            return false;

        if (filter.HasFlag(TargetFilter.MonstersOnly) && target is not Monster)
            return false;

        return true;
    }
}