namespace Chaos.Entities.Schemas.World;

public record StatSheetSchema : AttributesSchema
{
    public int Ability { get; init; }
    public int CurrentHp { get; init; }
    public int CurrentMp { get; init; }
    public int Level { get; init; }
}