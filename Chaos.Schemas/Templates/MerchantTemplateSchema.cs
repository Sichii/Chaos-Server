using Chaos.Common.Definitions;
using Chaos.Geometry.Abstractions.Definitions;

namespace Chaos.Schemas.Templates;

public sealed record MerchantTemplateSchema
{
    public required string TemplateKey { get; init; }
    public required string Name { get; init; }
    public required ushort Sprite { get; init; }
    public required ICollection<string> ScriptKeys { get; init; } = Array.Empty<string>();
    public required string? DialogKey { get; init; }
    public required Direction Direction { get; init; }
}