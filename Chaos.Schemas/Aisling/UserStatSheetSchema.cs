using Chaos.Common.Definitions;

namespace Chaos.Schemas.Aisling;

public sealed record UserStatSheetSchema : StatSheetSchema
{
    public required AdvClass AdvClass { get; init; }
    public required BaseClass BaseClass { get; init; }
    public required int MaxWeight { get; init; }
    public required uint ToNextAbility { get; init; }
    public required uint ToNextLevel { get; init; }
    public required uint TotalAbility { get; init; }
    public required uint TotalExp { get; init; }
    public required int UnspentPoints { get; init; }
}