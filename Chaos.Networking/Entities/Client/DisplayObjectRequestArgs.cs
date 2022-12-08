using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

[SuppressMessage("ReSharper", "NotAccessedPositionalProperty.Global", Justification = "We don't respond to the packet")]
public sealed record DisplayObjectRequestArgs(uint TargetId) : IReceiveArgs;