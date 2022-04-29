using Chaos.Packets.Interfaces;

namespace Chaos.Networking.Model.Client;

public record HeartBeatArgs(byte First, byte Second) : IReceiveArgs;