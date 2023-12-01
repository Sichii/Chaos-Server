using System.Diagnostics;
using Chaos.Common.Utilities;
using Chaos.Extensions;
using Chaos.Extensions.Common;
using Chaos.Extensions.Geometry;
using Chaos.Geometry.Abstractions;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Time.Abstractions;
using Microsoft.Extensions.Logging;

namespace Chaos.Collections;

public sealed class MapEntityCollection : IDeltaUpdatable
{
    private readonly HashSet<Aisling> Aislings;
    private readonly Rectangle Bounds;
    private readonly HashSet<Door> Doors;
    private readonly Dictionary<uint, MapEntity> EntityLookup;
    private readonly HashSet<GroundEntity> GroundEntities;
    // ReSharper disable once NotAccessedField.Local
    private readonly ILogger Logger;
    private readonly HashSet<Merchant> Merchants;
    private readonly HashSet<Monster> Monsters;
    private readonly HashSet<MapEntity>[,] PointLookup;
    private readonly HashSet<ReactorTile> Reactors;
    private readonly UpdatableCollection Updatables;
    private readonly TypeSwitchExpression<IEnumerable> ValuesCases;

    private readonly int WalkableArea;

    public MapEntityCollection(
        ILogger logger,
        int mapWidth,
        int mapHeight,
        int walkableArea
    )
    {
        Logger = logger;
        EntityLookup = new Dictionary<uint, MapEntity>();
        PointLookup = new HashSet<MapEntity>[mapWidth, mapHeight];
        Monsters = new HashSet<Monster>(WorldEntity.IdComparer);
        Merchants = new HashSet<Merchant>(WorldEntity.IdComparer);
        Aislings = new HashSet<Aisling>(WorldEntity.IdComparer);
        GroundEntities = new HashSet<GroundEntity>(WorldEntity.IdComparer);
        Reactors = new HashSet<ReactorTile>(WorldEntity.IdComparer);
        Doors = new HashSet<Door>(WorldEntity.IdComparer);
        Updatables = new UpdatableCollection(logger);

        Bounds = new Rectangle(
            0,
            0,
            mapWidth,
            mapHeight);

        WalkableArea = walkableArea;

        for (var x = 0; x < mapWidth; x++)
            for (var y = 0; y < mapHeight; y++)
                PointLookup[x, y] = new HashSet<MapEntity>(WorldEntity.IdComparer);

        //setup Values<T> cases
        ValuesCases = new TypeSwitchExpression<IEnumerable>().Case<Aisling>(Aislings)
                                                             .Case<Monster>(Monsters)
                                                             .Case<Merchant>(Merchants)
                                                             .Case<GroundEntity>(GroundEntities)
                                                             .Case<ReactorTile>(Reactors)
                                                             .Case<Door>(Doors)
                                                             .Case<Creature>(() => Aislings.Concat<Creature>(Monsters).Concat(Merchants))
                                                             .Case<NamedEntity>(() => Aislings.Concat<NamedEntity>(Monsters).Concat(Merchants).Concat(GroundEntities))
                                                             .Case<VisibleEntity>(
                                                                 () => Aislings.Concat<VisibleEntity>(Monsters).Concat(Merchants).Concat(GroundEntities).Concat(Doors))
                                                             .Default(EntityLookup.Values);
    }

    /// <inheritdoc />
    public void Update(TimeSpan delta) => Updatables.Update(delta);

    public void Add(uint id, MapEntity entity)
    {
        EntityLookup.Add(id, entity);
        AddToPointLookup(entity);

        if (entity is IDeltaUpdatable updatable)
            Updatables.Add(updatable);

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
                throw new InvalidOperationException("Unrecognized entity type");
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
        Updatables.Clear();

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

        if (entity is IDeltaUpdatable updatable)
            Updatables.Remove(updatable);

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
                throw new InvalidOperationException("Unrecognized entity type");
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

    public IEnumerable<T> Values<T>() where T: MapEntity
    {
        var result = ValuesCases.Switch<T>();

        // ReSharper disable once ConvertIfStatementToSwitchStatement
        if (result is null)
            throw new UnreachableException("Expression result should never be null. If it is, some type is not handled");

        if (result is IEnumerable<T> typedResult)
            return typedResult;

        return result.OfType<T>();
    }

    public IEnumerable<T> WithinRange<T>(IPoint point, int range = 15) where T: MapEntity
    {
        var area = Math.Ceiling(Math.Pow(range * 2 + 1, 2) / 2);
        var avgEntitiesPerTile = Math.Max(0.33, EntityLookup.Count / (float)WalkableArea);
        int entityCount;
        var tType = typeof(T);

        //get an estimate of the number of entities that exist on the map for the type theyre trying to get
        if (tType.IsAssignableTo(typeof(Aisling)))
            entityCount = Aislings.Count;
        else if (tType.IsAssignableTo(typeof(Monster)))
            entityCount = Monsters.Count;
        else if (tType.IsAssignableTo(typeof(Merchant)))
            entityCount = Merchants.Count;
        else if (tType.IsAssignableTo(typeof(GroundEntity)))
            entityCount = GroundEntities.Count;
        else if (tType.IsAssignableTo(typeof(ReactorTile)))
            entityCount = Reactors.Count;
        else if (tType.IsAssignableFrom(typeof(VisibleEntity)))
            entityCount = EntityLookup.Count;
        else
            entityCount = 1;

        //if we can expect to search significantly fewer entities by searching points
        //then search by point lookup
        if (10 + area * avgEntitiesPerTile < entityCount)
            foreach (var pt in point.SpiralSearch(range))
            {
                if (!Bounds.Contains(pt))
                    continue;

                var entities = PointLookup[pt.X, pt.Y];

                if (entities.Count == 0)
                    continue;

                foreach (var entity in entities)
                    if (entity is T t)
                        yield return t;
            }
        else //otherwise just check every entity of that type with a distance check
            foreach (var entity in Values<T>().ThatAreWithinRange(point, range))
                yield return entity;
    }
}