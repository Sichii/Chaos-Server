using Chaos.Packets.Abstractions;

namespace Chaos.Entities.Networking.Client;

public record ItemDroppedOnCreatureArgs(byte SourceSlot, uint TargetId, byte Count) : IReceiveArgs;