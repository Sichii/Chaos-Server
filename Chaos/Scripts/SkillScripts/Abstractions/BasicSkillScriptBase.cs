using Chaos.Common.Definitions;
using Chaos.Data;
using Chaos.Definitions;
using Chaos.Objects.Panel;
using Chaos.Objects.World.Abstractions;
using Chaos.Scripts.Components;

namespace Chaos.Scripts.SkillScripts.Abstractions;

public abstract class BasicSkillScriptBase : ConfigurableSkillScriptBase
{
    protected AbilityComponent AbilityComponent { get; }
    protected AbilityComponent.AbilityComponentOptions AbilityComponentOptions { get; }

    /// <inheritdoc />
    protected BasicSkillScriptBase(Skill subject)
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
    public override void OnUse(ActivationContext context) => _ = AbilityComponent.Activate<Creature>(context, AbilityComponentOptions);

    #region ScriptVars
    protected AoeShape Shape { get; init; }
    protected int Range { get; init; }
    protected TargetFilter? Filter { get; init; }
    protected BodyAnimation? BodyAnimation { get; init; }
    protected Animation? Animation { get; init; }
    protected byte? Sound { get; init; }
    protected bool AnimatePoints { get; init; } = true;
    protected bool MustHaveTargets { get; init; } = false;
    protected bool IncludeSourcePoint { get; init; } = false;
    #endregion
}