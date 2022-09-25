using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

public record ItemDroppedOnCreatureArgs(byte SourceSlot, uint TargetId, byte Count) : IReceiveArgs;