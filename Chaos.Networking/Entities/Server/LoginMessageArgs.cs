using Chaos.DarkAges.Definitions;
using Chaos.Networking.Abstractions.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Server;

/// <summary>
///     Represents the serialization of the <see cref="ServerOpCode.LoginMessage" /> packet
/// </summary>
public sealed record LoginMessageArgs : IPacketSerializable
{
    /// <summary>
    ///     The type of login message to be used
    /// </summary>
    public LoginMessageType LoginMessageType { get; set; }

    /// <summary>
    ///     If the login message type can have a custom message, this will be the message displayed.
    /// </summary>
    public string? Message { get; set; }
}