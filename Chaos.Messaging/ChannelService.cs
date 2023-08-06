using System.Collections.Concurrent;
using System.Text;
using Chaos.Common.Definitions;
using Chaos.Extensions.Common;
using Chaos.IO.Memory;
using Chaos.Messaging.Abstractions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Chaos.Messaging;

/// <summary>
///     Represents a service that manages channels and their subscribers
/// </summary>
public sealed class ChannelService : IChannelService
{
    private readonly ConcurrentDictionary<string, ChannelDetails> Channels;
    private readonly ILogger Logger;
    private readonly ChannelServiceOptions Options;

    /// <summary>
    ///     Creates a new instance of <see cref="ChannelService" />
    /// </summary>
    public ChannelService(IOptions<ChannelServiceOptions> options, ILogger<ChannelService> logger)
    {
        Options = options.Value;
        Channels = new ConcurrentDictionary<string, ChannelDetails>(StringComparer.OrdinalIgnoreCase);
        Logger = logger;
    }

    /// <inheritdoc />
    public bool ContainsChannel(string channelName) => Channels.ContainsKey(channelName);

    /// <inheritdoc />
    public IEnumerable<IChannelSubscriber> GetSubscribers(string channelName)
    {
        if (!Channels.TryGetValue(channelName, out var channelDetails))
            return Enumerable.Empty<IChannelSubscriber>();

        return channelDetails.Subscribers.Values.Select(subDetails => subDetails.Subscriber);
    }

    /// <inheritdoc />
    public bool IsChannel(string str) => str.StartsWithI(Options.ChannelPrefix);

    /// <inheritdoc />
    public void JoinChannel(IChannelSubscriber subscriber, string channelName, bool bypassValidation = false)
    {
        if (!bypassValidation && Options.ReservedChannelNames.Any(channelName.ContainsI))
        {
            subscriber.SendMessage($"Channel {channelName} not found");

            return;
        }

        if (!Channels.TryGetValue(channelName, out var channelDetails))
        {
            subscriber.SendMessage($"Channel {channelName} not found");

            return;
        }

        if (channelDetails.AddSubscriber(subscriber))
        {
            Logger.LogDebug("{@SubscriberName} has joined channel {@ChannelName}", subscriber.Name, channelName);

            subscriber.SendMessage($"You have joined channel {channelDetails.ChannelNameOverride ?? channelName}");
        } else
            subscriber.SendMessage($"You are already in channel {channelDetails.ChannelNameOverride ?? channelName}");
    }

    /// <inheritdoc />
    public void LeaveChannel(IChannelSubscriber subscriber, string channelName)
    {
        if (!Channels.TryGetValue(channelName, out var channelDetails))
        {
            subscriber.SendMessage($"Channel {channelName} not found");

            return;
        }

        if (channelDetails.RemoveSubscriber(subscriber))
        {
            Logger.LogDebug("{@SubscriberName} has left channel {@ChannelName}", subscriber.Name, channelName);

            subscriber.SendMessage($"You have left channel {channelDetails.ChannelNameOverride ?? channelName}");
        } else
            subscriber.SendMessage($"You are not in channel {channelDetails.ChannelNameOverride ?? channelName}");
    }

    /// <inheritdoc />
    public void RegisterChannel(
        IChannelSubscriber? subscriber,
        string channelName,
        MessageColor defaultMessageColor,
        Action<IChannelSubscriber, string> sendMessageAction,
        bool bypassValidation = false,
        string? channelNameOverride = null
    )
    {
        if (!IsChannel(channelName))
            channelName = $"{Options.ChannelPrefix}{channelName}";

        if (!bypassValidation)
        {
            if (Options.BlacklistedChannelNamePhrases.Any(channelName.ContainsI))
            {
                subscriber?.SendMessage("Invalid channel name");

                return;
            }

            if (Options.ReservedChannelNames.Any(channelName.ContainsI))
            {
                subscriber?.SendMessage("Invalid channel name");

                return;
            }

            if (Channels.TryGetValue(channelName, out _))
            {
                subscriber?.SendMessage("Channel already exists");

                return;
            }

            if (Options.MaxChannelNameLength < channelName.Length)
            {
                subscriber?.SendMessage("Channel name is too long");

                return;
            }

            if (Options.MinChannelNameLength > channelName.Length)
            {
                subscriber?.SendMessage("Channel name is too short");

                return;
            }
        }

        Channels.TryAdd(channelName, new ChannelDetails(defaultMessageColor, sendMessageAction, channelNameOverride));
        Logger.LogInformation("Channel {@ChannelName} has been registered", channelName);

        if (subscriber is not null)
            JoinChannel(subscriber, channelName, bypassValidation);
    }

    /// <inheritdoc />
    public void SendMessage(IChannelSubscriber subscriber, string channelName, string message)
    {
        if (!Channels.TryGetValue(channelName, out var channelDetails))
        {
            subscriber.SendMessage($"Channel {channelName} not found");

            return;
        }

        if (!channelDetails.ContainsSubscriber(subscriber))
        {
            subscriber.SendMessage("You are not in that channel");

            return;
        }

        //we do things like this to avoid allocating a bunch of extra strings
        //the string values here that are not stored in other objects should be intern()'d
        //all other strings are just having their bytes copied into the span
        //the only allocation is the final string
        var spanWriter = new SpanWriter(Encoding.Default);
        //$"[{finalChannelName}] {senderName}: {message}";

        if (channelDetails.DefaultColor != MessageColor.Default)
        {
            spanWriter.WriteString("{=");
            spanWriter.WriteByte((byte)channelDetails.DefaultColor);
        }

        spanWriter.WriteString("[");
        spanWriter.WriteString(channelDetails.ChannelNameOverride ?? channelName);
        spanWriter.WriteString("] ");
        spanWriter.WriteString(subscriber.Name);
        spanWriter.WriteString(": ");
        spanWriter.WriteString(message);

        var buffer = spanWriter.ToSpan();

        if (buffer.Length > CONSTANTS.MAX_SERVER_MESSAGE_LENGTH)
            buffer = buffer[..CONSTANTS.MAX_SERVER_MESSAGE_LENGTH];

        var defaultMessage = Encoding.Default.GetString(buffer);

        Logger.LogInformation(
            "Subscriber {@SubscriberName} sent message {@Message} to channel {@ChannelName}",
            subscriber.Name,
            message,
            channelName);

        foreach (var subDetails in channelDetails.Subscribers.Values)
        {
            //don't log here, channels are a choice
            if (subDetails.Subscriber.IsIgnoring(subscriber.Name))
                continue;

            //if there's a message color override
            if (subDetails.MessageColorOverride.HasValue)
            {
                //set the 3rd byte to the override color
                buffer[2] = (byte)subDetails.MessageColorOverride;
                //create a new string from the buffer and send it
                channelDetails.SendMessageAction(subDetails.Subscriber, Encoding.Default.GetString(buffer));
            } else
                channelDetails.SendMessageAction(subDetails.Subscriber, defaultMessage);
        }
    }

    /// <inheritdoc />
    public void SetChannelColor(IChannelSubscriber subscriber, string channelName, MessageColor messageColor)
    {
        if (!Channels.TryGetValue(channelName, out var channelDetails))
            return;

        if (!channelDetails.TryGetSubscriber(subscriber.Name, out var subDetails))
        {
            subscriber.SendMessage($"You are not in channel {channelDetails.ChannelNameOverride ?? channelName}");

            return;
        }

        subDetails.MessageColorOverride = messageColor;
    }

    /// <inheritdoc />
    public bool UnregisterChannel(string channelName)
    {
        if (!Channels.TryGetValue(channelName, out var channel))
            return false;

        foreach (var subDetails in channel.Subscribers.Values)
            LeaveChannel(subDetails.Subscriber, channelName);

        Logger.LogInformation("Channel {@ChannelName} has been unregistered", channelName);

        return Channels.TryRemove(channelName, out _);
    }
}