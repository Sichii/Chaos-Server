using Chaos.DarkAges.Definitions;
using Chaos.DarkAges.Extensions;
using Chaos.Extensions.Common;
using Chaos.Messaging.Abstractions;
using Chaos.Testing.Infrastructure.Mocks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace Chaos.Messaging.Tests;

public sealed class ChannelServiceTests
{
    private readonly IChannelService ChannelService;
    private readonly IOptions<ChannelServiceOptions> ChannelServiceOptions;
    private readonly Mock<ILogger<ChannelService>> LoggerMock;
    private readonly Mock<IChannelSubscriber> SubscriberMock;

    public ChannelServiceTests()
    {
        ChannelServiceOptions = Microsoft.Extensions.Options.Options.Create(
            new ChannelServiceOptions
            {
                BlacklistedChannelNamePhrases = ["Blacklisted"],
                ChannelPrefix = "!",
                MaxChannelNameLength = 20,
                MinChannelNameLength = 3,
                ReservedChannelNames = ["!Reserved"]
            });

        LoggerMock = MockLogger.Create<ChannelService>();

        SubscriberMock = MockChannelSubscriber.Create(
            "TestSubscriber",
            mock => mock.Setup(s => s.IsIgnoring(It.IsAny<string>()))
                        .Returns(false));

        ChannelService = new ChannelService(ChannelServiceOptions, LoggerMock.Object);
    }

    [Fact]
    public void ContainsChannel_ShouldReturnFalseForNonExistentChannel()
        => ChannelService.ContainsChannel("!NonExistentChannel")
                         .Should()
                         .BeFalse();

    [Fact]
    public void GetSubscribers_ShouldReturnEmptyForNonExistentChannel()
    {
        var subscribers = ChannelService.GetSubscribers("!NonExistentChannel");

        subscribers.Should()
                   .BeEmpty();
    }

    [Fact]
    public void IsChannel_ShouldReturnFalseIfDoesNotStartWithChannelPrefix()
        => ChannelService.IsChannel("Test")
                         .Should()
                         .BeFalse();

    [Fact]
    public void IsChannel_ShouldReturnTrueIfStartsWithChannelPrefix()
        => ChannelService.IsChannel("!Test")
                         .Should()
                         .BeTrue();

    [Fact]
    public void JoinChannel_ReservedChannelWithoutBypassValidation_ShouldFail()
    {
        ChannelService.JoinChannel(SubscriberMock.Object, "!Reserved");

        SubscriberMock.Verify(s => s.SendMessage("Channel !Reserved not found"), Times.Once);
    }

    [Fact]
    public void JoinChannel_ShouldAddSubscriber()
    {
        ChannelService.RegisterChannel(
            null,
            "!Test",
            MessageColor.Default,
            (_, _) => { });

        ChannelService.JoinChannel(SubscriberMock.Object, "!Test");

        ChannelService.GetSubscribers("!Test")
                      .Contains(SubscriberMock.Object)
                      .Should()
                      .BeTrue();
    }

    [Fact]
    public void JoinChannel_ShouldFailForNonExistentChannel()
    {
        ChannelService.JoinChannel(SubscriberMock.Object, "!NonExistentChannel");

        SubscriberMock.Verify(s => s.SendMessage("Channel !NonExistentChannel not found"), Times.Once);
    }

    [Fact]
    public void JoinChannel_WhenSubscriberAlreadySubscribed_ShouldNotAddSubscriber()
    {
        ChannelService.RegisterChannel(
            null,
            "!Test",
            MessageColor.Default,
            (_, _) => { });

        ChannelService.JoinChannel(SubscriberMock.Object, "!Test");
        ChannelService.JoinChannel(SubscriberMock.Object, "!Test");

        ChannelService.GetSubscribers("!Test")
                      .Count(subscriber => subscriber.Equals(SubscriberMock.Object))
                      .Should()
                      .Be(1);
        SubscriberMock.Verify(s => s.SendMessage("You are already in channel !Test"), Times.Once);
    }

    [Fact]
    public void JoinChannel_WithBypassValidation_ShouldAddSubscriber()
    {
        ChannelService.RegisterChannel(
            null,
            "!Reserved",
            MessageColor.Default,
            (_, _) => { },
            true);

        ChannelService.RegisterChannel(
            null,
            "!Blacklisted",
            MessageColor.Default,
            (_, _) => { },
            true);

        ChannelService.JoinChannel(SubscriberMock.Object, "!Reserved", true);
        ChannelService.JoinChannel(SubscriberMock.Object, "!Blacklisted", true);

        ChannelService.GetSubscribers("!Reserved")
                      .Contains(SubscriberMock.Object)
                      .Should()
                      .BeTrue();

        ChannelService.GetSubscribers("!Blacklisted")
                      .Contains(SubscriberMock.Object)
                      .Should()
                      .BeTrue();
    }

    [Fact]
    public void LeaveChannel_ShouldFailForNonExistentChannel()
    {
        ChannelService.LeaveChannel(SubscriberMock.Object, "!NonExistentChannel");

        SubscriberMock.Verify(s => s.SendMessage("You are not in !NonExistentChannel"), Times.Once);
    }

    [Fact]
    public void LeaveChannel_ShouldRemoveSubscriber()
    {
        ChannelService.RegisterChannel(
            null,
            "!Test",
            MessageColor.Default,
            (_, _) => { });

        ChannelService.JoinChannel(SubscriberMock.Object, "!Test");
        ChannelService.LeaveChannel(SubscriberMock.Object, "!Test");

        ChannelService.GetSubscribers("!Test")
                      .Contains(SubscriberMock.Object)
                      .Should()
                      .BeFalse();
    }

    [Fact]
    public void LeaveChannel_WhenNotInChannel_ShouldNotRemoveSubscriber()
    {
        ChannelService.RegisterChannel(
            null,
            "!Test",
            MessageColor.Default,
            (_, _) => { });

        ChannelService.LeaveChannel(SubscriberMock.Object, "!Test");

        ChannelService.GetSubscribers("!Test")
                      .Contains(SubscriberMock.Object)
                      .Should()
                      .BeFalse();
        SubscriberMock.Verify(s => s.SendMessage("You are not in channel !Test"), Times.Once);
    }

    [Fact]
    public void RegisterChannel_ChannelNameTooLong_ShouldFail()
    {
        var nameTooLongStr = $"!{new string('a', ChannelServiceOptions.Value.MaxChannelNameLength + 1)}";

        ChannelService.RegisterChannel(
            SubscriberMock.Object,
            nameTooLongStr,
            MessageColor.Default,
            (_, _) => { });

        ChannelService.ContainsChannel(nameTooLongStr)
                      .Should()
                      .BeFalse();
        SubscriberMock.Verify(s => s.SendMessage("Channel name is too long"), Times.Once);
    }

    [Fact]
    public void RegisterChannel_ChannelNameTooShort_ShouldFail()
    {
        ChannelService.RegisterChannel(
            SubscriberMock.Object,
            "!a",
            MessageColor.Default,
            (_, _) => { });

        ChannelService.ContainsChannel("!a")
                      .Should()
                      .BeFalse();
        SubscriberMock.Verify(s => s.SendMessage("Channel name is too short"), Times.Once);
    }

    [Fact]
    public void RegisterChannel_ExistingChannel_ShouldFail()
    {
        ChannelService.RegisterChannel(
            SubscriberMock.Object,
            "!Test",
            MessageColor.Default,
            (_, _) => { });

        ChannelService.RegisterChannel(
            SubscriberMock.Object,
            "!Test",
            MessageColor.Default,
            (_, _) => { });

        SubscriberMock.Verify(s => s.SendMessage("Channel already exists"), Times.Once);
    }

    [Fact]
    public void RegisterChannel_ShouldAddChannel()
    {
        ChannelService.RegisterChannel(
            null,
            "!Test",
            MessageColor.Default,
            (_, _) => { });

        ChannelService.ContainsChannel("!Test")
                      .Should()
                      .BeTrue();
    }

    [Fact]
    public void RegisterChannel_WithBlacklistedName_ShouldFail()
    {
        ChannelService.RegisterChannel(
            SubscriberMock.Object,
            "!Blacklisted",
            MessageColor.Default,
            (_, _) => { });

        ChannelService.ContainsChannel("!Blacklisted")
                      .Should()
                      .BeFalse();
        SubscriberMock.Verify(s => s.SendMessage("Invalid channel name"), Times.Once);
    }

    [Fact]
    public void RegisterChannel_WithReservedName_ShouldFail()
    {
        ChannelService.RegisterChannel(
            SubscriberMock.Object,
            "!Reserved",
            MessageColor.Default,
            (_, _) => { });

        ChannelService.ContainsChannel("!Reserved")
                      .Should()
                      .BeFalse();

        SubscriberMock.Verify(s => s.SendMessage("Invalid channel name"), Times.Once);
    }

    [Fact]
    public void RegisterChannel_WithSubscriber_ShouldJoinChannel()
    {
        ChannelService.RegisterChannel(
            SubscriberMock.Object,
            "!Test",
            MessageColor.Default,
            (_, _) => { });

        ChannelService.GetSubscribers("!Test")
                      .Contains(SubscriberMock.Object)
                      .Should()
                      .BeTrue();
    }

    [Fact]
    public void SendMessage_ShouldFailForNonExistentChannel()
    {
        ChannelService.SendMessage(SubscriberMock.Object, "!NonExistentChannel", "Test message");

        SubscriberMock.Verify(s => s.SendMessage("Channel !NonExistentChannel not found"), Times.Once);
    }

    [Fact]
    public void SendMessage_ShouldFailIfSubscriberNotInChannel()
    {
        ChannelService.RegisterChannel(
            null,
            "!Test",
            MessageColor.Default,
            (_, _) => { });

        ChannelService.SendMessage(SubscriberMock.Object, "!Test", "Test message");

        SubscriberMock.Verify(s => s.SendMessage("You are not in that channel"), Times.Once);
    }

    [Fact]
    public void SendMessage_ShouldSendToSubscribers()
    {
        ChannelService.RegisterChannel(
            SubscriberMock.Object,
            "!Test",
            MessageColor.Default,
            (subscriber, s) => subscriber.SendMessage(s));

        ChannelService.SendMessage(SubscriberMock.Object, "!Test", "Hello, world!");

        SubscriberMock.Verify(
            s => s.SendMessage(It.Is<string>(str => str.ContainsI("Hello, world!") && str.ContainsI("Test"))),
            Times.AtLeastOnce);
    }

    [Fact]
    public void SendMessage_TruncateLongString_ShouldSendTruncatedMessage()
    {
        ChannelService.RegisterChannel(
            SubscriberMock.Object,
            "!Test",
            MessageColor.Default,
            (subscriber, s) => subscriber.SendMessage(s));

        var longMessage = new string('a', CONSTANTS.MAX_COMPLETE_MESSAGE_LENGTH + 1);
        var expectedTruncatedMessage = longMessage[..(CONSTANTS.MAX_COMPLETE_MESSAGE_LENGTH / 2)];

        ChannelService.SendMessage(SubscriberMock.Object, "!Test", longMessage);

        SubscriberMock.Verify(
            s => s.SendMessage(It.Is<string>(str => str.ContainsI("Test") && str.ContainsI(expectedTruncatedMessage))),
            Times.AtLeastOnce);
    }

    [Fact]
    public void SendMessage_WithColorOverride_ShouldSendToSubscribersWithColorOverride()
    {
        ChannelService.RegisterChannel(
            SubscriberMock.Object,
            "!Test",
            MessageColor.Red,
            (subscriber, s) => subscriber.SendMessage(s));

        ChannelService.SetChannelColor(SubscriberMock.Object, "!Test", MessageColor.Blue);

        ChannelService.SendMessage(SubscriberMock.Object, "!Test", "Hello, world!");

        SubscriberMock.Verify(
            s => s.SendMessage(
                It.Is<string>(
                    str => str.ContainsI(MessageColor.Blue.ToPrefix()) && str.ContainsI("Test") && str.ContainsI("Hello, world!"))),
            Times.AtLeastOnce);
    }

    [Fact]
    public void SendMessage_WithCustomColor_ShouldSendToSubscribersWithCustomColor()
    {
        ChannelService.RegisterChannel(
            SubscriberMock.Object,
            "!Test",
            MessageColor.Red,
            (subscriber, s) => subscriber.SendMessage(s));

        ChannelService.SendMessage(SubscriberMock.Object, "!Test", "Hello, world!");

        SubscriberMock.Verify(
            s => s.SendMessage(
                It.Is<string>(
                    str => str.ContainsI(MessageColor.Red.ToPrefix()) && str.ContainsI("Test") && str.ContainsI("Hello, world!"))),
            Times.AtLeastOnce);
    }

    [Fact]
    public void SetChannelColor_ShouldFailIfNotInChannel()
    {
        ChannelService.RegisterChannel(
            null,
            "!Test",
            MessageColor.Default,
            (_, _) => { });

        ChannelService.SetChannelColor(SubscriberMock.Object, "!Test", MessageColor.Default);

        SubscriberMock.Verify(s => s.SendMessage("You are not in channel !Test"), Times.Once);
    }

    [Fact]
    public void UnregisterChannel_ShouldFailForNonExistentChannel()
        => ChannelService.UnregisterChannel("!NonExistentChannel")
                         .Should()
                         .BeFalse();

    [Fact]
    public void UnregisterChannel_ShouldRemoveAllSubscribers()
    {
        ChannelService.RegisterChannel(
            SubscriberMock.Object,
            "!Test",
            MessageColor.Default,
            (_, _) => { });

        ChannelService.UnregisterChannel("!Test")
                      .Should()
                      .BeTrue();

        ChannelService.GetSubscribers("!Test")
                      .Should()
                      .BeEmpty();
    }

    [Fact]
    public void UnregisterChannel_ShouldRemoveChannel()
    {
        ChannelService.RegisterChannel(
            null,
            "!Test",
            MessageColor.Default,
            (_, _) => { });

        ChannelService.UnregisterChannel("!Test")
                      .Should()
                      .BeTrue();

        ChannelService.ContainsChannel("!Test")
                      .Should()
                      .BeFalse();
    }
}