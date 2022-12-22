using Chaos.Objects.World;
using Chaos.Scripts.MonsterScripts.Abstractions;
using Chaos.Scripts.RuntimeScripts;

namespace Chaos.Scripts.MonsterScripts.Components;

// ReSharper disable once ClassCanBeSealed.Global
public class DeathScript : MonsterScriptBase
{
    /// <inheritdoc />
    public DeathScript(Monster subject)
        : base(subject) { }

    /// <inheritdoc />
    public override void OnDeath()
    {
        if (!Map.RemoveObject(Subject))
            return;

        //this code will set the reward target to the person at the top of the aggro list
        //var rewardTarget = Subject.AggroList
        //                          .OrderByDescending(kvp => kvp.Value)
        //                          .Select(kvp => Map.TryGetObject<Aisling>(kvp.Key, out var a) ? a : null)
        //                          .FirstOrDefault(a => a is not null);

        //get the highest contributor
        //if there are no contributor, try getting the highest aggro
        var rewardTarget = Subject.Contribution
                                  .OrderByDescending(kvp => kvp.Value)
                                  .Select(kvp => Map.TryGetObject<Aisling>(kvp.Key, out var a) ? a : null)
                                  .FirstOrDefault(a => a is not null)
                           ?? Subject.AggroList
                                     .OrderByDescending(kvp => kvp.Value)
                                     .Select(kvp => Map.TryGetObject<Aisling>(kvp.Key, out var a) ? a : null)
                                     .FirstOrDefault(a => a is not null);

        Aisling[]? rewardTargets = null;

        if (rewardTarget != null)
            rewardTargets = rewardTarget.Group?.ToArray() ?? new[] { rewardTarget };

        if (Subject.LootTable != null)
            Subject.Items.AddRange(Subject.LootTable.GenerateLoot());

        var droppedGold = Subject.TryDropGold(Subject, Subject.Gold, out var money);
        var droppedITems = Subject.TryDrop(Subject, Subject.Items, out var groundItems);

        if (rewardTargets is not null)
        {
            if (droppedGold)
                money!.LockToCreatures(30, rewardTargets);

            if (droppedITems)
                foreach (var groundItem in groundItems!)
                    groundItem.LockToCreatures(30, rewardTargets);

            ExperienceDistributionScripts.Default.DistributeExperience(Subject, rewardTargets);
        }
    }
}