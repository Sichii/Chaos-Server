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
    public void JoinChannel(IChannelSubscriber obj, string channelName)
    {
        if (!Channels.TryGetValue(channelName, out var channelDetails))
        {
            obj.SendServerMessage(ServerMessageType.ActiveMessage, $"Channel {channelName} not found");

            return;
        }

        if (channelDetails.AddSubscriber(obj))
        {
            Logger.LogDebug("{@Subscriber} has joined channel {@ChannelName}", obj, channelName);

            obj.SendServerMessage(
                ServerMessageType.ActiveMessage,
                $"You have joined channel {channelDetails.ChannelNameOverride ?? channelName}");
        } else
            obj.SendServerMessage(
                ServerMessageType.ActiveMessage,
                $"You are already in channel {channelDetails.ChannelNameOverride ?? channelName}");
    }

    /// <inheritdoc />
    public void LeaveChannel(IChannelSubscriber obj, string channelName)
    {
        if (!Channels.TryGetValue(channelName, out var channelDetails))
        {
            obj.SendServerMessage(ServerMessageType.ActiveMessage, $"Channel {channelName} not found");

            return;
        }

        if (channelDetails.RemoveSubscriber(obj))
        {
            Logger.LogDebug("{@Subscriber} has left channel {@ChannelName}", obj, channelName);

            obj.SendServerMessage(
                ServerMessageType.ActiveMessage,
                $"You have left channel {channelDetails.ChannelNameOverride ?? channelName}");
        } else
            obj.SendServerMessage(
                ServerMessageType.ActiveMessage,
                $"You are not in channel {channelDetails.ChannelNameOverride ?? channelName}");
    }

    /// <inheritdoc />
    public void RegisterChannel(
        IChannelSubscriber? obj,
        string channelName,
        MessageColor defaultMessageColor,
        bool bypassValidation = false,
        string? channelNameOverride = null,
        ServerMessageType messageTypeOverride = ServerMessageType.ActiveMessage
    )
    {
        if (!IsChannel(channelName))
            channelName = $"{Options.ChannelPrefix}{channelName}";

        if (!bypassValidation)
        {
            if (Options.BlacklistedChannelNamePhrases.Any(channelName.ContainsI))
                return;

            if (Options.ReservedChannelNames.ContainsI(channelName))
                return;

            if (Channels.TryGetValue(channelName, out _))
            {
                obj?.SendServerMessage(ServerMessageType.ActiveMessage, "Channel already exists");

                return;
            }

            if (Options.MaxChannelNameLength < channelName.Length)
            {
                obj?.SendServerMessage(ServerMessageType.ActiveMessage, "Channel name is too long");

                return;
            }

            if (Options.MinChannelNameLength > channelName.Length)
            {
                obj?.SendServerMessage(ServerMessageType.ActiveMessage, "Channel name is too short");

                return;
            }
        }

        Channels.TryAdd(channelName, new ChannelDetails(defaultMessageColor, messageTypeOverride, channelNameOverride));
        Logger.LogDebug("Channel {@ChannelName} has been registered", channelName);

        if (obj is not null)
            JoinChannel(obj, channelName);
    }

    /// <inheritdoc />
    public void SendMessage(IChannelSubscriber obj, string channelName, string message)
    {
        if (!Channels.TryGetValue(channelName, out var channelDetails))
        {
            obj.SendServerMessage(ServerMessageType.ActiveMessage, $"Channel {channelName} not found");

            return;
        }

        if (!channelDetails.ContainsSubscriber(obj))
        {
            obj.SendServerMessage(ServerMessageType.ActiveMessage, "You are not in that channel");

            return;
        }

        //we do things like this to avoid allocating a bunch of extra strings
        //the string values here that are not stored in other objects should be intern()'d
        //all other strings are just having their bytes copied into the span
        //the only allocation is the final string
        Span<byte> buffer = stackalloc byte[CONSTANTS.MAX_SERVER_MESSAGE_LENGTH];
        var spanWriter = new SpanWriter(Encoding.Default, ref buffer);
        //$"[{finalChannelName}] {senderName}: {message}";

        if (channelDetails.DefaultColor != MessageColor.Default)
        {
            spanWriter.WriteString("{=");
            spanWriter.WriteByte((byte)channelDetails.DefaultColor);
        }

        spanWriter.WriteString("[");
        spanWriter.WriteString(channelDetails.ChannelNameOverride ?? channelName);
        spanWriter.WriteString("] ");
        spanWriter.WriteString(obj.Name);
        spanWriter.WriteString(": ");
        spanWriter.WriteString(message);
        spanWriter.Flush();

        if (buffer.Length > CONSTANTS.MAX_SERVER_MESSAGE_LENGTH)
            buffer = buffer[..CONSTANTS.MAX_SERVER_MESSAGE_LENGTH];

        var defaultMessage = Encoding.Default.GetString(buffer);

        Logger.LogTrace(
            "{@Subscriber} sent message {@Message} to channel {@ChannelName}",
            obj,
            channelName,
            message);

        foreach (var subDetails in channelDetails.Subscribers.Values)
        {
            //don't log here, channels are a choice
            if (subDetails.Subscriber.IsIgnoring(obj.Name))
                continue;

            //if there's a message color override
            if (subDetails.MessageColorOverride.HasValue)
            {
                //set the 3rd byte to the override color
                buffer[2] = (byte)subDetails.MessageColorOverride;
                //create a new string from the buffer and send it
                subDetails.Subscriber.SendServerMessage(channelDetails.MessageType, Encoding.Default.GetString(buffer));
            } else
                subDetails.Subscriber.SendServerMessage(channelDetails.MessageType, defaultMessage);
        }
    }

    /// <inheritdoc />
    public void SetChannelColor(IChannelSubscriber obj, string channelName, MessageColor messageColor)
    {
        if (!Channels.TryGetValue(channelName, out var channelDetails))
            return;

        if (!channelDetails.TryGetSubscriber(obj.Name, out var subDetails))
        {
            obj.SendServerMessage(
                ServerMessageType.ActiveMessage,
                $"You are not in channel {channelDetails.ChannelNameOverride ?? channelName}");

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

        Logger.LogDebug("Channel {@ChannelName} has been unregistered", channelName);

        return Channels.TryRemove(channelName, out _);
    }
}