namespace Chaos.Entities.Schemas.World;

public record PanelObjectSchema
{
    public int? ElapsedMs { get; init; }
    public ICollection<string> ScriptKeys { get; init; } = Array.Empty<string>();
    public byte? Slot { get; init; }
    public string TemplateKey { get; init; } = null!;
    public ulong UniqueId { get; init; }
}