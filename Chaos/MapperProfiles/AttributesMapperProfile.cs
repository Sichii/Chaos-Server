using Chaos.Data;
using Chaos.Schemas.Aisling;
using Chaos.TypeMapper.Abstractions;

namespace Chaos.MapperProfiles;

public sealed class AttributesMapperProfile : IMapperProfile<Attributes, AttributesSchema>
{
    public Attributes Map(AttributesSchema obj) => new()
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
        AtkSpeedPct = obj.AtkSpeedPct
    };

    public AttributesSchema Map(Attributes obj) => new()
    {
        Ac = obj.Ac,
        Con = obj.Con,
        Dex = obj.Dex,
        Dmg = obj.Dmg,
        Hit = obj.Hit,
        Int = obj.Int,
        MagicResistance = obj.MagicResistance,
        MaximumHp = obj.MaximumHp,
        MaximumMp = obj.MaximumMp,
        Str = obj.Str,
        Wis = obj.Wis,
        AtkSpeedPct = obj.AtkSpeedPct
    };
}