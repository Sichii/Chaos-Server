using Chaos.Packets.Interfaces;

namespace Chaos.Networking.Model.Client;

public record ItemDroppedOnCreatureArgs(byte SourceSlot, uint TargetId, byte Count) : IReceiveArgs;