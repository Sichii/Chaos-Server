using Chaos.Packets.Interfaces;

namespace Chaos.Entities.Networking.Client;

public record GoldDroppedOnCreatureArgs(int Amount, uint TargetId) : IReceiveArgs;