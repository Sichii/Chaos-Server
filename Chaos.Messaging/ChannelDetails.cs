using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using Chaos.Common.Definitions;
using Chaos.Messaging.Abstractions;

namespace Chaos.Messaging;

internal sealed class ChannelDetails
{
    internal string? ChannelNameOverride { get; set; }
    internal MessageColor DefaultColor { get; set; }
    internal ServerMessageType MessageType { get; set; }
    internal ConcurrentDictionary<string, SubscriberDetails> Subscribers { get; set; }

    internal ChannelDetails(MessageColor defaultColor, ServerMessageType messageType, string? channelNameOverride = null)
    {
        MessageType = messageType;
        DefaultColor = defaultColor;
        Subscribers = new ConcurrentDictionary<string, SubscriberDetails>(StringComparer.OrdinalIgnoreCase);
        ChannelNameOverride = channelNameOverride;
    }

    internal bool AddSubscriber(IChannelSubscriber subscriber, MessageColor? messageColorOverride = null) =>
        Subscribers.TryAdd(subscriber.Name, new SubscriberDetails(subscriber, messageColorOverride));

    internal bool ContainsSubscriber(IChannelSubscriber subscriber) => Subscribers.ContainsKey(subscriber.Name);

    internal bool RemoveSubscriber(IChannelSubscriber subscriber) => Subscribers.TryRemove(subscriber.Name, out _);

    internal bool TryGetSubscriber(string name, [MaybeNullWhen(false)] out SubscriberDetails details) =>
        Subscribers.TryGetValue(name, out details);
}