using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripts.MonsterScripts.Abstractions;

namespace Chaos.Scripts.MonsterScripts;

// ReSharper disable once ClassCanBeSealed.Global
public class ContributionScript : MonsterScriptBase
{
    /// <inheritdoc />
    public ContributionScript(Monster subject)
        : base(subject) { }

    /// <inheritdoc />
    public override void OnAttacked(Creature source, int damage, int? aggroOverride = null)
    {
        if (!Subject.Contribution.TryGetValue(source.Id, out var currentValue))
            currentValue = 0;

        Subject.Contribution[source.Id] = currentValue + damage;
    }
}