using Chaos.Collections.Common;
using Chaos.Common.Definitions;
using Chaos.Extensions.Common;
using Chaos.Messaging.Abstractions;
using Chaos.Models.World;
using Chaos.Networking.Abstractions;

namespace Chaos.Messaging.Admin;

[Command("adminmessage", helpText: "<message>")]
public class AdminMessageCommand(IClientRegistry<IWorldClient> clientRegistry) : ICommand<Aisling>
{
    private readonly IClientRegistry<IWorldClient> ClientRegistry = clientRegistry;

    /// <inheritdoc />
    public ValueTask ExecuteAsync(Aisling source, ArgumentCollection args)
    {
        var message = args.ToString();

        if (string.IsNullOrEmpty(message))
            return default;

        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        foreach (var client in ClientRegistry)
            client.Aisling.SendActiveMessage($"{MessageColor.Blue.ToPrefix()}[Admin]: {message}");

        return default;
    }
}