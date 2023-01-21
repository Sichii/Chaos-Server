using System.Text.Json.Serialization;
using Chaos.Common.Definitions;

namespace Chaos.Schemas.Aisling;

public sealed record UserStatSheetSchema : StatSheetSchema
{
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public AdvClass AdvClass { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public BaseClass BaseClass { get; set; }
    public int MaxWeight { get; set; }
    public uint ToNextAbility { get; set; }
    public uint ToNextLevel { get; set; }
    public uint TotalAbility { get; set; }
    public uint TotalExp { get; set; }
    public int UnspentPoints { get; set; }
}