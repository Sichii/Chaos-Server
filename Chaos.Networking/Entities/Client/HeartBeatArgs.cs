using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

public record HeartBeatArgs(byte First, byte Second) : IReceiveArgs;