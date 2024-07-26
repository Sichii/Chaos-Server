using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Scripting.Components.Abstractions;
using Chaos.Scripting.Components.Execution;

namespace Chaos.Scripting.Components.AbilityComponents;

public struct SoundAbilityComponent : IComponent
{
    /// <inheritdoc />
    public void Execute(ActivationContext context, ComponentVars vars)
    {
        var options = vars.GetOptions<ISoundComponentOptions>();
        var points = vars.GetPoints();

        if (!options.Sound.HasValue)
            return;

        context.TargetMap.PlaySound(options.Sound.Value, points.ToArray());
    }

    public interface ISoundComponentOptions
    {
        byte? Sound { get; init; }
    }
}

public struct SoundClientComponent : IComponent
{
    /// <inheritdoc />
    public void Execute(ActivationContext context, ComponentVars vars)
    {
        var options = vars.Get<Aisling>("Subject");
        Console.WriteLine("This is a log message.");
    }

    public interface ISoundClientComponent
    {
        byte? Sound { get; init; }
        Aisling Aisling { get; init; }
    }
}