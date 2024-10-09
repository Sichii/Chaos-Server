using Chaos.DarkAges.Definitions;
using Chaos.Networking.Abstractions.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Server;

/// <summary>
///     Represents the serialization of the <see cref="ServerOpCode.DisplayPublicMessage" /> packet
/// </summary>
public sealed record DisplayPublicMessageArgs : IPacketSerializable
{
    /// <summary>
    ///     The message to be displayed
    /// </summary>
    public string Message { get; set; } = null!;

    /// <summary>
    ///     The type of message
    /// </summary>
    public PublicMessageType PublicMessageType { get; set; }

    /// <summary>
    ///     The id of the source of the message
    /// </summary>
    public uint SourceId { get; set; }
}