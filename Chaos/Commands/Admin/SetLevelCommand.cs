using Chaos.Collections.Common;
using Chaos.Common.Definitions;
using Chaos.Messaging;
using Chaos.Messaging.Abstractions;
using Chaos.Objects.World;

namespace Chaos.Commands.Admin;

[Command("setlevel")]
public class SetLevelCommand : ICommand<Aisling>
{
    /// <inheritdoc />
    public ValueTask ExecuteAsync(Aisling source, ArgumentCollection args)
    {
        if (!args.TryGetNext(out int level))
            return default;

        source.UserStatSheet.Assert(
            statref =>
            {
                statref.Level = level;
            });

        source.Client.SendAttributes(StatUpdateType.Full);

        return default;
    }
}