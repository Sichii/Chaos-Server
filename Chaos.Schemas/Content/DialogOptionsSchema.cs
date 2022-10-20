namespace Chaos.Schemas.Content;

public sealed record DialogOptionSchema
{
    public required string OptionText { get; init; }
    public required string DialogKey { get; init; }
}