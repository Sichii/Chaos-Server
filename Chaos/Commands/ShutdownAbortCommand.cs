using Chaos.Collections.Common;
using Chaos.CommandInterceptor;
using Chaos.CommandInterceptor.Abstractions;
using Chaos.Objects.World;
using Chaos.Utilities;

namespace Chaos.Commands;

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