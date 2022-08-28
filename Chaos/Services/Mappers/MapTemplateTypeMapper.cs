using Chaos.Entities.Schemas.Templates;
using Chaos.Services.Mappers.Interfaces;
using Chaos.Templates;

namespace Chaos.Services.Mappers;

public class MapTemplateTypeMapper : ITypeMapper<MapTemplate, MapTemplateSchema>
{
    public MapTemplate Map(MapTemplateSchema obj) => new(obj);
    public MapTemplateSchema Map(MapTemplate obj) => throw new NotImplementedException();
}