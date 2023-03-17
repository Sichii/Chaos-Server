using Chaos.Collections.Common;
using Chaos.Messaging;
using Chaos.Messaging.Abstractions;
using Chaos.Objects.World;

namespace Chaos.Commands;

[Command("channellist", false)]
public class ChannelListCommand : ICommand<Aisling>
{
    /// <inheritdoc />
    public ValueTask ExecuteAsync(Aisling source, ArgumentCollection args)
    {
        foreach (var channelSettings in source.ChannelSettings)
            source.SendOrangeBarMessage(channelSettings.ChannelName);

        return default;
    }
}