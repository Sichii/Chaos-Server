using Chaos.Extensions.Common;
using Microsoft.Extensions.Options;

namespace Chaos.Messaging;

/// <summary>
///     A class used to configure the <see cref="ChannelServiceOptions" /> object
/// </summary>
public sealed class ChannelServiceOptionsConfigurer : IPostConfigureOptions<ChannelServiceOptions>
{
    /// <inheritdoc />
    public void PostConfigure(string? name, ChannelServiceOptions options)
    {
        for (var i = 0; i < options.ReservedChannelNames.Length; i++)
            options.ReservedChannelNames[i] = PrependPrefix(options.ChannelPrefix, options.ReservedChannelNames[i]);
    }

    private string PrependPrefix(string prefix, string channelName)
    {
        if (channelName.StartsWithI(prefix))
            return channelName;

        return prefix + channelName;
    }
}