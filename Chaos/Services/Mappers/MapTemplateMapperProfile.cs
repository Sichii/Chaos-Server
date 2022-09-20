using Chaos.Data;
using Chaos.Entities.Schemas.Templates;
using Chaos.Services.Mappers.Abstractions;
using Chaos.Templates;

namespace Chaos.Services.Mappers;

public class MapTemplateMapperProfile : IMapperProfile<MapTemplate, MapTemplateSchema>
{
    public MapTemplate Map(MapTemplateSchema obj) => new()
    {
        Width = obj.Width,
        Height = obj.Height,
        TemplateKey = obj.TemplateKey,
        WarpPoints = obj.WarpPoints,
        ScriptKeys = new HashSet<string>(obj.ScriptKeys, StringComparer.OrdinalIgnoreCase),
        Tiles = new Tile[obj.Width, obj.Height]
    };

    public MapTemplateSchema Map(MapTemplate obj) => throw new NotImplementedException();
}