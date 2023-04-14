using Chaos.Geometry.Abstractions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

/// <summary>
///     Represents the serialization of the <see cref="Chaos.Packets.Abstractions.Definitions.ClientOpCode.Pickup" /> packet
/// </summary>
/// <param name="DestinationSlot">The slot the client is trying to pick up and item into</param>
/// <param name="SourcePoint">The point from which the client is trying to pick up an item from</param>
public sealed record PickupArgs(byte DestinationSlot, IPoint SourcePoint) : IReceiveArgs;