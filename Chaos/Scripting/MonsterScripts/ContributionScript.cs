#region
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.MonsterScripts.Abstractions;
#endregion

namespace Chaos.Scripting.MonsterScripts;

// ReSharper disable once ClassCanBeSealed.Global
public class ContributionScript : MonsterScriptBase
{
    /// <inheritdoc />
    public ContributionScript(Monster subject)
        : base(subject) { }

    /// <inheritdoc />
    public override void OnAttacked(Creature source, int damage, int? aggroOverride)
        => Subject.Contribution.AddContribution(source, damage);
}