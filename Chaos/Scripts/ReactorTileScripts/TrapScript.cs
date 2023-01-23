using Chaos.Common.Definitions;
using Chaos.Data;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Formulae;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripts.Components;
using Chaos.Scripts.FunctionalScripts.Abstractions;
using Chaos.Scripts.FunctionalScripts.ApplyDamage;
using Chaos.Scripts.ReactorTileScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripts.ReactorTileScripts;

public class TrapScript : ConfigurableReactorTileScriptBase
{
    protected IApplyDamageScript ApplyDamageScript { get; set; }
    protected IEffectFactory EffectFactory { get; set; }
    protected Creature Owner { get; set; }
    protected IIntervalTimer Timer { get; set; }
    protected int TriggerCount { get; set; }
    protected AbilityComponent AbilityComponent { get; }
    protected AbilityComponent.AbilityComponentOptions AbilityComponentOptions { get; }
    protected DamageComponent DamageComponent { get; }
    protected DamageComponent.DamageComponentOptions DamageComponentOptions { get; }

    /// <inheritdoc />
    public TrapScript(ReactorTile subject, IEffectFactory effectFactory)
        : base(subject)
    {
        if (Subject.Owner == null)
            throw new Exception(
                $"""
{nameof(TrapScript)} script initialized fo {Subject} that has no owner. 
If this reactor was created through json, you must specify the optional parameter "owningMonsterTemplateKey". 
If this reactor was created through a script, you must specify the owner in the {nameof(IReactorTileFactory)}.{
    nameof(IReactorTileFactory.Create)}() call.
""");

        Owner = subject.Owner!;
        EffectFactory = effectFactory;
        TriggerCount = 0;
        Timer = new IntervalTimer(TimeSpan.FromSeconds(DurationSecs), false);
        ApplyDamageScript = DefaultApplyDamageScript.Create();
        ApplyDamageScript.DamageFormula = DamageFormulae.PureDamage;

        AbilityComponent = new AbilityComponent();

        AbilityComponentOptions = new AbilityComponent.AbilityComponentOptions
        {
            Shape = Shape,
            Range = Range,
            Filter = Filter,
            BodyAnimation = null,
            Animation = Animation,
            Sound = Sound,
            AnimatePoints = AnimatePoints,
            MustHaveTargets = MustHaveTargets,
            IncludeSourcePoint = IncludeSourcePoint
        };

        DamageComponent = new DamageComponent();

        DamageComponentOptions = new DamageComponent.DamageComponentOptions
        {
            ApplyDamageScript = ApplyDamageScript,
            SourceScript = this,
            BaseDamage = BaseDamage,
            DamageStatMultiplier = DamageStatMultiplier,
            DamageStat = DamageStat
        };
    }

    /// <inheritdoc />
    public override void OnWalkedOn(Creature source)
    {
        //if the person who stepped on it isnt a valid target, do nothing
        if (Filter.HasValue && !Filter.Value.IsValidTarget(Owner, source))
            return;

        var context = new ActivationContext(Owner, source);

        var targets = AbilityComponent.Activate<Creature>(context, AbilityComponentOptions);

        if (MustHaveTargets && !targets.TargetEntities.Any())
            return;

        DamageComponent.ApplyDamage(context, targets.TargetEntities, DamageComponentOptions);

        if (!string.IsNullOrEmpty(EffectKey))
            foreach (var entity in targets.TargetEntities)
            {
                var effect = EffectFactory.Create(EffectKey);
                entity.Effects.Apply(context.Source, effect);
            }

        TriggerCount++;

        if (TriggerCount >= MaxTriggers)
            Map.RemoveObject(Subject);
    }

    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        base.Update(delta);

        Timer.Update(delta);

        if (Timer.IntervalElapsed)
            Map.RemoveObject(Subject);
    }

    #region ScriptVars
    protected AoeShape Shape { get; init; }
    protected int Range { get; init; }
    protected TargetFilter? Filter { get; init; }
    protected Animation? Animation { get; init; }
    protected byte? Sound { get; init; }
    protected bool AnimatePoints { get; init; }
    protected bool MustHaveTargets { get; init; } = true;
    protected bool IncludeSourcePoint { get; init; } = true;
    protected int? BaseDamage { get; init; }
    protected Stat? DamageStat { get; init; }
    protected decimal? DamageStatMultiplier { get; init; }
    protected int DurationSecs { get; init; }
    protected int MaxTriggers { get; init; }
    protected string? EffectKey { get; init; }
    #endregion
}