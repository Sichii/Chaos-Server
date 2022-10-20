using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

public sealed record DisplayObjectRequestArgs(uint TargetId) : IReceiveArgs;