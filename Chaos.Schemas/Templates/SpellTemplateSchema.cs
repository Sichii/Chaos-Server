using System.Text.Json.Serialization;
using Chaos.Common.Definitions;
using Chaos.Schemas.Data;

namespace Chaos.Schemas.Templates;

/// <summary>
///     This object is the intermediary between a serialized and deserialized SpellTemplate. It defines the form that this object
///     should be saved in, and loaded from.
/// </summary>
public sealed record SpellTemplateSchema : PanelObjectTemplateSchema
{
    /// <summary>
    ///     The number of chant lines this spell requires by default
    /// </summary>
    public byte CastLines { get; init; }

    /// <summary>
    ///     Default null<br />If set, these are the requirements for the spell to be learned
    /// </summary>
    /// <remarks>this is a test</remarks>
    public LearningRequirementsSchema? LearningRequirements { get; init; }
    /// <summary>
    ///     Defaults to null<br />Should be specified with a spell type of "Prompt", this is the prompt the spell will offer when cast
    /// </summary>
    public string? Prompt { get; init; }
    /// <summary>
    ///     The way the spell is cast by the player
    /// </summary>
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public SpellType SpellType { get; init; }
}