using Chaos.Common.Definitions;
using Chaos.Geometry.Abstractions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Client;

/// <summary>
///     Represents the serialization of the <see cref="Chaos.Packets.Abstractions.Definitions.ClientOpCode.Click" /> packet
/// </summary>
public sealed record ClickArgs : IPacketSerializable
{
    /// <summary>
    ///     The type of click being performed
    /// </summary>
    public required ClickType ClickType { get; set; }

    /// <summary>
    ///     If specified, the id of the object being clicked on
    /// </summary>
    public uint? TargetId { get; set; }

    /// <summary>
    ///     If specified, the point being clicked on
    /// </summary>
    public IPoint? TargetPoint { get; set; }
}