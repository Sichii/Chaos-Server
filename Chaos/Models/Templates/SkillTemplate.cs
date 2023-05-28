using Chaos.Models.Data;
using Chaos.Models.Templates.Abstractions;

namespace Chaos.Models.Templates;

public sealed record SkillTemplate : PanelEntityTemplateBase
{
    public required bool IsAssail { get; init; }
    public required LearningRequirements? LearningRequirements { get; init; }
}