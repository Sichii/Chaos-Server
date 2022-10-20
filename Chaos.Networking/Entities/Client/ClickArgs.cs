using Chaos.Geometry.Abstractions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

public sealed record ClickArgs(uint? TargetId, IPoint? TargetPoint) : IReceiveArgs;