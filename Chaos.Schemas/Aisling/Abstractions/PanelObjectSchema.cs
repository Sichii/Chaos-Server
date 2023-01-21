using System.Text.Json.Serialization;

namespace Chaos.Schemas.Aisling.Abstractions;

public abstract record PanelObjectSchema
{
    public int? ElapsedMs { get; set; }
    public ICollection<string> ScriptKeys { get; set; } = Array.Empty<string>();
    public byte? Slot { get; set; }
    [JsonRequired]
    public string TemplateKey { get; set; } = null!;
    public ulong UniqueId { get; set; }
}