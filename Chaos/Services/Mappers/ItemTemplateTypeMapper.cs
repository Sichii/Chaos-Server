using Chaos.Entities.Schemas.Templates;
using Chaos.Services.Mappers.Interfaces;
using Chaos.Templates;

namespace Chaos.Services.Mappers;

public class ItemTemplateTypeMapper : ITypeMapper<ItemTemplate, ItemTemplateSchema>
{
    private readonly ITypeMapper Mapper;
    public ItemTemplateTypeMapper(ITypeMapper mapper) => Mapper = mapper;

    public ItemTemplate Map(ItemTemplateSchema obj) => new(obj, Mapper);

    public ItemTemplateSchema Map(ItemTemplate obj) => throw new NotImplementedException();
}