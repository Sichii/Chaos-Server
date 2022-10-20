using Chaos.Containers;
using Chaos.Objects.Panel;
using Chaos.Schemas.Aisling;
using Chaos.TypeMapper.Abstractions;

namespace Chaos.MapperProfiles;

public sealed class EquipmentMapperProfile : IMapperProfile<Equipment, EquipmentSchema>
{
    private readonly ITypeMapper Mapper;

    public EquipmentMapperProfile(ITypeMapper mapper) => Mapper = mapper;

    public Equipment Map(EquipmentSchema obj) => new(Mapper.MapMany<Item>(obj));

    public EquipmentSchema Map(Equipment obj) => new(Mapper.MapMany<ItemSchema>(obj));
}