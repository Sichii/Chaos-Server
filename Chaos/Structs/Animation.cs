namespace Chaos
{
    internal struct Animation
    {
        internal uint TargetId { get; }
        internal uint SourceId { get; }
        internal ushort TargetAnimation { get; }
        internal ushort SourceAnimation { get; }
        internal ushort AnimationSpeed { get; }

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
