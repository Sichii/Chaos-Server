using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

public sealed record GoldDroppedOnCreatureArgs(int Amount, uint TargetId) : IReceiveArgs;