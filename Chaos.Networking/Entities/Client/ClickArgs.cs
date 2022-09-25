using Chaos.Geometry.Abstractions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

public record ClickArgs(uint? TargetId, IPoint? TargetPoint) : IReceiveArgs;