using Chaos.DarkAges.Definitions;
using Chaos.Networking.Abstractions.Definitions;

namespace Chaos.Networking.Entities.Server;

/// <summary>
///     Represents the serialization of a ground item in the <see cref="ServerOpCode.DisplayVisibleEntities" /> packet
/// </summary>
public sealed record GroundItemInfo : VisibleEntityInfo
{
    /// <summary>
    ///     The color of the ground item
    /// </summary>
    public DisplayColor Color { get; set; }
}