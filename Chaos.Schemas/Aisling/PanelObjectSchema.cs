using System.Text.Json.Serialization;

namespace Chaos.Schemas.Aisling;

public record PanelObjectSchema
{
    public int? ElapsedMs { get; init; }
    public ICollection<string> ScriptKeys { get; init; } = Array.Empty<string>();
    public byte? Slot { get; init; }
    [JsonRequired]
    public string TemplateKey { get; init; } = null!;
    public ulong UniqueId { get; set; }
}