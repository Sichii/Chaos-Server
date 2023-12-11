using Chaos.Schemas.Aisling.Abstractions;

namespace Chaos.Schemas.Aisling;

/// <summary>
///     Represents the serializable schema of a spell
/// </summary>
public sealed record SpellSchema : PanelEntitySchema
{
    /// <summary>
    ///     If the spell can level up, this is the level of the spell
    /// </summary>
    public byte? Level { get; set; }

    /// <summary>
    ///     If the spell can level up and has a custom max level, this is the maximum level the spell can be leveled up to
    /// </summary>
    public byte? MaxLevel { get; set; }
}