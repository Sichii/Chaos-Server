namespace Chaos.Entities.Schemas.Aisling;

public record PanelObjectSchema
{
    public required int? ElapsedMs { get; init; }
    public required ICollection<string> ScriptKeys { get; init; } = Array.Empty<string>();
    public required byte? Slot { get; init; }
    public required string TemplateKey { get; init; }
    public required ulong UniqueId { get; set; }
}