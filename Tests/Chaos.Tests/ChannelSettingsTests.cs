#region
using Chaos.DarkAges.Definitions;
using Chaos.Models.Data;
using FluentAssertions;
#endregion

namespace Chaos.Tests;

public sealed class ChannelSettingsTests
{
    #region Deconstruct
    [Test]
    public void Deconstruct_ShouldExtractAllProperties()
    {
        var settings = new ChannelSettings("General", true, MessageColor.Default);

        (var channelName, var customChannel, var messageColor) = settings;

        channelName.Should()
                   .Be("General");

        customChannel.Should()
                     .BeTrue();

        messageColor.Should()
                    .Be(MessageColor.Default);
    }
    #endregion

    #region GetHashCode
    [Test]
    public void GetHashCode_SameNameDifferentCase_ShouldBeEqual()
    {
        var settings1 = new ChannelSettings("General");
        var settings2 = new ChannelSettings("GENERAL");

        settings1.GetHashCode()
                 .Should()
                 .Be(settings2.GetHashCode());
    }
    #endregion

    #region Equals
    [Test]
    public void Equals_SameNameDifferentCase_ShouldBeTrue()
    {
        var settings1 = new ChannelSettings("General");
        var settings2 = new ChannelSettings("GENERAL");

        settings1.Equals(settings2)
                 .Should()
                 .BeTrue();
    }

    [Test]
    public void Equals_DifferentName_ShouldBeFalse()
    {
        var settings1 = new ChannelSettings("General");
        var settings2 = new ChannelSettings("Trade");

        settings1.Equals(settings2)
                 .Should()
                 .BeFalse();
    }

    [Test]
    public void Equals_Null_ShouldBeFalse()
    {
        var settings = new ChannelSettings("General");

        settings.Equals(null)
                .Should()
                .BeFalse();
    }
    #endregion
}