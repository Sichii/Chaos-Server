using Chaos.Common.Definitions;
using Chaos.Data;
using Chaos.Definitions;
using Chaos.Objects.Panel;
using Chaos.Scripting.Abstractions;
using Chaos.Scripts.Components;
using Chaos.Scripts.FunctionalScripts.Abstractions;
using Chaos.Scripts.FunctionalScripts.ApplyDamage;
using Chaos.Scripts.SpellScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripts.SpellScripts;

public class CascadeDamageScript : ConfigurableSpellScriptBase, CascadeAbilityComponent.ICascadeAbilityComponentOptions
{
    protected CascadeAbilityComponent CascadeAbilityComponent { get; }
    protected DamageComponent DamageComponent { get; }

    protected IEffectFactory EffectFactory { get; }

    /// <inheritdoc />
    public CascadeDamageScript(Spell subject, IEffectFactory effectFactory)
        : base(subject)
    {
        EffectFactory = effectFactory;
        CascadeAbilityComponent = new CascadeAbilityComponent();

        ApplyDamageScript = DefaultApplyDamageScript.Create();
        DamageComponent = new DamageComponent();
        SourceScript = this;
    }

    /// <inheritdoc />
    public override void OnUse(SpellContext context) =>
        CascadeAbilityComponent.Activate(context, this, EffectFactory);

    #region ScriptVars
    public IApplyDamageScript ApplyDamageScript { get; init; }
    public AoeShape Shape { get; init; }
    public int Range { get; init; }
    public TargetFilter? Filter { get; init; }
    public BodyAnimation? BodyAnimation { get; init; }
    public Animation? Animation { get; init; }
    public byte? Sound { get; init; }
    public bool AnimatePoints { get; init; } = true;
    public bool MustHaveTargets { get; init; }
    public bool IncludeSourcePoint { get; init; } = true;
    public int SoundIntervalMs { get; init; }
    public int PropagationIntervalMs { get; init; }
    public bool StopAtWalls { get; init; }
    public int? BaseDamage { get; init; }
    public Stat? DamageStat { get; init; }
    public decimal? DamageStatMultiplier { get; init; }
    public decimal? PctHpDamage { get; init; }
    public IScript SourceScript { get; init; }
    public string? EffectKey { get; init; }
    #endregion
}