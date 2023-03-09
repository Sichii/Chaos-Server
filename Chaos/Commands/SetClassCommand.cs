using Chaos.Collections.Common;
using Chaos.CommandInterceptor;
using Chaos.CommandInterceptor.Abstractions;
using Chaos.Common.Definitions;
using Chaos.Objects.World;

namespace Chaos.Commands;

[Command("setclass")]
public class SetClassCommand : ICommand<Aisling>
{
    /// <inheritdoc />
    public ValueTask ExecuteAsync(Aisling source, ArgumentCollection args)
    {
        if (!args.TryGetNext<BaseClass>(out var @class))
            return default;

        source.UserStatSheet.SetBaseClass(@class);
        source.Client.SendAttributes(StatUpdateType.Full);

        return default;
    }
}