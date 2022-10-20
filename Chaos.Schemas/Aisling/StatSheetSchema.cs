namespace Chaos.Schemas.Aisling;

public record StatSheetSchema : AttributesSchema
{
    public required int Ability { get; init; }
    public required int CurrentHp { get; set; }
    public required int CurrentMp { get; set; }
    public required int Level { get; init; }
}