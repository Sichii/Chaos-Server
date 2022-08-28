using Chaos.Data;
using Chaos.Entities.Schemas.World;
using Chaos.Services.Mappers.Interfaces;

namespace Chaos.Services.Mappers;

public class StatSheetTypeMapper : ITypeMapper<StatSheet, StatSheetSchema>
{
    public StatSheet Map(StatSheetSchema obj) => new(obj);

    public StatSheetSchema Map(StatSheet obj) => new()
    {
        Ability = obj.Ability,
        Ac = obj.Ac,
        Con = obj.Con,
        CurrentHp = obj.CurrentHp,
        CurrentMp = obj.CurrentMp,
        Level = obj.Level,
        Dex = obj.Dex,
        Dmg = obj.Dmg,
        Hit = obj.Hit,
        Int = obj.Int,
        MagicResistance = obj.MagicResistance,
        MaximumHp = obj.MaximumHp,
        MaximumMp = obj.MaximumMp,
        Str = obj.Str,
        Wis = obj.Wis
    };
}