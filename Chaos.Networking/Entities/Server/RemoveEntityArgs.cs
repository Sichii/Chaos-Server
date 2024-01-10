using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Entities.Server;

/// <summary>
///     Represents the serialization of the <see cref="ServerOpCode.RemoveEntity" /> packet
/// </summary>
public sealed record RemoveEntityArgs : IPacketSerializable
{
    /// <summary>
    ///     The id of the object to be removed
    /// </summary>
    public uint SourceId { get; set; }
}