using Chaos.Common.Definitions;

namespace Chaos.Entities.Schemas.Templates;

public record SpellTemplateSchema : PanelObjectTemplateSchema
{
    public required byte CastLines { get; init; }
    public required string? Prompt { get; init; }
    public required SpellType SpellType { get; init; }
}