using Chaos.Common.Definitions;
using Chaos.Templates.Abstractions;

namespace Chaos.Templates;

public class SpellTemplate : PanelObjectTemplateBase
{
    public byte CastLines { get; init; }
    public string? Prompt { get; set; }
    public SpellType SpellType { get; init; }
}