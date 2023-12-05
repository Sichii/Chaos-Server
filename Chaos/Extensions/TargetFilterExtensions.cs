using Chaos.Definitions;
using Chaos.Extensions.Common;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;

namespace Chaos.Extensions;

public static class TargetFilterExtensions
{
    private static bool InnerIsValidTarget(this TargetFilter filter, Creature source, Creature target)
        => filter switch
        {
            TargetFilter.None             => true,
            TargetFilter.FriendlyOnly     => source.IsFriendlyTo(target),
            TargetFilter.HostileOnly      => source.IsHostileTo(target),
            TargetFilter.NeutralOnly      => !source.IsFriendlyTo(target) && !source.IsHostileTo(target),
            TargetFilter.NonFriendlyOnly  => !source.IsFriendlyTo(target),
            TargetFilter.NonHostileOnly   => !source.IsHostileTo(target),
            TargetFilter.NonNeutralOnly   => source.IsFriendlyTo(target) || source.IsHostileTo(target),
            TargetFilter.AliveOnly        => target.IsAlive,
            TargetFilter.DeadOnly         => target.IsDead,
            TargetFilter.AislingsOnly     => target is Aisling,
            TargetFilter.MonstersOnly     => target is Monster,
            TargetFilter.MerchantsOnly    => target is Merchant,
            TargetFilter.NonAislingsOnly  => target is not Aisling,
            TargetFilter.NonMonstersOnly  => target is not Monster,
            TargetFilter.NonMerchantsOnly => target is not Merchant,
            TargetFilter.SelfOnly         => source.Equals(target),
            TargetFilter.OthersOnly       => !source.Equals(target),
            TargetFilter.GroupOnly => source.Equals(target)
                                      || (source is Aisling { Group: not null } aisling
                                          && aisling.Group.Contains(target, WorldEntity.IdComparer)),
            _ => throw new ArgumentOutOfRangeException(nameof(filter), filter, null)
        };

    //iterate over all flags present in the enum value
    //if they all result in true, this is a valid target
    public static bool IsValidTarget(this TargetFilter filter, Creature source, Creature target)
        => (filter == TargetFilter.None)
           || filter.GetFlags()
                    .All(flag => InnerIsValidTarget(flag, source, target));
}