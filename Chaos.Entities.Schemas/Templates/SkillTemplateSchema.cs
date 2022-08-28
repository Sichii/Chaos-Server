namespace Chaos.Entities.Schemas.Templates;

public record SkillTemplateSchema : PanelObjectTemplateSchema
{
    public bool IsAssail { get; init; }
}