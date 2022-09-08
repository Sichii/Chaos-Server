using Chaos.Data;
using Chaos.Entities.Schemas.Aisling;
using Chaos.Services.Mappers.Abstractions;

namespace Chaos.Services.Mappers;

public class StatSheetMapperProfile : IMapperProfile<StatSheet, StatSheetSchema>
{
    public StatSheet Map(StatSheetSchema obj) => new()
    {
        Ac = obj.Ac,
        Dmg = obj.Dmg,
        Hit = obj.Hit,
        Str = obj.Str,
        Int = obj.Int,
        Wis = obj.Wis,
        Con = obj.Con,
        Dex = obj.Dex,
        MagicResistance = obj.MagicResistance,
        MaximumHp = obj.MaximumHp,
        MaximumMp = obj.MaximumMp,
        CurrentHp = obj.CurrentHp,
        CurrentMp = obj.CurrentMp,
        Ability = obj.Ability,
        Level = obj.Level,
    };

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