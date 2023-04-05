using System.Text.Json.Serialization;

namespace Chaos.Schemas.Data;

/// <summary>
///     Represents the serializable schema of a dialog option
/// </summary>
public sealed record DialogOptionSchema
{
    /// <summary>
    ///     The key of the dialog this option leads to
    /// </summary>
    [JsonRequired]
    public string DialogKey { get; set; } = null!;
    /// <summary>
    ///     The text displayed for this option
    /// </summary>
    [JsonRequired]
    public string OptionText { get; set; } = null!;
}