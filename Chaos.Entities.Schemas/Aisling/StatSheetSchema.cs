namespace Chaos.Entities.Schemas.Aisling;

public record StatSheetSchema : AttributesSchema
{
    public required int Ability { get; init; }
    public required int CurrentHp { get; init; }
    public required int CurrentMp { get; init; }
    public required int Level { get; init; }
}