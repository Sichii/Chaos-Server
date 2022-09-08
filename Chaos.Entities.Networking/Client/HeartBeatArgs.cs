using Chaos.Packets.Abstractions;

namespace Chaos.Entities.Networking.Client;

public record HeartBeatArgs(byte First, byte Second) : IReceiveArgs;