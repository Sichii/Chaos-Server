namespace Chaos.Entities.Schemas.World;

public record UserOptionsSchema
{
    public bool Exchange { get; init; }
    public bool FastMove { get; init; }
    public bool Group { get; init; }
    public bool GuildChat { get; init; }
    public bool Magic { get; init; }
    public bool Shout { get; init; }
    public bool Whisper { get; init; }
    public bool Wisdom { get; init; }
}