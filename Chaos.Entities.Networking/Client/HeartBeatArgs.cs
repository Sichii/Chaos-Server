using Chaos.Packets.Interfaces;

namespace Chaos.Entities.Networking.Client;

public record HeartBeatArgs(byte First, byte Second) : IReceiveArgs;