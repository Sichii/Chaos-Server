#region
using Chaos.DarkAges.Definitions;
using Chaos.Definitions;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Components.AbilityComponents;
using Chaos.Scripting.Components.Execution;
using Chaos.Scripting.FunctionalScripts.Abstractions;
using Chaos.Scripting.FunctionalScripts.ApplyDamage;
using Chaos.Scripting.ReactorTileScripts.Abstractions;
using Chaos.Time;
using Chaos.Time.Abstractions;
#endregion

namespace Chaos.Scripting.ReactorTileScripts;

public sealed class CascadingDamageTileScript : ConfigurableReactorTileScriptBase,
                                                ICascadingTileScript,
                                                GetCascadingTargetsAbilityComponent<Creature>.IGetCascadingTargetsComponentOptions,
                                                DamageAbilityComponent.IDamageComponentOptions,
                                                SoundAbilityComponent.ISoundComponentOptions,
                                                AnimationAbilityComponent.IAnimationComponentOptions
{
    private readonly IIntervalTimer CascadeTimer;
    private readonly int EndingStage;
    private readonly IIntervalTimer SoundTimer;
    private readonly int StartingStage;
    public ComponentExecutor Executor { get; init; }
    private int Stages => Range;

    /// <inheritdoc />
    public CascadingDamageTileScript(ReactorTile subject)
        : base(subject)
    {
        ApplyDamageScript = ApplyAttackDamageScript.Create();
        CascadeTimer = new IntervalTimer(TimeSpan.FromMilliseconds(CascadeIntervalMs));
        SoundTimer = new IntervalTimer(TimeSpan.FromMilliseconds(MinSoundIntervalMs));

        var context = new ActivationContext(Subject.Owner!, Subject);
        var vars = new ComponentVars();

        if (InvertShape)
            StartingStage = Stages;
        else if (ExclusionRange.HasValue)
            StartingStage = ExclusionRange.Value + 1;
        else
            StartingStage = 0;

        vars.SetStage(StartingStage);

        // ReSharper disable once ConvertIfStatementToSwitchStatement
        if (InvertShape && ExclusionRange.HasValue)
            EndingStage = ExclusionRange.Value;
        else if (InvertShape)
            EndingStage = 0;
        else
            EndingStage = Stages;

        Executor = new ComponentExecutor(context, vars).WithOptions(this);
    }

    public bool HandleStage(ComponentVars vars)
    {
        if (InvertShape)
        {
            var stage = vars.GetStage() - 1;

            if (stage < EndingStage)
            {
                Subject.MapInstance.RemoveEntity(Subject);

                return false;
            }

            vars.SetStage(stage);
        } else
        {
            var stage = vars.GetStage() + 1;

            if (stage > EndingStage)
            {
                Subject.MapInstance.RemoveEntity(Subject);

                return false;
            }

            vars.SetStage(stage);
        }

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
            Executor.ExecuteAndCheck<GetCascadingTargetsAbilityComponent<Creature>>()
                    ?.Execute<DamageAbilityComponent>()
                    .Execute<AnimationAbilityComponent>()
                    .Check(ShouldPlaySound)
                    ?.Execute<SoundAbilityComponent>();

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
    public int? ExclusionRange { get; init; }

    /// <inheritdoc />
    public AoeShape Shape { get; init; }

    /// <inheritdoc />
    public bool InvertShape { get; init; }

    /// <inheritdoc />
    public bool IgnoreWalls { get; init; }

    /// <inheritdoc />
    public TargetFilter Filter { get; init; }

    /// <inheritdoc />
    public int Range { get; init; }

    /// <inheritdoc />
    public bool ExcludeSourcePoint { get; init; }

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
    public byte? Sound { get; init; }

    /// <inheritdoc />
    public Animation? Animation { get; init; }

    /// <inheritdoc />
    public bool AnimatePoints { get; init; }
    #endregion
}