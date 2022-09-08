using Chaos.Entities.Schemas.Data;

namespace Chaos.Entities.Schemas.Templates;

public record PanelObjectTemplateSchema
{
    public required AnimationSchema? Animation { get; init; }
    public required int? CooldownMs { get; init; }
    public required string Name { get; init; }
    public required ushort PanelSprite { get; init; }
    public required ICollection<string> ScriptKeys { get; init; } = Array.Empty<string>();
    public required string TemplateKey { get; init; }
}