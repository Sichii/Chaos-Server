using Chaos.Common.Definitions;
using Chaos.Templates.Abstractions;

namespace Chaos.Templates;

public class SpellTemplate : PanelObjectTemplateBase
{
    public required byte CastLines { get; init; }
    public required string? Prompt { get; set; }
    public required SpellType SpellType { get; init; }
}