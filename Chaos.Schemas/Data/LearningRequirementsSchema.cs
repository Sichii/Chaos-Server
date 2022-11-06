using Chaos.Common.Definitions;
using Chaos.Schemas.Aisling;

namespace Chaos.Schemas.Data;

public sealed record LearningRequirementsSchema
{
    /// <summary>
    ///     The items and their amounts required to learn this ability
    /// </summary>
    public ICollection<ItemRequirementSchema> ItemRequirements { get; init; } = Array.Empty<ItemRequirementSchema>();

    /// <summary>
    ///     The skills that must be learned before this ability can be learned
    /// </summary>
    public ICollection<string> PrerequisiteSkillTemplateKeys { get; init; } = Array.Empty<string>();

    /// <summary>
    ///     The spells that must be learned before this ability can be learned
    /// </summary>
    public ICollection<string> PrerequisiteSpellTemplateKeys { get; init; } = Array.Empty<string>();

    /// <summary>
    ///     The class required to learn this ability
    /// </summary>
    public BaseClass? RequiredClass { get; init; }

    /// <summary>
    ///     The amount of gold required to learn this ability
    /// </summary>
    public int? RequiredGold { get; init; }

    /// <summary>
    ///     The level required to learn this ability
    /// </summary>
    public int? RequiredLevel { get; init; }
    /// <summary>
    ///     The attributes required to learn this skill
    /// </summary>
    public StatsSchema? RequiredStats { get; init; } = null!;
}