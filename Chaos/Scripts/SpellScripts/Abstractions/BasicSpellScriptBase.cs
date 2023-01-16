using Chaos.Common.Definitions;
using Chaos.Data;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Objects.Panel;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripts.Components;

namespace Chaos.Scripts.SpellScripts.Abstractions;

public abstract class BasicSpellScriptBase : ConfigurableSpellScriptBase
{
    protected AbilityComponent AbilityComponent { get; }
    protected AbilityComponent.AbilityComponentOptions AbilityComponentOptions { get; }

    /// <inheritdoc />
    protected BasicSpellScriptBase(Spell subject)
        : base(subject)
    {
        AbilityComponent = new AbilityComponent();

        AbilityComponentOptions = new AbilityComponent.AbilityComponentOptions
        {
            Shape = Shape,
            Range = Range,
            Filter = Filter,
            BodyAnimation = BodyAnimation,
            Animation = Animation,
            Sound = Sound,
            AnimatePoints = AnimatePoints,
            MustHaveTargets = MustHaveTargets,
            IncludeSourcePoint = IncludeSourcePoint
        };
    }

    /// <inheritdoc />
    public override bool CanUse(SpellContext context) => (Filter ?? TargetFilter.None).IsValidTarget(context.Source, context.Target);

    /// <inheritdoc />
    public override void OnUse(SpellContext context)
    {
        _ = AbilityComponent.Activate<Creature>(context, AbilityComponentOptions);

        context.SourceAisling?.SendActiveMessage($"You cast {Subject.Template.Name}");
    }

    #region ScriptVars
    protected AoeShape Shape { get; init; }
    protected int Range { get; init; }
    protected TargetFilter? Filter { get; init; }
    protected BodyAnimation? BodyAnimation { get; init; }
    protected Animation? Animation { get; init; }
    protected byte? Sound { get; init; }
    protected bool AnimatePoints { get; init; } = true;
    protected bool MustHaveTargets { get; init; } = false;
    protected bool IncludeSourcePoint { get; init; } = true;
    #endregion
}