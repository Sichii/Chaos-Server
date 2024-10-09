using Chaos.Collections.Common;
using Chaos.DarkAges.Definitions;
using Chaos.Messaging.Abstractions;
using Chaos.Models.World;

namespace Chaos.Messaging.Admin;

[Command("setlevel", helpText: "<level>")]
public class SetLevelCommand : ICommand<Aisling>
{
    /// <inheritdoc />
    public ValueTask ExecuteAsync(Aisling source, ArgumentCollection args)
    {
        if (!args.TryGetNext(out int level))
            return default;

        source.UserStatSheet.SetLevel(level);

        source.Client.SendAttributes(StatUpdateType.Full);

        return default;
    }
}