using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

public record DisplayObjectRequestArgs(uint TargetId) : IReceiveArgs;