using System.Diagnostics;
using Chaos.Containers;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripts.Abstractions;
using Chaos.Utilities;

namespace Chaos.Scripts.MonsterScripts.Components;

public class DeathScript : MonsterScriptBase
{
    /// <inheritdoc />
    public DeathScript(Monster monster)
        : base(monster) { }

    /// <inheritdoc />
    public override void OnDeath(Creature source)
    {
        if (!Map.RemoveObject(Monster))
            return;

        base.OnDeath(source);

        Monster.Drop(Monster, Monster.Items);
        Monster.DropGold(Monster, Monster.Gold);

        if (source is Aisling aisling)
        {
            //TODO: distribute exp
        }

    }

    protected virtual void DistributeExp(Aisling source)
    {
        var group = source.Group?.ToList() ?? new List<Aisling> { source };
        var groupSizeDeductions = GetGroupSizeDeductions(group);
        var partyLevelDifferenceDeductions = GetPartyLevelDifferenceDeductions(group);
        var monsterLevelDeductions = GetMonsterLevelDifferenceDeductions(group);

        var groupMultiplier = Math.Max(0, 1 - (groupSizeDeductions + partyLevelDifferenceDeductions));
        var monsterLevelMultiplier = Math.Max(0, 1 - monsterLevelDeductions);
        var exp = Convert.ToInt64(Monster.Experience * groupMultiplier * monsterLevelMultiplier);

        foreach (var member in group)
        {
            member.UserStatSheet.AddTotalExp(exp);
        }
    }

    protected virtual decimal GetPartyLevelDifferenceDeductions(ICollection<Aisling> group)
    {
        var lowestMember = group.MinBy(p => p.StatSheet.Level)!;
        var highestMember = group.MaxBy(p => p.StatSheet.Level)!;

        if (lowestMember.WithinLevelRange(highestMember))
            return 0;

        var lowerBound = LevelRange.GetLowerBound(highestMember.StatSheet.Level);
        var stepSize = (highestMember.StatSheet.Level - lowerBound) / 2.0m;
        var faultSize = lowerBound - lowestMember.StatSheet.Level;

        return Math.Min(1, faultSize / stepSize * 0.25m);
    }

    protected virtual decimal GetMonsterLevelDifferenceDeductions(ICollection<Aisling> group)
    {
        var averageLevel = Convert.ToInt32(group.Average(p => p.StatSheet.Level));
        var monsterLevel = Monster.StatSheet.Level;

        var upperBound = LevelRange.GetUpperBound(averageLevel); 
        var lowerBound = LevelRange.GetLowerBound(averageLevel);

        if ((monsterLevel >= lowerBound) && (monsterLevel <= upperBound))
            return 0;
        
        var bounds = monsterLevel < averageLevel ? lowerBound : upperBound;
        var stepSize = Math.Abs(bounds - averageLevel) / 2.0m;
        var faultSize = Math.Abs(bounds - monsterLevel);

        return Math.Min(1, faultSize / stepSize * 0.25m);
    }

    protected virtual decimal GetGroupSizeDeductions(ICollection<Aisling> group) => group.Count switch
    {
        1 => 0,
        2 => 0,
        3 => 10,
        4 => 20,
        5 => 30,
        6 => 50,
        _ => throw new ArgumentOutOfRangeException(nameof(group.Count), "Group size is too large.")
    };
}