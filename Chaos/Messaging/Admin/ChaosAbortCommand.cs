using Chaos.Collections.Common;
using Chaos.Messaging;
using Chaos.Messaging.Abstractions;
using Chaos.Models.World;
using Chaos.Utilities;

namespace Chaos.Commands.Admin;

[Command("chaosabort")]
public class ChaosAbortCommand : ICommand<Aisling>
{
    /// <inheritdoc />
    public ValueTask ExecuteAsync(Aisling source, ArgumentCollection args)
    {
        ShutdownUtility.AbortShutdown();

        return default;
    }
}