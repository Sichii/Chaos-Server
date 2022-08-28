using Chaos.Containers;
using Chaos.Entities.Schemas.World;
using Chaos.Services.Caches.Interfaces;
using Chaos.Services.Mappers.Interfaces;

namespace Chaos.Services.Mappers;

public class MapInstanceTypeMapper : ITypeMapper<MapInstance, MapInstanceSchema>
{
    private readonly ISimpleCache SimpleCache;
    public MapInstanceTypeMapper(ISimpleCache simpleCache) => SimpleCache = simpleCache;

    public MapInstance Map(MapInstanceSchema obj) => new(obj, SimpleCache);

    public MapInstanceSchema Map(MapInstance obj) => throw new NotImplementedException();
}