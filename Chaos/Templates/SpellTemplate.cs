using Chaos.Common.Definitions;
using Chaos.Data;
using Chaos.Templates.Abstractions;

namespace Chaos.Templates;

public sealed class SpellTemplate : PanelObjectTemplateBase
{
    public required byte CastLines { get; init; }
    public required LearningRequirements? LearningRequirements { get; init; }
    public required string? Prompt { get; set; }
    public required SpellType SpellType { get; init; }
}