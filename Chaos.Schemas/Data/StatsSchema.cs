namespace Chaos.Schemas.Data;

/// <summary>
///     Represents the serializable schema of stats
/// </summary>
public record StatsSchema
{
    /// <summary>
    ///     Constitution
    /// </summary>
    public int Con { get; set; }

    /// <summary>
    ///     Dexterity
    /// </summary>
    public int Dex { get; set; }

    /// <summary>
    ///     Intelligence
    /// </summary>
    public int Int { get; set; }

    /// <summary>
    ///     Strength
    /// </summary>
    public int Str { get; set; }

    /// <summary>
    ///     Wisdom
    /// </summary>
    public int Wis { get; set; }
}