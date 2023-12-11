using System.Text.Json.Serialization;

namespace Chaos.Schemas.Data;

/// <summary>
///     Represents the serializable schema of a prerequisite ability requirement
/// </summary>
public class AbilityRequirementSchema
{
    /// <summary>
    ///     If specified, the level of the prerequisite ability. If not specified, uses the max level of the prerequisite
    ///     ability
    /// </summary>
    public byte? Level { get; set; }

    /// <summary>
    ///     The key of the prerequisite ability
    /// </summary>
    [JsonRequired]
    public string TemplateKey { get; set; } = null!;
}