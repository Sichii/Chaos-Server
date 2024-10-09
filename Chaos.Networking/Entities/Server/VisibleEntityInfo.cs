using Chaos.Networking.Abstractions.Definitions;

namespace Chaos.Networking.Entities.Server;

/// <summary>
///     Represents the serialization of a visible entity in the <see cref="ServerOpCode.DisplayVisibleEntities" /> packet
/// </summary>
public record VisibleEntityInfo : WorldEntityInfo
{
    /// <summary>
    ///     The sprite of the entity
    /// </summary>
    public ushort Sprite { get; set; }

    /// <summary>
    ///     The X coordinate of the entity
    /// </summary>
    public int X { get; set; }

    /// <summary>
    ///     The Y coordinate of the entity
    /// </summary>
    public int Y { get; set; }
}