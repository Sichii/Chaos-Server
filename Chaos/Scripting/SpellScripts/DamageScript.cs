using Chaos.Common.Definitions;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.Components;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ApplyDamage;
using Chaos.Scripting.SpellScripts.Abstractions;

namespace Chaos.Scripting.SpellScripts;

public class DamageScript : BasicSpellScriptBase, DamageComponent.IDamageComponentOptions
{
    public IApplyDamageScript ApplyDamageScript { get; init; }
    public IScript SourceScript { get; init; }
    protected DamageComponent DamageComponent { get; }

    /// <inheritdoc />
    public DamageScript(Spell subject)
        : base(subject)
    {
        ApplyDamageScript = ApplyAttackDamageScript.Create();
        DamageComponent = new DamageComponent();
        SourceScript = this;
    }

    /// <inheritdoc />
    public override void OnUse(SpellContext context)
    {
        var targets = AbilityComponent.Activate<Creature>(context, this);
        context.SourceAisling?.SendActiveMessage($"You cast {Subject.Template.Name}");
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