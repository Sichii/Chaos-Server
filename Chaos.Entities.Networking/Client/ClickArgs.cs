using Chaos.Geometry.Abstractions;
using Chaos.Packets.Abstractions;

namespace Chaos.Entities.Networking.Client;

public record ClickArgs(uint? TargetId, IPoint? TargetPoint) : IReceiveArgs;