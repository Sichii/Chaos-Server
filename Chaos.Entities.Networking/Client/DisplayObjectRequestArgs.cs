using Chaos.Packets.Abstractions;

namespace Chaos.Entities.Networking.Client;

public record DisplayObjectRequestArgs(uint TargetId) : IReceiveArgs;