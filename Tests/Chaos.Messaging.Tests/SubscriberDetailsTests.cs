using Chaos.DarkAges.Definitions;
using Chaos.Messaging.Abstractions;
using Chaos.Testing.Infrastructure.Mocks;
using FluentAssertions;
using Moq;
using Xunit;

namespace Chaos.Messaging.Tests;

public sealed class SubscriberDetailsTests
{
    private readonly Mock<IChannelSubscriber> MockChannelSubscriber1 = MockChannelSubscriber.Create();
    private readonly Mock<IChannelSubscriber> MockChannelSubscriber2 = MockChannelSubscriber.Create();

    [Fact]
    public void GetHashCode_ShouldBeBasedOnSubscriber()
    {
        var details1 = new SubscriberDetails(MockChannelSubscriber1.Object);
        var details2 = new SubscriberDetails(MockChannelSubscriber1.Object);

        details1.GetHashCode()
                .Should()
                .Be(details2.GetHashCode());
    }

    [Fact]
    public void GetHashCode_ShouldDifferForDifferentSubscribers()
    {
        var details1 = new SubscriberDetails(MockChannelSubscriber1.Object);
        var details2 = new SubscriberDetails(MockChannelSubscriber2.Object);

        details1.GetHashCode()
                .Should()
                .NotBe(details2.GetHashCode());
    }

    [Fact]
    public void SubscriberDetails_EqualityShouldBeBasedOnSubscriber()
    {
        var details1 = new SubscriberDetails(MockChannelSubscriber1.Object, MessageColor.Blue);
        var details2 = new SubscriberDetails(MockChannelSubscriber1.Object, MessageColor.Red);

        details1.Equals(details2)
                .Should()
                .BeTrue();
    }

    [Fact]
    public void SubscriberDetails_EqualityShouldFailForDifferentSubscribers()
    {
        var details1 = new SubscriberDetails(MockChannelSubscriber1.Object);
        var details2 = new SubscriberDetails(MockChannelSubscriber2.Object);

        details1.Equals(details2)
                .Should()
                .BeFalse();
    }

    [Fact]
    public void SubscriberDetails_EqualityShouldNotBeBasedOnColorOverride()
    {
        var details1 = new SubscriberDetails(MockChannelSubscriber1.Object, MessageColor.Blue);
        var details2 = new SubscriberDetails(MockChannelSubscriber1.Object, MessageColor.Red);

        details1.Equals(details2)
                .Should()
                .BeTrue();
    }

    [Fact]
    public void SubscriberDetails_ShouldNotBeEqualToNull()
        => new SubscriberDetails(MockChannelSubscriber1.Object).Equals(null)
                                                               .Should()
                                                               .BeFalse();

    [Fact]
    public void SubscriberDetails_ShouldStoreProvidedValues()
    {
        var details = new SubscriberDetails(MockChannelSubscriber1.Object, MessageColor.Blue);

        details.Subscriber
               .Should()
               .BeSameAs(MockChannelSubscriber1.Object);

        details.MessageColorOverride
               .Should()
               .Be(MessageColor.Blue);
    }
}