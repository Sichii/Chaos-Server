using Chaos.Networking.Abstractions.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

/// <summary>
///     Represents the serialization of the <see cref="ClientOpCode.SpellUse" /> packet
/// </summary>
public sealed record SpellUseArgs : IPacketSerializable
{
    /// <summary>
    ///     The rest of the packet data because the networking layer doesn't know what type of spell is being used, so it can't
    ///     determine if it should read a target id and point, or a prompt
    /// </summary>
    public required byte[] ArgsData { get; set; } = [];

    /// <summary>
    ///     The slot of the spell the client is trying to use
    /// </summary>
    public required byte SourceSlot { get; set; }
}