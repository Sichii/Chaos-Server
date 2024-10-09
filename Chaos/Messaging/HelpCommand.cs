using Chaos.Collections.Common;
using Chaos.DarkAges.Definitions;
using Chaos.Messaging.Abstractions;
using Chaos.Models.World;

namespace Chaos.Messaging;

[Command("help", false), Command("commands", false)]
public class HelpCommand : ICommand<Aisling>
{
    /// <inheritdoc />
    public ValueTask ExecuteAsync(Aisling source, ArgumentCollection args)
    {
        if (!args.TryGetNext<string>(out var helpText))
            return default;

        source.SendServerMessage(ServerMessageType.ScrollWindow, helpText);

        return default;
    }
}