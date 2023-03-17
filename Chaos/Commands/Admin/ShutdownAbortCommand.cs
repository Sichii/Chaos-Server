using Chaos.Collections.Common;
using Chaos.Messaging;
using Chaos.Messaging.Abstractions;
using Chaos.Objects.World;
using Chaos.Utilities;

namespace Chaos.Commands.Admin;

[Command("shutdownabort")]
public class ShutdownAbortCommand : ICommand<Aisling>
{
    /// <inheritdoc />
    public ValueTask ExecuteAsync(Aisling source, ArgumentCollection args)
    {
        ShutdownUtility.AbortShutdown();

        return default;
    }
}