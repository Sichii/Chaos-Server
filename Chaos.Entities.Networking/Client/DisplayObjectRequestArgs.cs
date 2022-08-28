using Chaos.Packets.Interfaces;

namespace Chaos.Entities.Networking.Client;

public record DisplayObjectRequestArgs(uint TargetId) : IReceiveArgs;