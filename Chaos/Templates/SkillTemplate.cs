using Chaos.Data;
using Chaos.Templates.Abstractions;

namespace Chaos.Templates;

public sealed class SkillTemplate : PanelObjectTemplateBase
{
    public required bool IsAssail { get; init; }
    public required LearningRequirements? LearningRequirements { get; init; }
}