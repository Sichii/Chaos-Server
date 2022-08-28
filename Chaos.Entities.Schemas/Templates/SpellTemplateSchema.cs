using Chaos.Common.Definitions;

namespace Chaos.Entities.Schemas.Templates;

public record SpellTemplateSchema : PanelObjectTemplateSchema
{
    public byte CastLines { get; init; }
    public string? Prompt { get; init; }
    public SpellType SpellType { get; init; }
}