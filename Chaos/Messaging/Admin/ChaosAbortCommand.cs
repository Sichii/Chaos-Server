using Chaos.Collections.Common;
using Chaos.Messaging.Abstractions;
using Chaos.Models.World;
using Chaos.Utilities;

namespace Chaos.Messaging.Admin;

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