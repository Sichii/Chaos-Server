using Chaos.Geometry.Abstractions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Server;

public record AnimationArgs : ISendArgs
{
    public ushort AnimationSpeed { get; set; }
    public ushort SourceAnimation { get; set; }
    public uint? SourceId { get; set; }
    public ushort TargetAnimation { get; set; }
    public uint? TargetId { get; set; }
    public IPoint? TargetPoint { get; set; }
}