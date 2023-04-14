using Chaos.Common.Definitions;

namespace Chaos.Networking.Entities.Server;

/// <summary>
///     Represents the serialization of a ground item in the
///     <see cref="Chaos.Packets.Abstractions.Definitions.ServerOpCode.DisplayVisibleEntities" /> packet
/// </summary>
public sealed record GroundItemInfo : VisibleEntityInfo
{
    /// <summary>
    ///     The color of the ground item
    /// </summary>
    public DisplayColor Color { get; set; }
}