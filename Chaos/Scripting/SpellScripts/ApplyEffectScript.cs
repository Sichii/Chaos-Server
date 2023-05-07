using Chaos.Common.Definitions;
using Chaos.Definitions;
using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Components;
using Chaos.Scripting.Components.Utilities;
using Chaos.Scripting.SpellScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.SpellScripts;

[SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global")]
public class ApplyEffectScript : ConfigurableSpellScriptBase,
                                 AbilityComponent<Creature>.IAbilityComponentOptions,
                                 ApplyEffectComponent.IApplyEffectComponentOptions
{
    /// <inheritdoc />
    public ApplyEffectScript(Spell subject, IEffectFactory effectFactory)
        : base(subject) =>
        EffectFactory = effectFactory;

    /// <inheritdoc />
    public override void OnUse(SpellContext context) =>
        new ComponentExecutor(context)
            .WithOptions(this)
            .ExecuteAndCheck<AbilityComponent<Creature>>()
            ?
            .Execute<ApplyEffectComponent>();

    #region ScriptVars
    /// <inheritdoc />
    public bool ShouldNotBreakHide { get; init; }
    /// <inheritdoc />
    public AoeShape Shape { get; init; }
    /// <inheritdoc />
    public TargetFilter Filter { get; init; }
    /// <inheritdoc />
    public int Range { get; init; }
    /// <inheritdoc />
    public bool ExcludeSourcePoint { get; init; }
    /// <inheritdoc />
    public bool MustHaveTargets { get; init; }
    /// <inheritdoc />
    public byte? Sound { get; init; }
    /// <inheritdoc />
    public BodyAnimation BodyAnimation { get; init; }
    /// <inheritdoc />
    public Animation? Animation { get; init; }
    /// <inheritdoc />
    public bool AnimatePoints { get; init; }
    /// <inheritdoc />
    public string? EffectKey { get; init; }
    /// <inheritdoc />
    public IEffectFactory EffectFactory { get; init; }
    /// <inheritdoc />
    public int? ManaCost { get; init; }
    /// <inheritdoc />
    public decimal PctManaCost { get; init; }
    #endregion
}