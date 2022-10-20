using Chaos.Templates.Abstractions;

namespace Chaos.Templates;

public sealed class SkillTemplate : PanelObjectTemplateBase
{
    public required bool IsAssail { get; init; }
}