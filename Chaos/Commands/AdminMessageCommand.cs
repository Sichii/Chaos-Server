using Chaos.Clients.Abstractions;
using Chaos.CommandInterceptor;
using Chaos.CommandInterceptor.Abstractions;
using Chaos.Common.Collections;
using Chaos.Common.Definitions;
using Chaos.Extensions.Common;
using Chaos.Networking.Abstractions;
using Chaos.Objects.World;

namespace Chaos.Commands;

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
            client.Aisling.SendActiveMessage($"{MessageColor.Silver.ToPrefix()}[Admin]: {message}");

        return default;
    }
}