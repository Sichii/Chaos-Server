using Chaos.Containers;
using Chaos.Entities.Schemas.Aisling;
using Chaos.Objects.Panel;
using Chaos.TypeMapper.Abstractions;

namespace Chaos.MapperProfiles;

public class EquipmentMapperProfile : IMapperProfile<Equipment, EquipmentSchema>
{
    private readonly ITypeMapper Mapper;

    public EquipmentMapperProfile(ITypeMapper mapper) => Mapper = mapper;

    public Equipment Map(EquipmentSchema obj) => new(Mapper.MapMany<Item>(obj));

    public EquipmentSchema Map(Equipment obj) => new(Mapper.MapMany<ItemSchema>(obj));
}