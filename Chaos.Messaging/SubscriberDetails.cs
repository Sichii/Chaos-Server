using Chaos.DarkAges.Definitions;
using Chaos.Messaging.Abstractions;

namespace Chaos.Messaging;

internal sealed record SubscriberDetails
{
    internal MessageColor? MessageColorOverride { get; set; }
    internal IChannelSubscriber Subscriber { get; }

    internal SubscriberDetails(IChannelSubscriber subscriber, MessageColor? messageColorOverride = null)
    {
        Subscriber = subscriber;
        MessageColorOverride = messageColorOverride;
    }

    /// <inheritdoc />
    public bool Equals(SubscriberDetails? other) => other is not null && (Subscriber == other.Subscriber);

    public override int GetHashCode() => Subscriber.GetHashCode();
}