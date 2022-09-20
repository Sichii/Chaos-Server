using Chaos.Common.Definitions;

namespace Chaos.Entities.Schemas.Templates;

public record SpellTemplateSchema : PanelObjectTemplateSchema
{
    /// <summary>
    ///     The number of chant lines this spell requires by default
    /// </summary>
    public required byte CastLines { get; init; }
    /// <summary>
    ///     Defaults to null<br />Should be specified with a spell type of "Prompt", this is the prompt the spell will offer when cast
    /// </summary>
    public required string? Prompt { get; init; }
    /// <summary>
    ///     The way the spell is cast by the player
    /// </summary>
    public required SpellType SpellType { get; init; }
}