using Chaos.Collections.Common;
using Chaos.DarkAges.Definitions;
using Chaos.Messaging.Abstractions;
using Chaos.Models.World;

namespace Chaos.Messaging.Admin;

[Command("setclass", helpText: "<class>")]
public class SetClassCommand : ICommand<Aisling>
{
    /// <inheritdoc />
    public ValueTask ExecuteAsync(Aisling source, ArgumentCollection args)
    {
        if (!args.TryGetNext<BaseClass>(out var @class))
            return default;

        source.UserStatSheet.SetBaseClass(@class);
        source.Client.SendAttributes(StatUpdateType.Full);
        source.Client.SendUserId();

        return default;
    }
}