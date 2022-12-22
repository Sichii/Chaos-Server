using System.Runtime.InteropServices;
using Chaos.Extensions;
using Chaos.Extensions.Common;
using Chaos.Extensions.Geometry;
using Chaos.Geometry.Abstractions;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;
using Chaos.Time.Abstractions;

namespace Chaos.Containers;

public sealed class MapEntityCollection : IDeltaUpdatable
{
    private readonly HashSet<Aisling> Aislings;
    private readonly Rectangle Bounds;
    private readonly HashSet<Door> Doors;
    private readonly Dictionary<uint, MapEntity> EntityLookup;
    private readonly HashSet<GroundEntity> GroundEntities;
    private readonly HashSet<Merchant> Merchants;
    private readonly HashSet<Monster> Monsters;
    private readonly HashSet<MapEntity>[,] PointLookup;
    private readonly HashSet<ReactorTile> Reactors;
    private readonly int WalkableArea;

    public MapEntityCollection(int mapWidth, int mapHeight, int walkableWalkableArea)
    {
        EntityLookup = new Dictionary<uint, MapEntity>();
        PointLookup = new HashSet<MapEntity>[mapWidth, mapHeight];
        Monsters = new HashSet<Monster>(WorldEntity.IdComparer);
        Merchants = new HashSet<Merchant>(WorldEntity.IdComparer);
        Aislings = new HashSet<Aisling>(WorldEntity.IdComparer);
        GroundEntities = new HashSet<GroundEntity>(WorldEntity.IdComparer);
        Reactors = new HashSet<ReactorTile>(WorldEntity.IdComparer);
        Doors = new HashSet<Door>(WorldEntity.IdComparer);

        Bounds = new Rectangle(
            0,
            0,
            mapWidth,
            mapHeight);

        WalkableArea = walkableWalkableArea;

        for (var x = 0; x < mapWidth; x++)
            for (var y = 0; y < mapHeight; y++)
                PointLookup[x, y] = new HashSet<MapEntity>(WorldEntity.IdComparer);
    }

    public void Add(uint id, MapEntity entity)
    {
        EntityLookup.Add(id, entity);
        AddToPointLookup(entity);

        switch (entity)
        {
            case Aisling aisling:
                Aislings.Add(aisling);

                break;
            case Monster monster:
                Monsters.Add(monster);

                break;
            case Merchant merchant:
                Merchants.Add(merchant);

                break;
            case GroundEntity groundEntity:
                GroundEntities.Add(groundEntity);

                break;
            case ReactorTile reactor:
                Reactors.Add(reactor);

                break;
            case Door door:
                Doors.Add(door);

                break;
            default:
                throw new InvalidOperationException("Unknown entity type");
        }
    }

    public void AddToPointLookup(MapEntity mapEntity)
    {
        var entities = PointLookup[mapEntity.X, mapEntity.Y];
        entities.Add(mapEntity);
    }

    public IEnumerable<T> AtPoint<T>(IPoint point) where T: MapEntity =>
        Bounds.Contains(point) ? PointLookup[point.X, point.Y].OfType<T>() : Enumerable.Empty<T>();

    public IEnumerable<T> AtPoints<T>(IEnumerable<IPoint> points) where T: MapEntity =>
        points.SelectMany(AtPoint<T>);

    public void Clear()
    {
        EntityLookup.Clear();
        Aislings.Clear();
        Monsters.Clear();
        GroundEntities.Clear();
        Reactors.Clear();
        Doors.Clear();

        foreach (var lookup in PointLookup.Flatten())
            lookup.Clear();
    }

    public bool ContainsKey(uint id) => EntityLookup.ContainsKey(id);

    public void MoveEntity(MapEntity mapEntity, IPoint oldPoint)
    {
        var fromEntities = PointLookup[oldPoint.X, oldPoint.Y];
        var toEntities = PointLookup[mapEntity.X, mapEntity.Y];

        if (fromEntities.Remove(mapEntity))
            toEntities.Add(mapEntity);
    }

    public bool Remove(uint id)
    {
        if (!EntityLookup.TryRemove(id, out var entity))
            return false;

        if (!RemoveFromPointLookup(entity))
            return false;

        switch (entity)
        {
            case Aisling aisling:
                Aislings.Remove(aisling);

                break;
            case Monster monster:
                Monsters.Remove(monster);

                break;
            case Merchant merchant:
                Merchants.Remove(merchant);

                break;
            case GroundEntity groundEntity:
                GroundEntities.Remove(groundEntity);

                break;
            case ReactorTile reactor:
                Reactors.Remove(reactor);

                break;
            case Door door:
                Doors.Remove(door);

                break;
            default:
                throw new InvalidOperationException("Unknown entity type");
        }

        return true;
    }

    public bool RemoveFromPointLookup(MapEntity mapEntity)
    {
        var entities = PointLookup[mapEntity.X, mapEntity.Y];

        return entities.Remove(mapEntity);
    }

    public bool TryGetValue<T>(uint id, [NotNullWhen(true)] out T? entity)
    {
        entity = default;

        if (EntityLookup.TryGetValue(id, out var obj) && obj is T t)
        {
            entity = t;

            return true;
        }

        return false;
    }

    /// <inheritdoc />
    public void Update(TimeSpan delta)
    {
        var entities = EntityLookup.Values
                                   .OfType<IDeltaUpdatable>()
                                   .ToList();

        foreach (ref var entity in CollectionsMarshal.AsSpan(entities))
            entity.Update(delta);
    }

    public IEnumerable<T> Values<T>() where T: MapEntity
    {
        var type = typeof(T);

        if (type == typeof(MapEntity))
            return (IEnumerable<T>)EntityLookup.Values.AsEnumerable();

        if (type == typeof(VisibleEntity))
            return (IEnumerable<T>)Doors.Concat<VisibleEntity>(Monsters)
                                        .Concat(Merchants)
                                        .Concat(Aislings)
                                        .Concat(GroundEntities);

        if (type == typeof(Door))
            return (IEnumerable<T>)Doors;

        if (type == typeof(NamedEntity))
            return (IEnumerable<T>)Monsters
                                   .Concat<NamedEntity>(Merchants)
                                   .Concat(Aislings)
                                   .Concat(GroundEntities);

        if (type == typeof(Creature))
            return (IEnumerable<T>)Monsters
                                   .Concat<Creature>(Merchants)
                                   .Concat(Aislings);

        if (type == typeof(Aisling))
            return (IEnumerable<T>)Aislings;

        if (type == typeof(Monster))
            return (IEnumerable<T>)Monsters;

        if (type == typeof(Merchant))
            return (IEnumerable<T>)Merchants;

        if (type == typeof(ReactorTile))
            return (IEnumerable<T>)Reactors;

        if (type.IsAssignableTo(typeof(GroundEntity)))
            return GroundEntities.OfType<T>();

        return EntityLookup.Values.OfType<T>();
    }

    public IEnumerable<T> WithinRange<T>(IPoint point, int range = 12) where T: MapEntity
    {
        var area = Math.Ceiling(Math.Pow(range * 2 + 1, 2) / 2);
        var avgEntitiesPerTile = Math.Max(0.33, EntityLookup.Count / (float)WalkableArea);
        int entityCount;
        var tType = typeof(T);

        //get an estimate of the number of entities that exist on the map for the type theyre trying to get
        if (tType.IsAssignableTo(typeof(Aisling)))
            entityCount = Aislings.Count;
        else if (tType.IsAssignableTo(typeof(Creature)))
            entityCount = Monsters.Count;
        else if (tType.IsAssignableTo(typeof(GroundEntity)))
            entityCount = GroundEntities.Count;
        else if (tType.IsAssignableFrom(typeof(VisibleEntity)))
            entityCount = EntityLookup.Count;
        else
            entityCount = 1;

        //if we can expect to search significantly fewer entities by searching points
        //then search by point lookup
        if (10 + area * avgEntitiesPerTile < entityCount)
            return point.SpiralSearch(range)
                        .Where(pt => Bounds.Contains(pt))
                        .Select(pt => PointLookup[pt.X, pt.Y])
                        .Where(cell => cell.Count > 0)
                        .SelectMany(_ => _)
                        .OfType<T>();

        //otherwise just check every entity of that type with a distance check
        return Values<T>().ThatAreWithinRange(point, range);
    }
}