using Chaos.Common.Definitions;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.Components;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ApplyDamage;
using Chaos.Scripting.SkillScripts.Abstractions;

namespace Chaos.Scripting.SkillScripts;

public class DamageScript : BasicSkillScriptBase, DamageComponent.IDamageComponentOptions
{
    public IApplyDamageScript ApplyDamageScript { get; init; }
    public IScript SourceScript { get; init; }
    protected DamageComponent DamageComponent { get; }

    /// <inheritdoc />
    public DamageScript(Skill subject)
        : base(subject)
    {
        ApplyDamageScript = ApplyAttackDamageScript.Create();
        DamageComponent = new DamageComponent();
        SourceScript = this;
    }

    /// <inheritdoc />
    public override void OnUse(ActivationContext context)
    {
        var targets = AbilityComponent.Activate<Creature>(context, this);
        DamageComponent.ApplyDamage(context, targets.TargetEntities, this);
    }

    #region ScriptVars
    public int? BaseDamage { get; init; }
    public Stat? DamageStat { get; init; }
    public decimal? DamageStatMultiplier { get; init; }
    public decimal? PctHpDamage { get; init; }
    public Element? Element { get; init; }
    #endregion
}