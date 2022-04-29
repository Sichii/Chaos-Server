using Chaos.Core.Geometry;
using Chaos.Packets.Interfaces;

namespace Chaos.Networking.Model.Client;

public record ClickArgs(uint? TargetId, Point? TargetPoint) : IReceiveArgs;