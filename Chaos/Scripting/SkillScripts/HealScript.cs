using Chaos.Common.Definitions;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.Components;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.SkillScripts.Abstractions;

namespace Chaos.Scripting.SkillScripts;

public class HealScript : BasicSkillScriptBase, HealComponent.IHealComponentOptions
{
    public IApplyHealScript ApplyHealScript { get; init; }
    public IScript SourceScript { get; init; }
    protected HealComponent HealComponent { get; }

    public HealScript(Skill subject)
        : base(subject)
    {
        ApplyHealScript = FunctionalScripts.ApplyHealing.HealScript.Create();
        HealComponent = new HealComponent();
        SourceScript = this;
    }

    /// <inheritdoc />
    public override void OnUse(ActivationContext context)
    {
        var targets = AbilityComponent.Activate<Creature>(context, this);
        HealComponent.ApplyHeal(context, targets.TargetEntities, this);
    }

    #region ScriptVars
    public int? BaseHeal { get; init; }
    public Stat? HealStat { get; init; }
    public decimal? HealStatMultiplier { get; init; }
    public decimal? PctHpHeal { get; init; }
    #endregion
}