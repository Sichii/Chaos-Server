using System.Text.Json.Serialization;
using Chaos.Common.Definitions;

namespace Chaos.Schemas.Aisling;

public sealed record UserStatSheetSchema : StatSheetSchema
{
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public AdvClass AdvClass { get; init; }
    [JsonIgnore(Condition = JsonIgnoreCondition.Never)]
    public BaseClass BaseClass { get; init; }
    public int MaxWeight { get; init; }
    public uint ToNextAbility { get; init; }
    public uint ToNextLevel { get; init; }
    public uint TotalAbility { get; init; }
    public uint TotalExp { get; init; }
    public int UnspentPoints { get; init; }
}