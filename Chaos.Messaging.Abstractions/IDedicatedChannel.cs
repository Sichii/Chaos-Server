namespace Chaos.Messaging.Abstractions;

/// <summary>
///     Represents a channel that is dedicated to a specific purpose
/// </summary>
public interface IDedicatedChannel
{
    /// <summary>
    ///     The name of the channel
    /// </summary>
    string ChannelName { get; }

    /// <summary>
    ///     Joins the dedicated channel
    /// </summary>
    void JoinChannel(IChannelSubscriber subscriber);

    /// <summary>
    ///     Leaves the dedicated channel
    /// </summary>
    void LeaveChannel(IChannelSubscriber subscriber);

    /// <summary>
    ///     Sends a message to the channel
    /// </summary>
    /// <param name="from">The subscriber the message is from</param>
    /// <param name="message">The message to send</param>
    void SendMessage(IChannelSubscriber from, string message);
}