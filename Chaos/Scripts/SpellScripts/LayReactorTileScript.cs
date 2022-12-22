using Chaos.Geometry.Abstractions;
using Chaos.Objects;
using Chaos.Objects.Panel;
using Chaos.Scripts.SpellScripts.Abstractions;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripts.SpellScripts;

public class LayReactorTileScript : BasicSpellScriptBase
{
    private readonly IReactorTileFactory ReactorTileFactory;
    protected string ReactorTileTemplateKey { get; init; } = null!;

    /// <inheritdoc />
    public LayReactorTileScript(Spell subject, IReactorTileFactory reactorTileFactory)
        : base(subject) =>
        ReactorTileFactory = reactorTileFactory;

    /// <inheritdoc />
    public override void OnUse(SpellContext context)
    {
        ShowBodyAnimation(context);

        var affectedPoints = GetAffectedPoints(context).Cast<IPoint>().ToList();

        foreach (var point in affectedPoints)
        {
            var trap = ReactorTileFactory.Create(
                ReactorTileTemplateKey,
                context.Map,
                point,
                owner: context.Source);

            context.Map.SimpleAdd(trap);
        }

        ShowAnimation(context, affectedPoints);
        PlaySound(context, affectedPoints);
        context.SourceAisling?.SendActiveMessage($"You cast {Subject.Template.Name}");
    }
}