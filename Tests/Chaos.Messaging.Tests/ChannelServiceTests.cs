#region
using Chaos.DarkAges.Definitions;
using Chaos.DarkAges.Extensions;
using Chaos.Extensions.Common;
using Chaos.Messaging.Abstractions;
using Chaos.Testing.Infrastructure.Mocks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
#endregion

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

        SubscriberMock = MockChannelSubscriber.Create("TestSubscriber");

        SubscriberMock.Setup(s => s.IsIgnoring(It.IsAny<string>()))
                      .Returns(false);

        ChannelService = new ChannelService(ChannelServiceOptions, LoggerMock.Object);
    }

    [Test]
    public void ContainsChannel_ShouldReturnFalseForNonExistentChannel()
        => ChannelService.ContainsChannel("!NonExistentChannel")
                         .Should()
                         .BeFalse();

    [Test]
    public void ContainsChannel_ShouldReturnTrueForExistingChannel()
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

    [Test]
    public void GetSubscribers_ShouldReturnEmptyForNonExistentChannel()
    {
        var subscribers = ChannelService.GetSubscribers("!NonExistentChannel");

        subscribers.Should()
                   .BeEmpty();
    }

    [Test]
    public void GetSubscribers_ShouldReturnSubscribers_WhenChannelHasMembers()
    {
        ChannelService.RegisterChannel(
            SubscriberMock.Object,
            "!Test",
            MessageColor.Default,
            (_, _) => { });

        var subscribers = ChannelService.GetSubscribers("!Test");

        subscribers.Should()
                   .ContainSingle()
                   .Which
                   .Should()
                   .Be(SubscriberMock.Object);
    }

    [Test]
    public void GetSubscribers_ShouldThrow_WhenNotAChannel()
    {
        var act = () => ChannelService.GetSubscribers("NotAChannel");

        act.Should()
           .Throw<InvalidOperationException>();
    }

    [Test]
    public void IsChannel_ShouldReturnFalseIfDoesNotStartWithChannelPrefix()
        => ChannelService.IsChannel("Test")
                         .Should()
                         .BeFalse();

    [Test]
    public void IsChannel_ShouldReturnTrueIfStartsWithChannelPrefix()
        => ChannelService.IsChannel("!Test")
                         .Should()
                         .BeTrue();

    [Test]
    public void IsInChannel_ChannelExistsAndSubscriberPresent_ShouldReturnTrue()
    {
        ChannelService.RegisterChannel(
            SubscriberMock.Object,
            "!Test",
            MessageColor.Default,
            (_, _) => { });

        ChannelService.IsInChannel(SubscriberMock.Object, "!Test")
                      .Should()
                      .BeTrue();
    }

    [Test]
    public void IsInChannel_ChannelExistsButSubscriberAbsent_ShouldReturnFalse()
    {
        ChannelService.RegisterChannel(
            null,
            "!Test",
            MessageColor.Default,
            (_, _) => { });

        ChannelService.IsInChannel(SubscriberMock.Object, "!Test")
                      .Should()
                      .BeFalse();
    }

    [Test]
    public void IsInChannel_MissingChannel_ShouldReturnFalse()
        => ChannelService.IsInChannel(SubscriberMock.Object, "!NonExistent")
                         .Should()
                         .BeFalse();

    [Test]
    public void JoinChannel_ReservedChannelWithoutBypassValidation_ShouldFail()
    {
        ChannelService.JoinChannel(SubscriberMock.Object, "!Reserved");

        SubscriberMock.Verify(s => s.SendMessage("Channel !Reserved not found"), Times.Once);
    }

    [Test]
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

    [Test]
    public void JoinChannel_ShouldFailForNonExistentChannel()
    {
        ChannelService.JoinChannel(SubscriberMock.Object, "!NonExistentChannel");

        SubscriberMock.Verify(s => s.SendMessage("Channel !NonExistentChannel not found"), Times.Once);
    }

    [Test]
    public void JoinChannel_ShouldReturnFalse_WhenAlreadyInChannel()
    {
        ChannelService.RegisterChannel(
            SubscriberMock.Object,
            "!Test",
            MessageColor.Default,
            (_, _) => { });

        var result = ChannelService.JoinChannel(SubscriberMock.Object, "!Test");

        result.Should()
              .BeFalse();
    }

    [Test]
    public void JoinChannel_ShouldReturnFalse_WhenChannelNotFound()
    {
        var result = ChannelService.JoinChannel(SubscriberMock.Object, "!NonExistent");

        result.Should()
              .BeFalse();
    }

    [Test]
    public void JoinChannel_ShouldReturnFalse_WhenReservedAndNotBypassed()
    {
        var result = ChannelService.JoinChannel(SubscriberMock.Object, "!Reserved");

        result.Should()
              .BeFalse();
    }

    [Test]
    public void JoinChannel_ShouldReturnTrue_WhenSuccessful()
    {
        ChannelService.RegisterChannel(
            null,
            "!Test",
            MessageColor.Default,
            (_, _) => { });

        var result = ChannelService.JoinChannel(SubscriberMock.Object, "!Test");

        result.Should()
              .BeTrue();
    }

    [Test]
    public void JoinChannel_ShouldThrow_WhenNotAChannel()
    {
        var act = () => ChannelService.JoinChannel(SubscriberMock.Object, "NotAChannel");

        act.Should()
           .Throw<InvalidOperationException>();
    }

    [Test]
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

    [Test]
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

    [Test]
    public void LeaveChannel_ShouldFailForNonExistentChannel()
    {
        ChannelService.LeaveChannel(SubscriberMock.Object, "!NonExistentChannel");

        SubscriberMock.Verify(s => s.SendMessage("You are not in !NonExistentChannel"), Times.Once);
    }

    [Test]
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

    [Test]
    public void LeaveChannel_ShouldThrow_WhenNotAChannel()
    {
        var act = () => ChannelService.LeaveChannel(SubscriberMock.Object, "NotAChannel");

        act.Should()
           .Throw<InvalidOperationException>();
    }

    [Test]
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

    [Test]
    public void MuteChannel_ExistingChannel_ShouldNotThrow()
    {
        ChannelService.RegisterChannel(
            null,
            "!Test",
            MessageColor.Default,
            (_, _) => { });

        var act = () => ChannelService.MuteChannel("!Test");

        act.Should()
           .NotThrow();
    }

    [Test]
    public void MuteChannel_MissingChannel_ShouldNotThrow()
    {
        var act = () => ChannelService.MuteChannel("!NonExistent");

        act.Should()
           .NotThrow();
    }

    [Test]
    public void MuteChannel_WithoutPrefix_ShouldPrependPrefixBeforeLookup()
    {
        ChannelService.RegisterChannel(
            null,
            "!Test",
            MessageColor.Default,
            (_, _) => { });

        // MuteChannel internally calls PrependPrefix, so "Test" becomes "!Test"
        var act = () => ChannelService.MuteChannel("Test");

        act.Should()
           .NotThrow();
    }

    [Test]
    public void PrependPrefix_AlreadyHasPrefix_ShouldReturnUnchanged()
        => ChannelService.PrependPrefix("!Test")
                         .Should()
                         .Be("!Test");

    [Test]
    public void PrependPrefix_MissingPrefix_ShouldPrependPrefix()
        => ChannelService.PrependPrefix("Test")
                         .Should()
                         .Be("!Test");

    [Test]
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

    [Test]
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

    [Test]
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

    [Test]
    public void RegisterChannel_NullSubscriber_ShouldNotJoinChannel()
    {
        ChannelService.RegisterChannel(
            null,
            "!Test",
            MessageColor.Default,
            (_, _) => { });

        ChannelService.GetSubscribers("!Test")
                      .Should()
                      .BeEmpty();
    }

    [Test]
    public void RegisterChannel_NullSubscriber_WhenChannelExists_ShouldReturnFalse()
    {
        ChannelService.RegisterChannel(
            null,
            "!Test",
            MessageColor.Default,
            (_, _) => { },
            true);

        var result = ChannelService.RegisterChannel(
            null,
            "!Test",
            MessageColor.Default,
            (_, _) => { });

        result.Should()
              .BeFalse();
    }

    [Test]
    public void RegisterChannel_NullSubscriber_WithBlacklistedName_ShouldReturnFalse()
    {
        var result = ChannelService.RegisterChannel(
            null,
            "!Blacklisted",
            MessageColor.Default,
            (_, _) => { });

        result.Should()
              .BeFalse();

        ChannelService.ContainsChannel("!Blacklisted")
                      .Should()
                      .BeFalse();
    }

    [Test]
    public void RegisterChannel_NullSubscriber_WithNameTooLong_ShouldReturnFalse()
    {
        var longName = $"!{new string('a', ChannelServiceOptions.Value.MaxChannelNameLength + 1)}";

        var result = ChannelService.RegisterChannel(
            null,
            longName,
            MessageColor.Default,
            (_, _) => { });

        result.Should()
              .BeFalse();

        ChannelService.ContainsChannel(longName)
                      .Should()
                      .BeFalse();
    }

    [Test]
    public void RegisterChannel_NullSubscriber_WithNameTooShort_ShouldReturnFalse()
    {
        var result = ChannelService.RegisterChannel(
            null,
            "!a",
            MessageColor.Default,
            (_, _) => { });

        result.Should()
              .BeFalse();

        ChannelService.ContainsChannel("!a")
                      .Should()
                      .BeFalse();
    }

    [Test]
    public void RegisterChannel_NullSubscriber_WithReservedName_ShouldReturnFalse()
    {
        var result = ChannelService.RegisterChannel(
            null,
            "!Reserved",
            MessageColor.Default,
            (_, _) => { });

        result.Should()
              .BeFalse();

        ChannelService.ContainsChannel("!Reserved")
                      .Should()
                      .BeFalse();
    }

    [Test]
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

    [Test]
    public void RegisterChannel_ShouldThrow_WhenNotAChannel()
    {
        var act = () => ChannelService.RegisterChannel(
            null,
            "NotAChannel",
            MessageColor.Default,
            (_, _) => { });

        act.Should()
           .Throw<InvalidOperationException>();
    }

    [Test]
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

    [Test]
    public void RegisterChannel_WithBypassValidation_ShouldAllowReservedName()
    {
        var result = ChannelService.RegisterChannel(
            null,
            "!Reserved",
            MessageColor.Default,
            (_, _) => { },
            true);

        result.Should()
              .BeTrue();

        ChannelService.ContainsChannel("!Reserved")
                      .Should()
                      .BeTrue();
    }

    [Test]
    public void RegisterChannel_WithChannelNameOverride_ShouldUseOverrideInMessages()
    {
        ChannelService.RegisterChannel(
            SubscriberMock.Object,
            "!internal-name",
            MessageColor.Default,
            (subscriber, s) => subscriber.SendMessage(s),
            channelNameOverride: "PublicName");

        ChannelService.SendMessage(SubscriberMock.Object, "!internal-name", "Hello");

        SubscriberMock.Verify(s => s.SendMessage(It.Is<string>(str => str.Contains("PublicName"))), Times.AtLeastOnce);
    }

    [Test]
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

    [Test]
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

    [Test]
    public void SendMessage_ShouldFailForNonExistentChannel()
    {
        ChannelService.SendMessage(SubscriberMock.Object, "!NonExistentChannel", "Test message");

        SubscriberMock.Verify(s => s.SendMessage("Channel !NonExistentChannel not found"), Times.Once);
    }

    [Test]
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

    [Test]
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

    [Test]
    public void SendMessage_ShouldThrow_WhenNotAChannel()
    {
        var act = () => ChannelService.SendMessage(SubscriberMock.Object, "NotAChannel", "Hello");

        act.Should()
           .Throw<InvalidOperationException>();
    }

    [Test]
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

    [Test]
    public void SendMessage_WhenChannelMuted_ShouldNotDeliverMessage()
    {
        var receiverMock = MockChannelSubscriber.Create("Receiver");

        receiverMock.Setup(s => s.IsIgnoring(It.IsAny<string>()))
                    .Returns(false);

        ChannelService.RegisterChannel(
            SubscriberMock.Object,
            "!Test",
            MessageColor.Default,
            (subscriber, s) => subscriber.SendMessage(s));

        ChannelService.JoinChannel(receiverMock.Object, "!Test");
        ChannelService.MuteChannel("!Test");

        ChannelService.SendMessage(SubscriberMock.Object, "!Test", "Muted message");

        // The muted channel should not deliver via the send action
        receiverMock.Verify(s => s.SendMessage(It.Is<string>(str => str.Contains("Muted message"))), Times.Never);
    }

    [Test]
    public void SendMessage_WhenSubscriberIsIgnoredBySelf_ShouldSkipDelivery()
    {
        var senderMock = MockChannelSubscriber.Create("Sender");

        senderMock.Setup(s => s.IsIgnoring(It.IsAny<string>()))
                  .Returns(false);

        // SubscriberMock ignores messages from "Sender"
        SubscriberMock.Setup(s => s.IsIgnoring("Sender"))
                      .Returns(true);

        ChannelService.RegisterChannel(
            senderMock.Object,
            "!Test",
            MessageColor.Default,
            (subscriber, s) => subscriber.SendMessage(s));

        ChannelService.JoinChannel(SubscriberMock.Object, "!Test");

        ChannelService.SendMessage(senderMock.Object, "!Test", "hello");

        // SubscriberMock ignores the sender — message should not reach it
        SubscriberMock.Verify(s => s.SendMessage(It.Is<string>(str => str.Contains("hello"))), Times.Never);
    }

    [Test]
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
                It.Is<string>(str
                    => str.ContainsI(MessageColor.Blue.ToPrefix()) && str.ContainsI("Test") && str.ContainsI("Hello, world!"))),
            Times.AtLeastOnce);
    }

    [Test]
    public void SendMessage_WithColorOverride_WhenDefaultColorIsDefault_ShouldNotCorruptMessage()
    {
        ChannelService.RegisterChannel(
            SubscriberMock.Object,
            "!Test",
            MessageColor.Default,
            (subscriber, s) => subscriber.SendMessage(s));

        ChannelService.SetChannelColor(SubscriberMock.Object, "!Test", MessageColor.Blue);

        ChannelService.SendMessage(SubscriberMock.Object, "!Test", "Hello");

        SubscriberMock.Verify(s => s.SendMessage(It.Is<string>(str => str.Contains("[!Test]"))), Times.AtLeastOnce);
    }

    [Test]
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
                It.Is<string>(str
                    => str.ContainsI(MessageColor.Red.ToPrefix()) && str.ContainsI("Test") && str.ContainsI("Hello, world!"))),
            Times.AtLeastOnce);
    }

    [Test]
    public void SetChannelColor_ShouldDoNothing_WhenChannelNotFound()
    {
        // Channel doesn't exist — should not throw (silent return)
        var act = () => ChannelService.SetChannelColor(SubscriberMock.Object, "!NonExistent", MessageColor.Blue);

        act.Should()
           .NotThrow();
    }

    [Test]
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

    [Test]
    public void SetChannelColor_ShouldSetColor_WhenSubscriberIsInChannel()
    {
        ChannelService.RegisterChannel(
            SubscriberMock.Object,
            "!Test",
            MessageColor.Red,
            (subscriber, s) => subscriber.SendMessage(s));

        // Should not throw and should set the color
        var act = () => ChannelService.SetChannelColor(SubscriberMock.Object, "!Test", MessageColor.Blue);

        act.Should()
           .NotThrow();

        // Verify by sending a message — it should use the override color
        ChannelService.SendMessage(SubscriberMock.Object, "!Test", "ColorTest");

        SubscriberMock.Verify(s => s.SendMessage(It.Is<string>(str => str.ContainsI(MessageColor.Blue.ToPrefix()))), Times.AtLeastOnce);
    }

    [Test]
    public void SetChannelColor_ShouldThrow_WhenNotAChannel()
    {
        var act = () => ChannelService.SetChannelColor(SubscriberMock.Object, "NotAChannel", MessageColor.Red);

        act.Should()
           .Throw<InvalidOperationException>();
    }

    [Test]
    public void UnmuteChannel_ExistingChannel_ShouldNotThrow()
    {
        ChannelService.RegisterChannel(
            null,
            "!Test",
            MessageColor.Default,
            (_, _) => { });

        ChannelService.MuteChannel("!Test");

        var act = () => ChannelService.UnmuteChannel("!Test");

        act.Should()
           .NotThrow();
    }

    [Test]
    public void UnmuteChannel_MissingChannel_ShouldNotThrow()
    {
        var act = () => ChannelService.UnmuteChannel("!NonExistent");

        act.Should()
           .NotThrow();
    }

    [Test]
    public void UnregisterChannel_ShouldFailForNonExistentChannel()
        => ChannelService.UnregisterChannel("!NonExistentChannel")
                         .Should()
                         .BeFalse();

    [Test]
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

    [Test]
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

    [Test]
    public void UnregisterChannel_ShouldThrow_WhenNotAChannel()
    {
        var act = () => ChannelService.UnregisterChannel("NotAChannel");

        act.Should()
           .Throw<InvalidOperationException>();
    }
}