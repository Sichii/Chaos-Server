using Chaos.Data;
using Chaos.Entities.Schemas.World;
using Chaos.Services.Mappers.Interfaces;

namespace Chaos.Services.Mappers;

public class UserStatSheetTypeMapper : ITypeMapper<UserStatSheet, UserStatSheetSchema>
{
    public UserStatSheet Map(UserStatSheetSchema obj) => new(obj);

    public UserStatSheetSchema Map(UserStatSheet obj) => new()
    {
        Ability = obj.Ability,
        Ac = obj.Ac,
        AdvClass = obj.AdvClass,
        BaseClass = obj.BaseClass,
        Con = obj.Con,
        CurrentHp = obj.CurrentHp,
        CurrentMp = obj.CurrentMp,
        Level = obj.Level,
        ToNextAbility = obj.ToNextAbility,
        ToNextLevel = obj.ToNextLevel,
        TotalAbility = obj.TotalAbility,
        TotalExp = obj.TotalExp,
        UnspentPoints = obj.UnspentPoints,
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