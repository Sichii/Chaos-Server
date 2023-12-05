using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Abstractions;
using Chaos.Scripting.Components;
using Chaos.Scripting.Components.Utilities;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ApplyDamage;
using Chaos.Scripting.ReactorTileScripts.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.ReactorTileScripts;

public sealed class CascadingDamageTileScript : ConfigurableReactorTileScriptBase,
                                                ICascadingTileScript,
                                                GetCascadingTargetsComponent<Creature>.IGetCascadingTargetsComponentOptions,
                                                DamageComponent.IDamageComponentOptions,
                                                SoundComponent.ISoundComponentOptions,
                                                AnimationComponent.IAnimationComponentOptions
{
    private readonly IIntervalTimer CascadeTimer;
    private readonly IIntervalTimer SoundTimer;
    public ComponentExecutor Executor { get; init; }
    private int Stages => Range;

    /// <inheritdoc />
    public CascadingDamageTileScript(ReactorTile subject)
        : base(subject)
    {
        ApplyDamageScript = ApplyAttackDamageScript.Create();
        SourceScript = this;
        CascadeTimer = new IntervalTimer(TimeSpan.FromMilliseconds(CascadeIntervalMs));
        SoundTimer = new IntervalTimer(TimeSpan.FromMilliseconds(MinSoundIntervalMs));

        var context = new ActivationContext(Subject.Owner!, Subject);
        var vars = new ComponentVars();
        vars.SetStage(0);

        Executor = new ComponentExecutor(context, vars).WithOptions(this);
    }

    public bool HandleStage(ComponentVars vars)
    {
        var stage = vars.GetStage() + 1;

        if (stage >= Stages)
        {
            Subject.MapInstance.RemoveObject(Subject);

            return false;
        }

        vars.SetStage(stage);

        return true;
    }

    public bool ShouldPlaySound(ComponentVars _) => SoundTimer.IntervalElapsed;

    /// <inheritdoc />
    public override void Update(TimeSpan delta)
    {
        CascadeTimer.Update(delta);

        //if sound timer is elapsed, we don't want to update it here and cause it to be reset
        //the idea here is that there is a minimum interval between sounds (the sound cant play too often)
        //but we also only want the sound to play if this iteration of the effect actually did anything
        if (!SoundTimer.IntervalElapsed)
            SoundTimer.Update(delta);

        //only execute if the cascade timer is elapsed
        if (CascadeTimer.IntervalElapsed)
        {
            Executor.ExecuteAndCheck<GetCascadingTargetsComponent<Creature>>()
                    ?.Execute<DamageComponent>()
                    .Execute<AnimationComponent>()
                    .Check(ShouldPlaySound)
                    ?.Execute<SoundComponent>();

            //if the sound timer is elapsed, the predicate above will play the sound
            //however, we still need to reset it
            if (SoundTimer.IntervalElapsed)
                SoundTimer.Update(delta);

            //handle stange increment and currentstage >= stages
            Executor.Check(HandleStage);
        }
    }

    #region ScriptVars
    public int MinSoundIntervalMs { get; init; }
    public int CascadeIntervalMs { get; init; }

    /// <inheritdoc />
    public AoeShape Shape { get; init; }

    /// <inheritdoc />
    public bool IgnoreWalls { get; init; }

    /// <inheritdoc />
    public TargetFilter Filter { get; init; }

    /// <inheritdoc />
    public int Range { get; init; }

    /// <inheritdoc />
    public bool IncludeSourcePoint { get; init; }

    /// <inheritdoc />
    public bool MustHaveTargets { get; init; }

    /// <inheritdoc />
    public IApplyDamageScript ApplyDamageScript { get; init; }

    /// <inheritdoc />
    public int? BaseDamage { get; init; }

    /// <inheritdoc />
    public Stat? DamageStat { get; init; }

    /// <inheritdoc />
    public decimal? DamageStatMultiplier { get; init; }

    /// <inheritdoc />
    public Element? Element { get; init; }

    /// <inheritdoc />
    public decimal? PctHpDamage { get; init; }

    /// <inheritdoc />
    public IScript SourceScript { get; init; }

    /// <inheritdoc />
    public byte? Sound { get; init; }

    /// <inheritdoc />
    public Animation? Animation { get; init; }

    /// <inheritdoc />
    public bool AnimatePoints { get; init; }
    #endregion
}