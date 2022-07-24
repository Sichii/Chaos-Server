using Chaos.Geometry.Interfaces;
using Chaos.Packets.Interfaces;

namespace Chaos.Networking.Model.Client;

public record ClickArgs(uint? TargetId, IPoint? TargetPoint) : IReceiveArgs;