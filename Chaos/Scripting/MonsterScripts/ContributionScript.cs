using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.MonsterScripts.Abstractions;

namespace Chaos.Scripting.MonsterScripts;

// ReSharper disable once ClassCanBeSealed.Global
public class ContributionScript : MonsterScriptBase
{
    /// <inheritdoc />
    public ContributionScript(Monster subject)
        : base(subject) { }

    /// <inheritdoc />
    public override void OnAttacked(Creature source, int damage, int? aggroOverride)
    {
        if (!Subject.Contribution.TryGetValue(source.Id, out var currentValue))
            currentValue = 0;

        Subject.Contribution[source.Id] = currentValue + damage;
    }
}