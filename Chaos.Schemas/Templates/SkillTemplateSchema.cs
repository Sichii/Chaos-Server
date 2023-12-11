using Chaos.Schemas.Data;
using Chaos.Schemas.Templates.Abstractions;

namespace Chaos.Schemas.Templates;

/// <summary>
///     Represents the serializable schema for a skill template
/// </summary>
public sealed record SkillTemplateSchema : PanelEntityTemplateSchema
{
    /// <summary>
    ///     Whether or not the skill is an assail and should be used when spacebar is pressed<br />Assail cooldowns are handled
    ///     by AssailIntervalMs
    ///     and AtkSpeedPct
    /// </summary>
    public bool IsAssail { get; set; }

    /// <summary>
    ///     Defaults to null<br />If set, these are the requirements for the skill to be learned<br />If null, the skill can't
    ///     be learned
    /// </summary>
    public LearningRequirementsSchema? LearningRequirements { get; set; }

    /// <summary>
    ///     Whether or not the spell is capable of leveling up. If false, the spell will start at level 100
    /// </summary>
    public bool LevelsUp { get; set; }

    /// <summary>
    ///     Defaults to null<br />If set, this is the maximum level the spell can be leveled up to
    /// </summary>
    public byte? MaxLevel { get; set; }
}