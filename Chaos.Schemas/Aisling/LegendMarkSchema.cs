using System.Text.Json.Serialization;
using Chaos.Common.Definitions;

namespace Chaos.Schemas.Aisling;

/// <summary>
///     Represents the serializable schema of a legend mark
/// </summary>
public sealed record LegendMarkSchema
{
    /// <summary>
    ///     The date this mark was added
    /// </summary>
    public DateTime Added { get; set; }

    /// <summary>
    ///     The color of this mark
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public MarkColor Color { get; set; }

    /// <summary>
    ///     The amount of this mark
    /// </summary>
    public int Count { get; set; }

    /// <summary>
    ///     The icon of this mark
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public MarkIcon Icon { get; set; }

    /// <summary>
    ///     The key of this mark
    /// </summary>
    [JsonRequired]
    public string Key { get; set; } = null!;

    /// <summary>
    ///     The text of this mark
    /// </summary>
    [JsonRequired]
    public string Text { get; set; } = null!;
}