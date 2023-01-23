using Chaos.Common.Definitions;
using Chaos.Data;
using Chaos.Definitions;
using Chaos.Objects.Panel;
using Chaos.Scripts.Components;
using Chaos.Scripts.FunctionalScripts.Abstractions;
using Chaos.Scripts.FunctionalScripts.ApplyDamage;
using Chaos.Scripts.SpellScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripts.SpellScripts;

public class CascadeDamageScript : ConfigurableSpellScriptBase
{
    protected IApplyDamageScript ApplyDamageScript { get; }

    protected CascadeAbilityComponent CascadeAbilityComponent { get; }
    protected CascadeAbilityComponent.CascadeAbilityComponentOptions CascadeAbilityComponentOptions { get; }
    protected DamageComponent DamageComponent { get; }
    protected DamageComponent.DamageComponentOptions DamageComponentOptions { get; }

    protected IEffectFactory EffectFactory { get; }

    /// <inheritdoc />
    public CascadeDamageScript(Spell subject, IEffectFactory effectFactory)
        : base(subject)
    {
        EffectFactory = effectFactory;
        CascadeAbilityComponent = new CascadeAbilityComponent();

        ApplyDamageScript = DefaultApplyDamageScript.Create();
        DamageComponent = new DamageComponent();

        DamageComponentOptions = new DamageComponent.DamageComponentOptions
        {
            ApplyDamageScript = ApplyDamageScript,
            SourceScript = this,
            BaseDamage = BaseDamage,
            DamageStatMultiplier = DamageStatMultiplier,
            DamageStat = DamageStat
        };

        CascadeAbilityComponentOptions = new CascadeAbilityComponent.CascadeAbilityComponentOptions
        {
            Shape = Shape,
            Range = Range,
            Sound = Sound,
            Filter = Filter,
            Animation = Animation,
            BodyAnimation = BodyAnimation,
            AnimatePoints = AnimatePoints,
            IncludeSourcePoint = IncludeSourcePoint,
            DamageComponentOptions = DamageComponentOptions,
            PropagationIntervalMs = PropagationIntervalMs,
            SoundIntervalMs = SoundIntervalMs,
            StopAtWalls = StopAtWalls,
            EffectKey = EffectKey
        };
    }

    /// <inheritdoc />
    public override void OnUse(SpellContext context) =>
        CascadeAbilityComponent.Activate(context, CascadeAbilityComponentOptions, EffectFactory);

    #region ScriptVars
    protected AoeShape Shape { get; init; }
    protected int Range { get; init; }
    protected TargetFilter? Filter { get; init; }
    protected BodyAnimation? BodyAnimation { get; init; }
    protected Animation? Animation { get; init; }
    protected byte? Sound { get; init; }
    protected bool AnimatePoints { get; init; } = true;
    protected bool MustHaveTargets { get; init; }
    protected bool IncludeSourcePoint { get; init; } = true;
    protected int SoundIntervalMs { get; init; }
    protected int PropagationIntervalMs { get; init; }
    protected bool StopAtWalls { get; init; }
    protected int? BaseDamage { get; init; }
    protected Stat? DamageStat { get; init; }
    protected decimal? DamageStatMultiplier { get; init; }
    protected string? EffectKey { get; init; }
    #endregion
}