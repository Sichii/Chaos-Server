using System.Text.Json.Serialization;

namespace Chaos.Schemas.Aisling;

public sealed record UserOptionsSchema
{
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public bool Exchange { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public bool FastMove { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public bool Group { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public bool GuildChat { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public bool Magic { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public bool Shout { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public bool Whisper { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public bool Wisdom { get; set; }
}