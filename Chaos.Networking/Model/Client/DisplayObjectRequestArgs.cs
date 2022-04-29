using Chaos.Packets.Interfaces;

namespace Chaos.Networking.Model.Client;

public record DisplayObjectRequestArgs(uint TargetId) : IReceiveArgs;