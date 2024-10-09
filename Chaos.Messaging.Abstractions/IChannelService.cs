using Chaos.DarkAges.Definitions;

namespace Chaos.Messaging.Abstractions;

/// <summary>
///     Defines the contract for an object that provides channel-based messaging services
/// </summary>
public interface IChannelService
{
    /// <summary>
    ///     Determines whether or not the given channel exists
    /// </summary>
    /// <param name="channelName">
    ///     The channel name to check
    /// </param>
    /// <returns>
    ///     <c>
    ///         true
    ///     </c>
    ///     if a channel with the given name exists, otherwise
    ///     <c>
    ///         false
    ///     </c>
    /// </returns>
    bool ContainsChannel(string channelName);

    /// <summary>
    ///     Gets all members of a channel
    /// </summary>
    /// <param name="channelName">
    /// </param>
    IEnumerable<IChannelSubscriber> GetSubscribers(string channelName);

    /// <summary>
    ///     Determines whether or not the given string is a valid channel name
    /// </summary>
    /// <param name="str">
    ///     The string to check
    /// </param>
    /// <returns>
    ///     <c>
    ///         true
    ///     </c>
    ///     if the given string is in the correct format to be a channel name, otherwise
    ///     <c>
    ///         false
    ///     </c>
    /// </returns>
    bool IsChannel(string str);

    /// <summary>
    ///     Determines whether or not the subscriber is in the given channel
    /// </summary>
    /// <param name="subscriber">
    ///     The subscriber to check for
    /// </param>
    /// <param name="channelName">
    ///     The channel to check if the subscriber is in
    /// </param>
    /// <returns>
    ///     <c>
    ///         true
    ///     </c>
    ///     if the given subscriber is in the specified channel, otherwise
    ///     <c>
    ///         false
    ///     </c>
    /// </returns>
    bool IsInChannel(IChannelSubscriber subscriber, string channelName);

    /// <summary>
    ///     Joins a channel, allowing you to send and receive messages to/from it
    /// </summary>
    /// <param name="subscriber">
    ///     The object being added to the channel
    /// </param>
    /// <param name="channelName">
    ///     The name of the channel
    /// </param>
    /// <param name="bypassValidation">
    ///     Whether or not to bypass validation of the channel name
    /// </param>
    bool JoinChannel(IChannelSubscriber subscriber, string channelName, bool bypassValidation = false);

    /// <summary>
    ///     Leaves a channel, preventing you from sending and receiving messages to/from it
    /// </summary>
    /// <param name="subscriber">
    ///     The object leaving the channel
    /// </param>
    /// <param name="channelName">
    ///     The name of the channel
    /// </param>
    void LeaveChannel(IChannelSubscriber subscriber, string channelName);

    /// <summary>
    ///     Prepends the channel prefix to the string if it is not already present
    /// </summary>
    string PrependPrefix(string channelName);

    /// <summary>
    ///     Registers a channel, allowing you to send and receive messages to/from it
    /// </summary>
    /// <param name="subscriber">
    ///     If a subscriber registered the channel, this will be that subscriber
    /// </param>
    /// <param name="channelName">
    ///     The name of the channel
    /// </param>
    /// <param name="defaultMessageColor">
    ///     The color to use for the given channel
    /// </param>
    /// <param name="sendMessageAction">
    ///     The action used to send a message to a subscriber on this channel
    /// </param>
    /// <param name="bypassValidation">
    ///     Whether or not to bypass the phrase filter. Use this to add channels with names that you have added to the phrase
    ///     blacklist for reservation purposes
    /// </param>
    /// <param name="channelNameOverride">
    ///     The name that will be displayed when sending and receiving from this channel (INTERNAL ONLY)
    /// </param>
    bool RegisterChannel(
        IChannelSubscriber? subscriber,
        string channelName,
        MessageColor defaultMessageColor,
        Action<IChannelSubscriber, string> sendMessageAction,
        bool bypassValidation = false,
        string? channelNameOverride = null);

    /// <summary>
    ///     Sends a message to a channel
    /// </summary>
    /// <param name="subscriber">
    ///     The obj sending the message
    /// </param>
    /// <param name="channelName">
    ///     The name of the channel to send to
    /// </param>
    /// <param name="message">
    ///     The message to send to the channel
    /// </param>
    void SendMessage(IChannelSubscriber subscriber, string channelName, string message);

    /// <summary>
    ///     Sets the text color of a channel
    /// </summary>
    /// <param name="subscriber">
    ///     The subscriber to set the channel color for
    /// </param>
    /// <param name="channelName">
    ///     The name of the channel
    /// </param>
    /// <param name="messageColor">
    ///     The color to use for the given channel
    /// </param>
    void SetChannelColor(IChannelSubscriber subscriber, string channelName, MessageColor messageColor);

    /// <summary>
    ///     Unregisters a channel, preventing anyone from sending and receiving messages to/from it
    /// </summary>
    /// <param name="channelName">
    ///     The name of the channel
    /// </param>
    bool UnregisterChannel(string channelName);
}