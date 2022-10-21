using System.Text.Json.Serialization;
using Chaos.Common.Definitions;

namespace Chaos.Schemas.Templates;

public sealed record SpellTemplateSchema : PanelObjectTemplateSchema
{
    /// <summary>
    ///     The number of chant lines this spell requires by default
    /// </summary>
    public byte CastLines { get; init; }
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