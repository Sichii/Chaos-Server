namespace Chaos.Messaging.Abstractions;

/// <summary>
///     Defines the contract used for an object that subscribes to a channel
/// </summary>
public interface IChannelSubscriber
{
    /// <summary>
    ///     The name of the subscriber
    /// </summary>
    string Name { get; }

    /// <summary>
    ///     Determines whether or not the subscriber is ignoring the given name
    /// </summary>
    /// <returns><c>true</c> if message from the given name are being ignored, otherwise <c>false</c></returns>
    bool IsIgnoring(string name);

    /// <summary>
    ///     Sends a message to the subscriber
    /// </summary>
    /// <param name="message">The message being send</param>
    void SendMessage(string message);
}