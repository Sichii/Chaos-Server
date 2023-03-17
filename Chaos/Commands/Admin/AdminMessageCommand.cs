using Chaos.Clients.Abstractions;
using Chaos.Collections.Common;
using Chaos.Common.Definitions;
using Chaos.Extensions.Common;
using Chaos.Messaging;
using Chaos.Messaging.Abstractions;
using Chaos.Networking.Abstractions;
using Chaos.Objects.World;

namespace Chaos.Commands.Admin;

[Command("adminmessage")]
public class AdminMessageCommand : ICommand<Aisling>
{
    private readonly IClientRegistry<IWorldClient> ClientRegistry;

    public AdminMessageCommand(IClientRegistry<IWorldClient> clientRegistry) => ClientRegistry = clientRegistry;

    /// <inheritdoc />
    public ValueTask ExecuteAsync(Aisling source, ArgumentCollection args)
    {
        var message = args.ToString();

        if (string.IsNullOrEmpty(message))
            return default;

        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        foreach (var client in ClientRegistry.Where(c => c.Aisling != null))
            client.Aisling.SendActiveMessage($"{MessageColor.Blue.ToPrefix()}[Admin]: {message}");

        return default;
    }
}