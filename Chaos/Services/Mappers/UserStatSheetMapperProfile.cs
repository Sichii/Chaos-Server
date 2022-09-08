using Chaos.Data;
using Chaos.Entities.Schemas.Aisling;
using Chaos.Services.Mappers.Abstractions;

namespace Chaos.Services.Mappers;

public class UserStatSheetMapperProfile : IMapperProfile<UserStatSheet, UserStatSheetSchema>
{
    public UserStatSheet Map(UserStatSheetSchema obj) => new()
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
        ToNextAbility = obj.ToNextAbility,
        ToNextLevel = obj.ToNextLevel,
        TotalAbility = obj.TotalAbility,
        TotalExp = obj.TotalExp,
        UnspentPoints = obj.UnspentPoints,
        BaseClass = obj.BaseClass,
        AdvClass = obj.AdvClass,
        MaxWeight = obj.MaxWeight
    };

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
        Wis = obj.Wis,
        MaxWeight = obj.MaxWeight
    };
}