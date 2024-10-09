using Chaos.Geometry;
using Chaos.Networking.Abstractions.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Server;

/// <summary>
///     Represents the serialization of the <see cref="ServerOpCode.Animation" /> packet
/// </summary>
public sealed record AnimationArgs : IPacketSerializable
{
    /// <summary>
    ///     The speed of the animation
    /// </summary>
    public ushort AnimationSpeed { get; set; }

    /// <summary>
    ///     The animation to play on the object that is the source of the animation
    /// </summary>
    public ushort SourceAnimation { get; set; }

    /// <summary>
    ///     The id of the object that is the source of the animation
    /// </summary>
    public uint? SourceId { get; set; }

    /// <summary>
    ///     The animation to play on the object that is the target of the animation
    /// </summary>
    public ushort TargetAnimation { get; set; }

    /// <summary>
    ///     If the animation targets an object, this is the id of the object that is the target of the animation
    /// </summary>
    public uint? TargetId { get; set; }

    /// <summary>
    ///     If the animation targets a point, this is the point that animation will play on
    /// </summary>
    public Point? TargetPoint { get; set; }
}