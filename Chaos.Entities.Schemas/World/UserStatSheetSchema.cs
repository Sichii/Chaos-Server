using Chaos.Common.Definitions;

namespace Chaos.Entities.Schemas.World;

public record UserStatSheetSchema : StatSheetSchema
{
    public AdvClass AdvClass { get; init; }
    public BaseClass BaseClass { get; init; }
    public uint ToNextAbility { get; init; }
    public uint ToNextLevel { get; init; }
    public uint TotalAbility { get; init; }
    public uint TotalExp { get; init; }
    public int UnspentPoints { get; init; }
}