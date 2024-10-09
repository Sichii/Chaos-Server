using Chaos.DarkAges.Definitions;
using Chaos.Geometry.Abstractions.Definitions;
using Chaos.Networking.Abstractions.Definitions;

namespace Chaos.Networking.Entities.Server;

/// <summary>
///     Represents the serialization of a creature in the the <see cref="ServerOpCode.DisplayVisibleEntities" /> packet
/// </summary>
public sealed record CreatureInfo : VisibleEntityInfo
{
    /// <summary>
    ///     The type of the creature
    /// </summary>
    public CreatureType CreatureType { get; set; }

    /// <summary>
    ///     The direction the creature is facing
    /// </summary>
    public Direction Direction { get; set; }

    /// <summary>
    ///     The name of the creature
    /// </summary>
    public string Name { get; set; } = null!;
}