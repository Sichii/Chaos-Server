using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

/// <summary>
///     Represents the serialization of the <see cref="Chaos.Packets.Abstractions.Definitions.ClientOpCode.GoldDroppedOnCreature" /> packet
/// </summary>
/// <param name="Amount">The amount of gold the client is trying to drop on the creature</param>
/// <param name="TargetId">The id of the creature the client is trying to drop gold on</param>
public sealed record GoldDroppedOnCreatureArgs(int Amount, uint TargetId) : IReceiveArgs;