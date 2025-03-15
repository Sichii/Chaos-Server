#region
using Chaos.Geometry;
using Chaos.Geometry.Abstractions;
#endregion

namespace Chaos.Collections.Specialized;

/// <summary>
///     A QuadTree with a SpatialHash for fast point-based queries.
/// </summary>
/// <typeparam name="T">
///     An implementation of <see cref="IPoint" />
/// </typeparam>
public sealed class QuadTreeWithSpatialHash<T> : QuadTree<T> where T: IPoint
{
    private readonly SpatialHash<T> SpatialHash;

    /// <summary>
    ///     Initializes a new instance of <see cref="QuadTreeWithSpatialHash{T}" />.
    /// </summary>
    /// <param name="bounds">
    ///     The initial bounds of the root of this QuadTree
    /// </param>
    /// <param name="comparer">
    ///     The comparer used by the <see cref="SpatialHash" />
    /// </param>
    public QuadTreeWithSpatialHash(IRectangle bounds, IEqualityComparer<T>? comparer = null)
        : base(bounds)
        => SpatialHash = new SpatialHash<T>(comparer);

    /// <inheritdoc />
    public override void Clear()
    {
        base.Clear();
        SpatialHash.Clear();
    }

    /// <inheritdoc />
    public override bool Insert(T item)
    {
        if (base.Insert(item))
        {
            SpatialHash.Add(item);

            return true;
        }

        return false;
    }

    /// <inheritdoc />
    public override IEnumerable<T> Query(Point point) => SpatialHash.Query(point);

    /// <inheritdoc />
    public override bool Remove(T entity)
    {
        if (base.Remove(entity))
        {
            SpatialHash.Remove(entity);

            return true;
        }

        return false;
    }
}