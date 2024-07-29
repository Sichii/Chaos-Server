using System.Diagnostics;
using Chaos.Collections.Specialized;
using Chaos.Common.Utilities;
using Chaos.Extensions.Common;
using Chaos.Extensions.Geometry;
using Chaos.Geometry.Abstractions;
using Chaos.Geometry.EqualityComparers;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Time.Abstractions;

namespace Chaos.Collections;

public sealed class MapEntityCollection : IDeltaUpdatable
{
    private readonly HashSet<Aisling> Aislings;
    private readonly IRectangle Bounds;
    private readonly HashSet<Door> Doors;
    private readonly Dictionary<uint, MapEntity> EntityLookup;
    private readonly HashSet<GroundEntity> GroundEntities;

    // ReSharper disable once NotAccessedField.Local
    private readonly ILogger Logger;
    private readonly HashSet<Merchant> Merchants;
    private readonly HashSet<Monster> Monsters;
    private readonly QuadTreeWithSpatialHash<MapEntity> QuadTree;
    private readonly HashSet<ReactorTile> Reactors;
    private readonly UpdatableCollection Updatables;
    private readonly TypeSwitchExpression<IEnumerable> ValuesCases;

    private readonly int WalkableArea;

    public MapEntityCollection(
        ILogger logger,
        int mapWidth,
        int mapHeight,
        int walkableArea)
    {
        Bounds = new Rectangle(
            0,
            0,
            mapWidth,
            mapHeight);

        Logger = logger;
        EntityLookup = new Dictionary<uint, MapEntity>();
        QuadTree = new QuadTreeWithSpatialHash<MapEntity>(Bounds, EqualityComparer<WorldEntity>.Default);
        Monsters = new HashSet<Monster>(WorldEntity.IdComparer);
        Merchants = new HashSet<Merchant>(WorldEntity.IdComparer);
        Aislings = new HashSet<Aisling>(WorldEntity.IdComparer);
        GroundEntities = new HashSet<GroundEntity>(WorldEntity.IdComparer);
        Reactors = new HashSet<ReactorTile>(WorldEntity.IdComparer);
        Doors = new HashSet<Door>(WorldEntity.IdComparer);
        Updatables = new UpdatableCollection(logger);
        WalkableArea = walkableArea;

        //setup Values<T> cases
        ValuesCases = new TypeSwitchExpression<IEnumerable>().Case<Aisling>(Aislings)
                                                             .Case<Monster>(Monsters)
                                                             .Case<Merchant>(Merchants)
                                                             .Case<GroundItem>(() => GroundEntities.OfType<GroundItem>())
                                                             .Case<GroundEntity>(GroundEntities)
                                                             .Case<ReactorTile>(Reactors)
                                                             .Case<Door>(Doors)
                                                             .Case<Creature>(
                                                                 () => Aislings.Concat<Creature>(Monsters)
                                                                               .Concat(Merchants))
                                                             .Case<NamedEntity>(
                                                                 () => Aislings.Concat<NamedEntity>(Monsters)
                                                                               .Concat(Merchants)
                                                                               .Concat(GroundEntities))
                                                             .Case<VisibleEntity>(
                                                                 () => Aislings.Concat<VisibleEntity>(Monsters)
                                                                               .Concat(Merchants)
                                                                               .Concat(GroundEntities)
                                                                               .Concat(Doors))
                                                             .Default(EntityLookup.Values)
                                                             .Freeze();
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

    public void AddToPointLookup(MapEntity mapEntity) => QuadTree.Insert(mapEntity);

    public IEnumerable<T> AtPoint<T>(IPoint point) where T: MapEntity
        => Bounds.Contains(point)
            ? QuadTree.Query(Point.From(point))
                      .OfType<T>()
            : [];

    public IEnumerable<T> AtPoints<T>(IEnumerable<IPoint> points) where T: MapEntity => points.SelectMany(AtPoint<T>);

    public void Clear()
    {
        EntityLookup.Clear();
        Aislings.Clear();
        Monsters.Clear();
        GroundEntities.Clear();
        Reactors.Clear();
        Doors.Clear();
        Updatables.Clear();
        QuadTree.Clear();
    }

    public bool ContainsKey(uint id) => EntityLookup.ContainsKey(id);

    public void MoveEntity(MapEntity mapEntity, IPoint newPoint)
    {
        if (PointEqualityComparer.Instance.Equals(newPoint, mapEntity))
            return;

        QuadTree.Remove(mapEntity);
        mapEntity.SetLocation(mapEntity.MapInstance, newPoint);
        QuadTree.Insert(mapEntity);
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

    public bool RemoveFromPointLookup(MapEntity mapEntity) => QuadTree.Remove(mapEntity);

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
        var searchBounds = new Circle(point, range);

        return QuadTree.Query(searchBounds)
                       .OfType<T>();
    }
}