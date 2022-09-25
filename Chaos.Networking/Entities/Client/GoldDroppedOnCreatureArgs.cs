using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

public record GoldDroppedOnCreatureArgs(int Amount, uint TargetId) : IReceiveArgs;