using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

/// <summary>
///     Represents the serialization of the
///     <see cref="Chaos.Packets.Abstractions.Definitions.ClientOpCode.ItemDroppedOnCreature" /> packet
///     <br />
/// </summary>
/// <param name="SourceSlot">The slot of the item the client is trying to drop on the creature</param>
/// <param name="TargetId">The id of the creature the client is trying to drop the item on</param>
/// <param name="Count">The amount of the item the client is trying to drop on the creature</param>
public sealed record ItemDroppedOnCreatureArgs(byte SourceSlot, uint TargetId, byte Count) : IReceiveArgs;