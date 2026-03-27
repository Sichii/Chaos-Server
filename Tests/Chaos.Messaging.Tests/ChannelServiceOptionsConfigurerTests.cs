#region
using FluentAssertions;
using Microsoft.Extensions.Options;
#endregion

namespace Chaos.Messaging.Tests;

public sealed class ChannelServiceOptionsConfigurerTests
{
    [Test]
    public void PostConfigure_Should_Be_Case_Insensitive_When_Detecting_Existing_Prefix()
    {
        var options = new ChannelServiceOptions
        {
            ChannelPrefix = "SYS-",
            ReservedChannelNames = new[]
            {
                "sys-foo",
                "SYS-bar",
                "Baz"
            },
            BlacklistedChannelNamePhrases = Array.Empty<string>(),
            MaxChannelNameLength = 64,
            MinChannelNameLength = 1
        };

        IPostConfigureOptions<ChannelServiceOptions> configurer = new ChannelServiceOptionsConfigurer();

        configurer.PostConfigure(null, options);

        options.ReservedChannelNames
               .Should()
               .BeEquivalentTo("sys-foo", "SYS-bar", "SYS-Baz");
    }

    [Test]
    public void PostConfigure_Should_Handle_Empty_Prefix_By_Leaving_Names_Unchanged()
    {
        var options = new ChannelServiceOptions
        {
            ChannelPrefix = string.Empty,
            ReservedChannelNames = new[]
            {
                "alpha",
                "BETA"
            },
            BlacklistedChannelNamePhrases = Array.Empty<string>(),
            MaxChannelNameLength = 64,
            MinChannelNameLength = 1
        };

        IPostConfigureOptions<ChannelServiceOptions> configurer = new ChannelServiceOptionsConfigurer();

        configurer.PostConfigure(null, options);

        options.ReservedChannelNames
               .Should()
               .ContainInOrder("alpha", "BETA");
    }

    [Test]
    public void PostConfigure_Should_Prefix_All_Reserved_Names_When_Missing()
    {
        var options = new ChannelServiceOptions
        {
            ChannelPrefix = "sys-",
            ReservedChannelNames = new[]
            {
                "admin",
                "General",
                "sys-ops"
            },
            BlacklistedChannelNamePhrases = Array.Empty<string>(),
            MaxChannelNameLength = 64,
            MinChannelNameLength = 1
        };

        IPostConfigureOptions<ChannelServiceOptions> configurer = new ChannelServiceOptionsConfigurer();

        configurer.PostConfigure(null, options);

        options.ReservedChannelNames
               .Should()
               .ContainInOrder("sys-admin", "sys-General", "sys-ops");
    }
}