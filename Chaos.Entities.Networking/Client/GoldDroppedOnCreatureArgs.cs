using Chaos.Packets.Abstractions;

namespace Chaos.Entities.Networking.Client;

public record GoldDroppedOnCreatureArgs(int Amount, uint TargetId) : IReceiveArgs;