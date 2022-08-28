using Chaos.Geometry.Interfaces;
using Chaos.Packets.Interfaces;

namespace Chaos.Entities.Networking.Client;

public record ClickArgs(uint? TargetId, IPoint? TargetPoint) : IReceiveArgs;