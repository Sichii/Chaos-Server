using Chaos.Models.Data;
using Chaos.Models.Panel;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.SpellScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.SpellScripts;

public class LayReactorTileScript : BasicSpellScriptBase
{
    protected IReactorTileFactory ReactorTileFactory { get; set; }

    #region ScriptVars
    protected string ReactorTileTemplateKey { get; init; } = null!;
    #endregion

    /// <inheritdoc />
    public LayReactorTileScript(Spell subject, IReactorTileFactory reactorTileFactory)
        : base(subject) =>
        ReactorTileFactory = reactorTileFactory;

    /// <inheritdoc />
    public override void OnUse(SpellContext context)
    {
        var targets = AbilityComponent.Activate<Creature>(context, this);

        context.SourceAisling?.SendActiveMessage($"You cast {Subject.Template.Name}");

        foreach (var point in targets.TargetPoints)
        {
            var trap = ReactorTileFactory.Create(
                ReactorTileTemplateKey,
                context.Map,
                point,
                owner: context.Target);

            context.Map.SimpleAdd(trap);
        }
    }
}