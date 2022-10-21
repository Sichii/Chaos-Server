using System.Text.Json.Serialization;

namespace Chaos.Schemas.Aisling;

public sealed record UserOptionsSchema
{
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public bool Exchange { get; init; }
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public bool FastMove { get; init; }
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public bool Group { get; init; }
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public bool GuildChat { get; init; }
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public bool Magic { get; init; }
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public bool Shout { get; init; }
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public bool Whisper { get; init; }
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public bool Wisdom { get; init; }
}