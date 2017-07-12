namespace Chaos
{
    internal struct Animation
    {
        internal uint TargetId { get; set; }
        internal uint SourceId { get; set; }
        internal ushort TargetAnimation { get; set; }
        internal ushort SourceAnimation { get; set; }
        internal ushort AnimationSpeed { get; set; }

        internal Animation(uint targetId, uint sourceId, ushort targetAnimation, ushort sourceAnimation, ushort speed)
        {
            TargetId = targetId;
            SourceId = sourceId;
            TargetAnimation = targetAnimation;
            SourceAnimation = sourceAnimation;
            AnimationSpeed = speed;
        }
    }
}
