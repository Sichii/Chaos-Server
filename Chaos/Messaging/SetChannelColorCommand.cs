using Chaos.Collections.Common;
using Chaos.Common.Definitions;
using Chaos.Extensions.Common;
using Chaos.Messaging;
using Chaos.Messaging.Abstractions;
using Chaos.Models.World;

namespace Chaos.Commands;

[Command("setchannelcolor", false)]
public class SetChannelColorCommand : ICommand<Aisling>
{
    private readonly IChannelService ChannelService;

    public SetChannelColorCommand(IChannelService channelService) => ChannelService = channelService;

    /// <inheritdoc />
    public ValueTask ExecuteAsync(Aisling source, ArgumentCollection args)
    {
        if (!args.TryGetNext<string>(out var channelName))
            return default;

        if (!args.TryGetNext<MessageColor>(out var messageColor))
            return default;

        var channelSettings = source.ChannelSettings.FirstOrDefault(x => x.ChannelName.EqualsI(channelName));

        if (channelSettings is null)
            return default;

        channelSettings.MessageColor = messageColor;
        ChannelService.SetChannelColor(source, channelName, messageColor);

        return default;
    }
}