using Chaos.Collections.Common;
using Chaos.Data;
using Chaos.Messaging;
using Chaos.Messaging.Abstractions;
using Chaos.Objects.World;

namespace Chaos.Commands;

[Command("joinchannel", false)]
public class JoinChannelCommand : ICommand<Aisling>
{
    private readonly IChannelService ChannelService;

    public JoinChannelCommand(IChannelService channelService) => ChannelService = channelService;

    /// <inheritdoc />
    public ValueTask ExecuteAsync(Aisling source, ArgumentCollection args)
    {
        if (!args.TryGetNext<string>(out var channelName))
            return default;

        source.ChannelSettings.Add(new ChannelSettings(channelName));
        ChannelService.JoinChannel(source, channelName);

        return default;
    }
}