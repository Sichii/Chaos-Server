using Chaos.Schemas.Data;

namespace Chaos.Schemas.Templates;

public sealed record SkillTemplateSchema : PanelObjectTemplateSchema
{
    /// <summary>
    ///     Whether or not the skill is an assail and should be used when spacebar is pressed<br />Assail cooldowns are handled by AssailIntervalMs
    ///     and AtkSpeedPct
    /// </summary>
    public bool IsAssail { get; init; }

    /// <summary>
    ///     Defaults to null<br />If set, these are the requirements for the skill to be learned
    /// </summary>
    public LearningRequirementsSchema? LearningRequirements { get; init; }
}