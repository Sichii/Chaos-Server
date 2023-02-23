using Chaos.Common.Definitions;
using Chaos.Data;
using Chaos.Definitions;
using Chaos.Objects.Panel;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripting.Components;

namespace Chaos.Scripting.SkillScripts.Abstractions;

public abstract class BasicSkillScriptBase : ConfigurableSkillScriptBase, AbilityComponent.IAbilityComponentOptions
{
    protected AbilityComponent AbilityComponent { get; }

    /// <inheritdoc />
    protected BasicSkillScriptBase(Skill subject)
        : base(subject) =>
        AbilityComponent = new AbilityComponent();

    /// <inheritdoc />
    public override void OnUse(ActivationContext context) => _ = AbilityComponent.Activate<Creature>(context, this);

    #region ScriptVars
    public AoeShape Shape { get; init; }
    public int Range { get; init; }
    public TargetFilter? Filter { get; init; }
    public BodyAnimation? BodyAnimation { get; init; }
    public Animation? Animation { get; init; }
    public byte? Sound { get; init; }
    public bool AnimatePoints { get; init; } = true;
    public bool MustHaveTargets { get; init; } = false;
    public bool IncludeSourcePoint { get; init; } = false;
    #endregion
}