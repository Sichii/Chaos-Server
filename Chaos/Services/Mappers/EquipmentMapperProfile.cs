using Chaos.Containers;
using Chaos.Entities.Schemas.Aisling;
using Chaos.Objects.Panel;
using Chaos.Services.Mappers.Abstractions;

namespace Chaos.Services.Mappers;

public class EquipmentMapperProfile : IMapperProfile<Equipment, EquipmentSchema>
{
    private readonly ITypeMapper Mapper;

    public EquipmentMapperProfile(ITypeMapper mapper) => Mapper = mapper;

    public Equipment Map(EquipmentSchema obj) => new(Mapper.MapMany<Item>(obj));

    public EquipmentSchema Map(Equipment obj) => new(Mapper.MapMany<ItemSchema>(obj));
}