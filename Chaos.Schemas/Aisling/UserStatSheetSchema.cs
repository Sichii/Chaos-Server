using System.Text.Json.Serialization;
using Chaos.Common.Definitions;

namespace Chaos.Schemas.Aisling;

/// <summary>
///     Represents the serializable schema of an aisling's stat sheet
/// </summary>
public sealed record UserStatSheetSchema : StatSheetSchema
{
    /// <summary>
    ///     The aisling's advanced class
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public AdvClass AdvClass { get; set; }
    /// <summary>
    ///     The aisling's base class
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public BaseClass BaseClass { get; set; }
    /// <summary>
    ///     The aisling's max weight
    /// </summary>
    public int MaxWeight { get; set; }
    /// <summary>
    ///     The amount of ability exp until the next ability
    /// </summary>
    public uint ToNextAbility { get; set; }
    /// <summary>
    ///     The amount of exp until the next level
    /// </summary>
    public uint ToNextLevel { get; set; }
    /// <summary>
    ///     The total amount of ability exp obtained
    /// </summary>
    public uint TotalAbility { get; set; }
    /// <summary>
    ///     The total amount of exp obtained
    /// </summary>
    public uint TotalExp { get; set; }
    /// <summary>
    ///     The amount of unspent stat points
    /// </summary>
    public int UnspentPoints { get; set; }
}