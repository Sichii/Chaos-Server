#region
using Chaos.Collections.Common;
using Chaos.Messaging.Abstractions;
using Chaos.Models.World;
#endregion

namespace Chaos.Messaging;

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