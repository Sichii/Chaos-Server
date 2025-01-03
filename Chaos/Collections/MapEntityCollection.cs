#region
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Chaos.Collections.Specialized;
using Chaos.Common.Utilities;
using Chaos.Extensions.Common;
using Chaos.Extensions.Geometry;
using Chaos.Geometry.Abstractions;
using Chaos.Geometry.EqualityComparers;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Time.Abstractions;
#endregion

namespace Chaos.Collections;

/// <summary>
///     Represents a collection of entities on a map
/// </summary>
/// <remarks>
///     This is a very specialized collection used to satisfy very specific requirements. It is not intended for general
///     use
/// </remarks>
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

    /// <summary>
    ///     Initializes a new instance of the <see cref="MapEntityCollection" /> class
    /// </summary>
    /// <param name="logger">
    ///     A logger to log messages
    /// </param>
    /// <param name="mapWidth">
    ///     The width of the map
    /// </param>
    /// <param name="mapHeight">
    ///     The height of the map
    /// </param>
    public MapEntityCollection(ILogger logger, int mapWidth, int mapHeight)
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

    /// <summary>
    ///     Adds an entity to the collection
    /// </summary>
    /// <param name="id">
    ///     The id of the entity
    /// </param>
    /// <param name="entity">
    ///     The entity being added
    /// </param>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when an unrecognized entity type is added
    /// </exception>
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

    private void AddToPointLookup(MapEntity mapEntity) => QuadTree.Insert(mapEntity);

    [OverloadResolutionPriority(1)]
    private IEnumerable<T> AtPoint<T>(Point point) where T: MapEntity
        => Bounds.Contains(point)
            ? QuadTree.Query(point)
                      .OfType<T>()
            : [];

    /// <summary>
    ///     Gets all entities at the given points
    /// </summary>
    /// <param name="points">
    ///     The points at which to look for entities
    /// </param>
    /// <typeparam name="T">
    ///     The type of entity to look for. Must inherit from <see cref="MapEntity" />
    /// </typeparam>
    /// <remarks>
    ///     This method uses a spatial hash to quickly find entities at the given points. However, it is less efficient for
    ///     large sets of points. Try using <see cref="WithinRange{T}" /> in those cases
    /// </remarks>
    [OverloadResolutionPriority(1)]
    public IEnumerable<T> AtPoints<T>(params IEnumerable<Point> points) where T: MapEntity => points.SelectMany(AtPoint<T>);

    /// <summary>
    ///     Gets all entities at the given points
    /// </summary>
    /// <param name="points">
    ///     The points at which to look for entities
    /// </param>
    /// <typeparam name="T">
    ///     The type of entity to look for. Must inherit from <see cref="MapEntity" />
    /// </typeparam>
    /// <remarks>
    ///     This method uses a spatial hash to quickly find entities at the given points. However, it is less efficient for
    ///     large sets of points. Try using <see cref="WithinRange{T}" /> in those cases
    /// </remarks>
    public IEnumerable<T> AtPoints<T>(params IEnumerable<IPoint> points) where T: MapEntity => AtPoints<T>(points.Select(Point.From));

    /// <summary>
    ///     Clears the collection of all entities
    /// </summary>
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

    /// <summary>
    ///     Determines if the collection contains an entity with the given id
    /// </summary>
    /// <param name="id">
    ///     The id to look for
    /// </param>
    /// <returns>
    ///     <c>
    ///         true
    ///     </c>
    ///     if the collection contains an entity with the given id, otherwise
    ///     <c>
    ///         false
    ///     </c>
    /// </returns>
    public bool ContainsKey(uint id) => EntityLookup.ContainsKey(id);

    /// <summary>
    ///     Moves an entity within the collection
    /// </summary>
    /// <param name="mapEntity">
    ///     The entity to move
    /// </param>
    /// <param name="newPoint">
    ///     The point to move the entity to
    /// </param>
    /// <remarks>
    ///     Since this collection has spatial aspects to it, when an entity's location changes, it must be given special care.
    ///     This method will also set the entity's location data
    /// </remarks>
    public void MoveEntity(MapEntity mapEntity, IPoint newPoint)
    {
        if (PointEqualityComparer.Instance.Equals(newPoint, mapEntity))
            return;

        QuadTree.Remove(mapEntity);
        mapEntity.SetLocation(mapEntity.MapInstance, newPoint);
        QuadTree.Insert(mapEntity);
    }

    /// <summary>
    ///     Removes an entity from the collection
    /// </summary>
    /// <param name="id">
    ///     The id of the entity to remove
    /// </param>
    /// <returns>
    ///     <c>
    ///         true
    ///     </c>
    ///     if the entity was found and removed, otherwise
    ///     <c>
    ///         false
    ///     </c>
    /// </returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when an unrecognized entity type is removed
    /// </exception>
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

    private bool RemoveFromPointLookup(MapEntity mapEntity) => QuadTree.Remove(mapEntity);

    /// <summary>
    ///     Attempts to retrieve the entity with the given id
    /// </summary>
    /// <param name="id">
    ///     The id of the entity to retrieve
    /// </param>
    /// <param name="entity">
    ///     The entity if found
    /// </param>
    /// <typeparam name="T">
    ///     The type of the entity to find or cast to
    /// </typeparam>
    /// <returns>
    ///     <c>
    ///         true
    ///     </c>
    ///     if the entity was found and was able to be cast to the specified type, otherwise
    ///     <c>
    ///         false
    ///     </c>
    /// </returns>
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

    /// <summary>
    ///     Gets all entities of the specified type
    /// </summary>
    /// <typeparam name="T">
    ///     The type of entity to find or cast to
    /// </typeparam>
    /// <exception cref="UnreachableException">
    ///     Thrown when the expression result is null
    /// </exception>
    /// <remarks>
    ///     This method will retrieve all entities of the specified type. If the type has inheritors, those will also be
    ///     returned if found
    /// </remarks>
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

    /// <summary>
    ///     Gets all entities within a certain range of a point
    /// </summary>
    /// <param name="point">
    ///     The point around which to search
    /// </param>
    /// <param name="range">
    ///     The range of the search
    /// </param>
    /// <typeparam name="T">
    ///     The type of entity to retrieve
    /// </typeparam>
    /// <remarks>
    ///     This method queries a quadtree to find entities. It is very efficient for large search areas, but less so for small
    ///     areas. Try using <see cref="AtPoints{T}(System.Collections.Generic.IEnumerable{Chaos.Geometry.Point})" /> in this
    ///     cases
    /// </remarks>
    public IEnumerable<T> WithinRange<T>(IPoint point, int range = 15) where T: MapEntity
    {
        var searchBounds = new Circle(point, range);

        return QuadTree.Query(searchBounds)
                       .OfType<T>();
    }
}