using Chaos.Collections.Common;
using Chaos.DarkAges.Definitions;
using Chaos.Extensions.Common;
using Chaos.Messaging.Abstractions;
using Chaos.Models.World;

namespace Chaos.Messaging.Admin;

[Command("mapFlag", helpText: "<add|remove> <flagName>")]
public class MapFlagCommand : ICommand<Aisling>
{
    /// <inheritdoc />
    public ValueTask ExecuteAsync(Aisling source, ArgumentCollection args)
    {
        if (!args.TryGetNext<string>(out var verb) || !args.TryGetNext<MapFlags>(out var flagValue))
        {
            source.SendOrangeBarMessage("Invalid arguments.");

            return default;
        }

        if (verb.EqualsI("add"))
            source.MapInstance.Flags |= flagValue;
        else if (verb.EqualsI("remove"))
            source.MapInstance.Flags &= ~flagValue;

        foreach (var aisling in source.MapInstance.GetEntities<Aisling>())
            aisling.Refresh(true);

        return default;
    }
}