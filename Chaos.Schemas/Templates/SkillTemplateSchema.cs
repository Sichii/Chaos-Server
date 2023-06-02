using Chaos.Schemas.Data;
using Chaos.Schemas.Templates.Abstractions;

namespace Chaos.Schemas.Templates;

/// <summary>
///     Represents the serializable schema for a skill template
/// </summary>
public sealed record SkillTemplateSchema : PanelEntityTemplateSchema
{
    /// <summary>
    ///     Whether or not the skill is an assail and should be used when spacebar is pressed<br />Assail cooldowns are handled by AssailIntervalMs
    ///     and AtkSpeedPct
    /// </summary>
    public bool IsAssail { get; set; }

    /// <summary>
    ///     Defaults to null<br />If set, these are the requirements for the skill to be learned<br />If null, the skill can't be learned
    /// </summary>
    public LearningRequirementsSchema? LearningRequirements { get; set; }
}