using Chaos.Geometry.Abstractions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

/// <summary>
///     Represents the serialization of the <see cref="Chaos.Packets.Abstractions.Definitions.ClientOpCode.ItemDrop" /> packet
/// </summary>
/// <param name="SourceSlot">The slot of the item the client is trying to drop</param>
/// <param name="DestinationPoint">The point the client is trying to drop the item on</param>
/// <param name="Count">The amount of the item the client is trying to drop</param>
public sealed record ItemDropArgs(byte SourceSlot, IPoint DestinationPoint, int Count) : IReceiveArgs;