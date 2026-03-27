#region
using Chaos.Collections.Common;
using Chaos.Extensions.Common;
using Chaos.Messaging.Abstractions;
using Chaos.Models.Data;
using Chaos.Models.World;
#endregion

namespace Chaos.Messaging;

[Command("joinchannel", false, "<channelName>")]
public class JoinChannelCommand(IChannelService channelService) : ICommand<Aisling>
{
    private readonly IChannelService ChannelService = channelService;

    /// <inheritdoc />
    public ValueTask ExecuteAsync(Aisling source, ArgumentCollection args)
    {
        if (!args.TryGetNext<string>(out var channelName))
            return default;

        channelName = ChannelService.PrependPrefix(channelName);

        if (channelName.EqualsI("!guild") && (source.Guild != null))
        {
            source.Guild.JoinChannel(source);

            return default;
        }

        if (ChannelService.JoinChannel(source, channelName))
            source.ChannelSettings.Add(new ChannelSettings(channelName, true));

        return default;
    }
}