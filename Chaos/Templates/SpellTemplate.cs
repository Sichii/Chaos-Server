using Chaos.Core.Definitions;
using Chaos.Templates.Abstractions;

namespace Chaos.Templates;

public class SpellTemplate : PanelObjectTemplateBase
{
    public byte BaseCastLines { get; init; }
    public string? Prompt { get; set; }
    public SpellType SpellType { get; init; }
    public override string TemplateKey { get; init; } = "PLACEHOLDER";
}