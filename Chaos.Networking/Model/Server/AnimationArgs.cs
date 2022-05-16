using Chaos.Packets.Interfaces;

namespace Chaos.Networking.Model.Server;

public record AnimationArgs : ISendArgs
{
    public ushort AnimationSpeed { get; set; }
    public ushort SourceAnimation { get; set; }
    public uint? SourceId { get; set; }
    public ushort TargetAnimation { get; set; }
    public uint? TargetId { get; set; }
    public Point? TargetPoint { get; set; }
}

/*
public record CreatureWalkSerializer : ServerPacketSerializerBase<CreatureWalkArgs : ISendArgs>
{
    public override void Serialize(ref SpanWriter writer, CreatureWalkArgs args)
    {
        
    }
}
*/