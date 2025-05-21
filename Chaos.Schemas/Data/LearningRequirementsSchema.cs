namespace Chaos.Schemas.Data;

/// <summary>
///     Represents the serializable schema of the requirements of an ability
/// </summary>
public sealed record LearningRequirementsSchema
{
    /// <summary>
    ///     The items and their amounts required to learn this ability
    /// </summary>
    public ICollection<ItemRequirementSchema> ItemRequirements { get; set; } = [];

    /// <summary>
    ///     The skills that must be learned before this ability can be learned
    /// </summary>
    public ICollection<AbilityRequirementSchema> PrerequisiteSkills { get; set; } = [];

    /// <summary>
    ///     The spells that must be learned before this ability can be learned
    /// </summary>
    public ICollection<AbilityRequirementSchema> PrerequisiteSpells { get; set; } = [];

    /// <summary>
    ///     The amount of gold required to learn this ability
    /// </summary>
    public int? RequiredGold { get; set; }

    /// <summary>
    ///     The attributes required to learn this skill
    /// </summary>
    public StatsSchema? RequiredStats { get; set; }
}