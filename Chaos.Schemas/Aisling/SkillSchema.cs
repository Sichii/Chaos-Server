using Chaos.Schemas.Aisling.Abstractions;

namespace Chaos.Schemas.Aisling;

/// <summary>
///     Represents the serializable schema of a skill
/// </summary>
public sealed record SkillSchema : PanelEntitySchema
{
    /// <summary>
    ///     If the skill can level up, this is the level of the skill
    /// </summary>
    public byte? Level { get; set; }

    /// <summary>
    ///     If the skill can level up and has a custom max level, this is the maximum level the skill can be leveled up to
    /// </summary>
    public byte? MaxLevel { get; set; }
}