using Chaos.CommandInterceptor;
using Chaos.CommandInterceptor.Abstractions;
using Chaos.Common.Collections;
using Chaos.Common.Definitions;
using Chaos.Objects.World;

namespace Chaos.Commands;

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