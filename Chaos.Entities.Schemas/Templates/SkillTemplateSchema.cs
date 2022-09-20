namespace Chaos.Entities.Schemas.Templates;

public record SkillTemplateSchema : PanelObjectTemplateSchema
{
    /// <summary>
    ///     Whether or not the skill is an assail and should be used when spacebar is pressed<br/>Assail cooldowns are handled by AssailIntervalMs and AtkSpeedPct
    /// </summary>
    public required bool IsAssail { get; init; }
}