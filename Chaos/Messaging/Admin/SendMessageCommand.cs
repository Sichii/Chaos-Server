using Chaos.Collections.Common;
using Chaos.DarkAges.Definitions;
using Chaos.Messaging.Abstractions;
using Chaos.Models.World;

namespace Chaos.Messaging.Admin;

[Command("sendmessage")]
public class SendMessageCommand : ICommand<Aisling>
{
    /// <inheritdoc />
    public ValueTask ExecuteAsync(Aisling source, ArgumentCollection args)
    {
        if (args.TryGetNext(out byte messageType) && args.TryGetNext<string>(out var message))
            source.Client.SendServerMessage((ServerMessageType)messageType, message.Replace("\"", string.Empty));

        return default;
    }
}