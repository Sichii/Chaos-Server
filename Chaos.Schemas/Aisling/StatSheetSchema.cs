namespace Chaos.Schemas.Aisling;

/// <summary>
///     Represents the serializable schema of a creature's stat sheet
/// </summary>
public record StatSheetSchema : AttributesSchema
{
    /// <summary>
    ///     The creature's current ability
    /// </summary>
    public int Ability { get; set; }
    /// <summary>
    ///     The creature's current hp
    /// </summary>
    public int CurrentHp { get; set; }
    /// <summary>
    ///     The creature's current mp
    /// </summary>
    public int CurrentMp { get; set; }
    /// <summary>
    ///     The creature's level
    /// </summary>
    public int Level { get; set; }
}