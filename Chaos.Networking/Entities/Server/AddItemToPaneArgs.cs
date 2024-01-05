using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Server;

/// <summary>
///     Represents the serialization of the
///     <see cref="Chaos.Packets.Abstractions.Definitions.ServerOpCode.AddItemToPane" /> packet
/// </summary>
public sealed record AddItemToPaneArgs : IPacketSerializable
{
    /// <summary>
    ///     The info of the item to add to the client's inventory
    /// </summary>
    public ItemInfo Item { get; set; } = null!;
}