namespace Chaos
{
    internal struct Animation
    {
        internal int TargetId { get; }
        internal int SourceId { get; }
        internal ushort TargetAnimation { get; }
        internal ushort SourceAnimation { get; }
        internal ushort AnimationSpeed { get; }

        internal Animation(int targetId, int sourceId, ushort targetAnimation, ushort sourceAnimation, ushort speed)
        {
            TargetId = targetId;
            SourceId = sourceId;
            TargetAnimation = targetAnimation;
            SourceAnimation = sourceAnimation;
            AnimationSpeed = speed;
        }
    }
}
