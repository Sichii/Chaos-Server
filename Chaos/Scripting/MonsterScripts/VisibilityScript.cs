using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Behaviors;
using Chaos.Scripting.MonsterScripts.Abstractions;

namespace Chaos.Scripting.MonsterScripts;

public class VisibilityScript : MonsterScriptBase
{
    protected virtual VisibilityBehavior VisibilityBehavior { get; }

    /// <inheritdoc />
    public VisibilityScript(Monster subject)
        : base(subject) => VisibilityBehavior = new VisibilityBehavior();

    /// <inheritdoc />
    public override bool CanSee(VisibleEntity entity) => VisibilityBehavior.CanSee(Subject, entity);
}