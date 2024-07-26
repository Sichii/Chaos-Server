using Chaos.Definitions;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Components.AbilityComponents;
using Chaos.Scripting.Components.EffectComponents;
using Chaos.Scripting.Components.Execution;
using Chaos.Scripting.EffectScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.EffectScripts;

public class ApplyNonSpellEffect : EffectBase,
                            GetTargetsAbilityComponent<Creature>.IGetTargetsComponentOptions,
                            SoundAbilityComponent.ISoundComponentOptions,
                            ApplyEffectAbilityComponent.IApplyEffectComponentOptions
{
    
    public ApplyNonSpellEffect(IEffectFactory effectFactory) => EffectFactory = effectFactory;

    /// <inheritdoc />
    protected override TimeSpan Duration { get; set; } = TimeSpan.FromMilliseconds(10000);
    
    /// <inheritdoc />
    public bool ExcludeSourcePoint { get; init; }

    /// <inheritdoc />
    public TargetFilter Filter { get; init; }

    /// <inheritdoc />
    public bool MustHaveTargets { get; init; }

    /// <inheritdoc />
    public int Range { get; init; }

    /// <inheritdoc />
    public AoeShape Shape { get; init; }

    /// <inheritdoc />
    public bool SingleTarget { get; init; } = true;

    /// <inheritdoc />
    public byte? Sound { get; init; } = 97;

    /// <inheritdoc />
    public override byte Icon => 5;

    /// <inheritdoc />
    public override string Name => "skulled";

    /// <inheritdoc />
    public override void OnApplied()
        => new ComponentExecutor(Subject, Subject).WithOptions(this)
            .ExecuteAndCheck<GetTargetsAbilityComponent<Creature>>()
            ?.Execute<ApplyEffectAbilityComponent>();
    
    /// <inheritdoc />
    public override bool ShouldApply(Creature source, Creature target)
    {
        var execution = new ComponentExecutor(source, target).WithOptions(this)
                                                             .ExecuteAndCheck<NonOverwritableEffectComponent>();
        return execution is not null;
    }

    public TimeSpan? EffectDurationOverride { get; init; }
    
    public IEffectFactory EffectFactory { get; init; }
    
    public string? EffectKey { get; init; }
}
