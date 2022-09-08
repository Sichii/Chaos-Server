namespace Chaos.Entities.Schemas.Templates;

public record SkillTemplateSchema : PanelObjectTemplateSchema
{
    public required bool IsAssail { get; init; }
}