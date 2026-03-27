#region
using Chaos.Messaging;
using Chaos.Messaging.Abstractions;
using Microsoft.Extensions.Options;
#endregion

namespace Chaos.Testing.Infrastructure.Mocks;

public static class MockChannelService
{
    public static IChannelService Create(Action<ChannelServiceOptions>? setup = null)
    {
        var channelOptions = new ChannelServiceOptions
        {
            BlacklistedChannelNamePhrases = [],
            ChannelPrefix = "!",
            MaxChannelNameLength = 100,
            MinChannelNameLength = 1,
            ReservedChannelNames = []
        };

        setup?.Invoke(channelOptions);

        var options = Options.Create(channelOptions);
        var logger = MockLogger.Create<ChannelService>();

        return new ChannelService(options, logger.Object);
    }
}