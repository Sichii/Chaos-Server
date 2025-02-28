#region
using Chaos.Collections.Common;
using Chaos.Definitions;
using Chaos.Messaging.Abstractions;
using Chaos.Models.Data;
using Chaos.Models.World;
using Chaos.Utilities;
#endregion

namespace Chaos.Messaging;

[Command("createchannel", false, "<channelName>")]
public class CreateChannelCommand(IChannelService channelService) : ICommand<Aisling>
{
    private readonly IChannelService ChannelService = channelService;

    /// <inheritdoc />
    public ValueTask ExecuteAsync(Aisling source, ArgumentCollection args)
    {
        if (!args.TryGetNext<string>(out var channelName))
            return default;

        channelName = ChannelService.PrependPrefix(channelName);

        if (ChannelService.ContainsChannel(channelName))
        {
            source.SendMessage($"Channel {channelName} already exists");

            return default;
        }

        if (ChannelService.RegisterChannel(
                source,
                channelName,
                CHAOS_CONSTANTS.DEFAULT_CHANNEL_MESSAGE_COLOR,
                Helpers.DefaultChannelMessageHandler))
            source.ChannelSettings.Add(new ChannelSettings(channelName, true));

        return default;
    }
}