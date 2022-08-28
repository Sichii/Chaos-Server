using Chaos.Packets.Interfaces;

namespace Chaos.Entities.Networking.Client;

public record ItemDroppedOnCreatureArgs(byte SourceSlot, uint TargetId, byte Count) : IReceiveArgs;