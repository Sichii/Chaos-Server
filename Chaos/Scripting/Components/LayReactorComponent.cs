using Chaos.Models.Data;
using Chaos.Scripting.Components.Abstractions;
using Chaos.Scripting.Components.Utilities;
using Chaos.Services.Factories.Abstractions;

namespace Chaos.Scripting.Components;

public class LayReactorComponent : IComponent
{
    /// <inheritdoc />
    public virtual void Execute(ActivationContext context, ComponentVars vars)
    {
        var options = vars.GetOptions<ILayReactorComponentOptions>();
        var map = context.TargetMap;

        if (string.IsNullOrEmpty(options.ReactorTileTemplateKey))
            return;

        var points = vars.GetPoints();

        foreach (var point in points)
        {
            var trap = options.ReactorTileFactory.Create(
                options.ReactorTileTemplateKey,
                map,
                point,
                owner: context.Source);

            map.SimpleAdd(trap);
        }
    }

    public interface ILayReactorComponentOptions
    {
        IReactorTileFactory ReactorTileFactory { get; init; }
        string? ReactorTileTemplateKey { get; init; }
    }
}