namespace Chaos.Objects.Serializable;

public record SerializableOptions
{
    public bool Exchange { get; set; }
    public bool FastMove { get; set; }
    public bool Group { get; set; }
    public bool GuildChat { get; set; }
    public bool Magic { get; set; }
    public bool Shout { get; set; }
    public bool Whisper { get; set; }
    public bool Wisdom { get; set; }
}