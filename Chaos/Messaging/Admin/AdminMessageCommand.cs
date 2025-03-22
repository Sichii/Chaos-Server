#region
using Chaos.Collections.Common;
using Chaos.DarkAges.Definitions;
using Chaos.DarkAges.Extensions;
using Chaos.Messaging.Abstractions;
using Chaos.Models.World;
using Chaos.Networking.Abstractions;
#endregion

namespace Chaos.Messaging.Admin;

[Command("adminmessage", helpText: "<message>")]
public class AdminMessageCommand(IClientRegistry<IChaosWorldClient> clientRegistry) : ICommand<Aisling>
{
    private readonly IClientRegistry<IChaosWorldClient> ClientRegistry = clientRegistry;

    /// <inheritdoc />
    public ValueTask ExecuteAsync(Aisling source, ArgumentCollection args)
    {
        if (!args.TryGetNext<string>(out var message))
            return default;

        if (string.IsNullOrEmpty(message))
            return default;

        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        foreach (var client in ClientRegistry)
            client.Aisling.SendActiveMessage($"{MessageColor.HotPink.ToPrefix()}[Admin]: {message}");

        return default;
    }
}