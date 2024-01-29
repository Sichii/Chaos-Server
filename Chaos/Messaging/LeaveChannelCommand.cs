using Chaos.Collections.Common;
using Chaos.Messaging.Abstractions;
using Chaos.Models.Data;
using Chaos.Models.World;

namespace Chaos.Messaging;

[Command("leavechannel", false, "<channelName>")]
public class LeaveChannelCommand(IChannelService channelService) : ICommand<Aisling>
{
    private readonly IChannelService ChannelService = channelService;

    /// <inheritdoc />
    public ValueTask ExecuteAsync(Aisling source, ArgumentCollection args)
    {
        if (!args.TryGetNext<string>(out var channelName))
            return default;

        channelName = ChannelService.PrependPrefix(channelName);

        source.ChannelSettings.Remove(new ChannelSettings(channelName));
        ChannelService.LeaveChannel(source, channelName);

        return default;
    }
}