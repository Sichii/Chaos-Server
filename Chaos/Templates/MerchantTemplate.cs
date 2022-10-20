using Chaos.Geometry.Abstractions.Definitions;
using Chaos.Objects.Dialog;
using Chaos.Scripting.Abstractions;
using Chaos.Templates.Abstractions;

namespace Chaos.Templates;

public sealed record MerchantTemplate : ITemplate, IScripted
{
    /// <inheritdoc />
    public required string TemplateKey { get; init; }
    /// <inheritdoc />
    public required ISet<string> ScriptKeys { get; init; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    public required string Name { get; init; }
    public required ushort Sprite { get; init; }
    public required Dialog? Dialog { get; init; }
    public required Direction Direction { get; init; }
}