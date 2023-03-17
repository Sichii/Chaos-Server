using Chaos.Collections.Common;
using Chaos.Messaging;
using Chaos.Messaging.Abstractions;
using Chaos.Objects.World;
using Chaos.Utilities;

namespace Chaos.Commands.Admin;

[Command("shutdown")]
public class ShutdownCommand : ICommand<Aisling>
{
    private readonly IServiceProvider ServiceProvider;

    public ShutdownCommand(IServiceProvider serviceProvider) => ServiceProvider = serviceProvider;

    /// <inheritdoc />
    public ValueTask ExecuteAsync(Aisling source, ArgumentCollection args)
    {
        if (!args.TryGetNext<string>(out var messageFormat))
            return default;

        if (!args.TryGetNext<int>(out var mins))
            return default;

        ShutdownUtility.BeginShutdown(ServiceProvider, messageFormat, mins);

        return default;
    }
}