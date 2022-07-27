using Chaos.Templates.Abstractions;

namespace Chaos.Templates;

public class SkillTemplate : PanelObjectTemplateBase
{
    public bool IsAssail { get; init; }
    public override string TemplateKey { get; init; } = "PLACEHOLDER";
}