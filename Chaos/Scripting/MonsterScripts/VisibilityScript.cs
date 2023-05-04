using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Components;
using Chaos.Scripting.MonsterScripts.Abstractions;

namespace Chaos.Scripting.MonsterScripts;

public class VisibilityScript : MonsterScriptBase
{
    protected virtual VisibilityComponent VisibilityComponent { get; }

    /// <inheritdoc />
    public VisibilityScript(Monster subject)
        : base(subject) => VisibilityComponent = new VisibilityComponent();

    /// <inheritdoc />
    public override bool CanSee(VisibleEntity entity) => VisibilityComponent.CanSee(Subject, entity);
}