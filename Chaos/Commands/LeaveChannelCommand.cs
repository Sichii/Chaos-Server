using Chaos.Collections.Common;
using Chaos.Data;
using Chaos.Messaging;
using Chaos.Messaging.Abstractions;
using Chaos.Objects.World;

namespace Chaos.Commands;

[Command("leavechannel", false)]
public class LeaveChannelCommand : ICommand<Aisling>
{
    private readonly IChannelService ChannelService;

    public LeaveChannelCommand(IChannelService channelService) => ChannelService = channelService;

    /// <inheritdoc />
    public ValueTask ExecuteAsync(Aisling source, ArgumentCollection args)
    {
        if (!args.TryGetNext<string>(out var channelName))
            return default;

        source.ChannelSettings.Remove(new ChannelSettings(channelName));
        ChannelService.LeaveChannel(source, channelName);

        return default;
    }
}