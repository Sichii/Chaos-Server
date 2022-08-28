using Chaos.Data;
using Chaos.Entities.Schemas.World;
using Chaos.Services.Mappers.Interfaces;

namespace Chaos.Services.Mappers;

public class AttributesTypeMapper : ITypeMapper<Attributes, AttributesSchema>
{
    public Attributes Map(AttributesSchema obj) => new(obj);

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
        Wis = obj.Wis
    };
}