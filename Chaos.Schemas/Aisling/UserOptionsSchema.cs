namespace Chaos.Schemas.Aisling;

public sealed record UserOptionsSchema
{
    public required bool Exchange { get; init; }
    public required bool FastMove { get; init; }
    public required bool Group { get; init; }
    public required bool GuildChat { get; init; }
    public required bool Magic { get; init; }
    public required bool Shout { get; init; }
    public required bool Whisper { get; init; }
    public required bool Wisdom { get; init; }
}