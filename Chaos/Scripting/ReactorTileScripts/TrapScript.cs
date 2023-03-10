using Chaos.Common.Definitions;
using Chaos.Data;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Formulae;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.Components;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ApplyDamage;
using Chaos.Scripting.ReactorTileScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.ReactorTileScripts;

public class TrapScript : ConfigurableReactorTileScriptBase,
                          AbilityComponent.IAbilityComponentOptions,
                          DamageComponent.IDamageComponentOptions,
                          ManaDrainComponent.IManaDrainComponentOptions
{
    protected IEffectFactory EffectFactory { get; set; }
    protected Creature Owner { get; set; }
    protected IIntervalTimer? Timer { get; set; }
    protected int TriggerCount { get; set; }
    protected AbilityComponent AbilityComponent { get; }
    protected DamageComponent DamageComponent { get; }
    protected ManaDrainComponent ManaDrainComponent { get; }

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

        if (DurationSecs.HasValue)
            Timer = new IntervalTimer(TimeSpan.FromSeconds(DurationSecs.Value), false);

        ApplyDamageScript = ApplyAttackDamageScript.Create();
        ApplyDamageScript.DamageFormula = DamageFormulae.PureDamage;
        SourceScript = this;

        AbilityComponent = new AbilityComponent();
        DamageComponent = new DamageComponent();
        ManaDrainComponent = new ManaDrainComponent();
    }

    /// <inheritdoc />
    public override void OnWalkedOn(Creature source)
    {
        //if the person who stepped on it isnt a valid target, do nothing
        if (Filter.HasValue && !Filter.Value.IsValidTarget(Owner, source))
            return;

        var context = new ActivationContext(Owner, source);

        var targets = AbilityComponent.Activate<Creature>(context, this);

        if (MustHaveTargets && !targets.TargetEntities.Any())
            return;

        DamageComponent.ApplyDamage(context, targets.TargetEntities, this);
        ManaDrainComponent.ApplyManaDrain(targets.TargetEntities, this);

        if (!string.IsNullOrEmpty(EffectKey))
            foreach (var entity in targets.TargetEntities)
            {
                var effect = EffectFactory.Create(EffectKey);
                entity.Effects.Apply(context.Source, effect);
            }

        if (MaxTriggers.HasValue)
        {
            TriggerCount++;

            if (TriggerCount >= MaxTriggers)
                Map.RemoveObject(Subject);
        }
    }

    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        base.Update(delta);

        if (Timer != null)
        {
            Timer.Update(delta);

            if (Timer.IntervalElapsed)
                Map.RemoveObject(Subject);
        }
    }

    #region ScriptVars
    public IApplyDamageScript ApplyDamageScript { get; init; }
    public AoeShape Shape { get; init; }
    public BodyAnimation? BodyAnimation { get; init; }
    public int Range { get; init; }
    public TargetFilter? Filter { get; init; }
    public Animation? Animation { get; init; }
    public byte? Sound { get; init; }
    public bool AnimatePoints { get; init; }
    public bool MustHaveTargets { get; init; } = true;
    public bool IncludeSourcePoint { get; init; } = true;
    public int? BaseDamage { get; init; }
    public Stat? DamageStat { get; init; }
    public decimal? DamageStatMultiplier { get; init; }
    public decimal? PctHpDamage { get; init; }
    public IScript SourceScript { get; init; }
    /// <inheritdoc />
    public Element? Element { get; init; }
    public int? DurationSecs { get; init; }
    public int? MaxTriggers { get; init; }
    public string? EffectKey { get; init; }
    public int? ManaDrain { get; init; }
    public decimal PctManaDrain { get; init; }
    #endregion
}