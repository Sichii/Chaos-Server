using Chaos.Collections.Common;
using Chaos.Common.Definitions;
using Chaos.Extensions.Common;
using Chaos.Messaging.Abstractions;
using Chaos.Models.World;

namespace Chaos.Messaging.Admin;

[Command("setLightLevel", helpText: "<lightLevel, auto>")]
public class SetLightLevelCommand : ICommand<Aisling>
{
    /// <inheritdoc />
    public ValueTask ExecuteAsync(Aisling source, ArgumentCollection args)
    {
        if (args.TryGetNext(out LightLevel lightLevel))
        {
            source.MapInstance.AutoDayNightCycle = false;
            source.MapInstance.CurrentLightLevel = lightLevel;

            foreach (var aisling in source.MapInstance.GetEntities<Aisling>())
                aisling.Client.SendLightLevel(lightLevel);
        } else if (args.TryGetNext<string>(out var auto) && auto.EqualsI("auto"))
            source.MapInstance.AutoDayNightCycle = true;

        return default;
    }
}