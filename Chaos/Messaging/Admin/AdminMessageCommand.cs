using Chaos.Collections.Common;
using Chaos.DarkAges.Definitions;
using Chaos.DarkAges.Extensions;
using Chaos.Messaging.Abstractions;
using Chaos.Models.World;
using Chaos.Networking.Abstractions;

namespace Chaos.Messaging.Admin;

[Command("adminmessage", helpText: "<message>")]
public class AdminMessageCommand(IClientRegistry<IChaosWorldClient> clientRegistry) : ICommand<Aisling>
{
    private readonly IClientRegistry<IChaosWorldClient> ClientRegistry = clientRegistry;

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