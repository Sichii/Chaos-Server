using Chaos.Packets.Interfaces;

namespace Chaos.Networking.Model.Client;

public record GoldDroppedOnCreatureArgs(int Amount, uint TargetId) : IReceiveArgs;