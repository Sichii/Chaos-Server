using Chaos.Common.Definitions;
using Chaos.Data;
using Chaos.Definitions;
using Chaos.Extensions;
using Chaos.Objects.Panel;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripts.Components;

namespace Chaos.Scripts.SpellScripts.Abstractions;

public abstract class BasicSpellScriptBase : ConfigurableSpellScriptBase, AbilityComponent.IAbilityComponentOptions
{
    protected AbilityComponent AbilityComponent { get; }

    /// <inheritdoc />
    protected BasicSpellScriptBase(Spell subject)
        : base(subject) =>
        AbilityComponent = new AbilityComponent();

    /// <inheritdoc />
    public override bool CanUse(SpellContext context) => (Filter ?? TargetFilter.None).IsValidTarget(context.Source, context.Target);

    /// <inheritdoc />
    public override void OnUse(SpellContext context)
    {
        _ = AbilityComponent.Activate<Creature>(context, this);

        context.SourceAisling?.SendActiveMessage($"You cast {Subject.Template.Name}");
    }

    #region ScriptVars
    public AoeShape Shape { get; init; }
    public int Range { get; init; }
    public TargetFilter? Filter { get; init; }
    public BodyAnimation? BodyAnimation { get; init; }
    public Animation? Animation { get; init; }
    public byte? Sound { get; init; }
    public bool AnimatePoints { get; init; } = true;
    public bool MustHaveTargets { get; init; } = false;
    public bool IncludeSourcePoint { get; init; } = true;
    #endregion
}