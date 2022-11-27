using Chaos.CommandInterceptor;
using Chaos.CommandInterceptor.Abstractions;
using Chaos.Common.Collections;
using Chaos.Common.Definitions;
using Chaos.Extensions;
using Chaos.Extensions.Common;
using Chaos.Objects.World;

namespace Chaos.Commands;

[Command("adminmessage")]
public class AdminMessageCommand : ICommand<Aisling>
{
    private readonly IServiceProvider Provider;

    public AdminMessageCommand(IServiceProvider provider) => Provider = provider;

    /// <inheritdoc />
    public ValueTask ExecuteAsync(Aisling source, ArgumentCollection args)
    {
        var message = args.ToString();

        if (string.IsNullOrEmpty(message))
            return default;

        _ = Task.Run(
            async () =>
            {
                await foreach (var player in Provider.GetAislingsAsync())
                    player.Client.SendServerMessage(ServerMessageType.AdminMessage, $"{MessageColor.Silver.ToPrefix()}[Admin]: {message}");
            });

        return default;
    }
}