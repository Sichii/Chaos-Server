using Chaos.Containers;

namespace Chaos.Objects.Serializable;

public record SerializableOptions
{
    public bool Exchange { get; init; }
    public bool FastMove { get; init; }
    public bool Group { get; init; }
    public bool GuildChat { get; init; }
    public bool Magic { get; init; }
    public bool Shout { get; init; }
    public bool Whisper { get; init; }
    public bool Wisdom { get; init; }

    #pragma warning disable CS8618
    //json constructor
    public SerializableOptions() { }
    #pragma warning restore CS8618
    
    public SerializableOptions(UserOptions userOptions)
    {
        Exchange = userOptions.Exchange;
        FastMove = userOptions.FastMove;
        Group = userOptions.FastMove;
        GuildChat = userOptions.GuildChat;
        Magic = userOptions.GuildChat;
        Shout = userOptions.Shout;
        Whisper = userOptions.Whisper;
        Wisdom = userOptions.Wisdom;
    }
}