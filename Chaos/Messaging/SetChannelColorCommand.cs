#region
using Chaos.Collections.Common;
using Chaos.Extensions.Common;
using Chaos.Messaging.Abstractions;
using Chaos.Models.World;
using Chaos.Utilities;
#endregion

namespace Chaos.Messaging;

[Command("setchannelcolor", false, "<channelName> <messageColor>")]
public class SetChannelColorCommand(IChannelService channelService) : ICommand<Aisling>
{
    private readonly IChannelService ChannelService = channelService;

    /// <inheritdoc />
    public ValueTask ExecuteAsync(Aisling source, ArgumentCollection args)
    {
        if (!args.TryGetNext<string>(out var channelName))
            return default;

        if (!Helpers.TryGetMessageColor(args, out var messageColor))
            return default;

        channelName = ChannelService.PrependPrefix(channelName);

        if (!ChannelService.IsInChannel(source, channelName))
        {
            source.SendMessage($"You are not in channel {channelName}");

            return default;
        }

        var channelSettings = source.ChannelSettings.FirstOrDefault(x => x.ChannelName.EqualsI(channelName));

        if (channelSettings is null)
        {
            source.SendMessage($"You are not in channel {channelName}");

            return default;
        }

        channelSettings.MessageColor = messageColor;
        ChannelService.SetChannelColor(source, channelName, messageColor.Value);

        return default;
    }
}