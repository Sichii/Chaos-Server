using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

public sealed record HeartBeatArgs(byte First, byte Second) : IReceiveArgs;