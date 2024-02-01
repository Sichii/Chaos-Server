namespace Chaos.Packets.Abstractions;

/// <summary>
///     Represents a packet that changes the sequence number
/// </summary>
public interface ISequencerPacket : IPacketSerializable
{
    /// <summary>
    ///     The new sequence number (the sequence of the packet that sent this)
    /// </summary>
    byte Sequence { get; set; }
}