namespace Chaos.Schemas.Aisling;

public record StatSheetSchema : AttributesSchema
{
    public int Ability { get; set; }
    public int CurrentHp { get; set; }
    public int CurrentMp { get; set; }
    public int Level { get; set; }
}