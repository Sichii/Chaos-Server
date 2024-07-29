using Chaos.Geometry.Abstractions;

namespace Chaos.Collections.Specialized;

public sealed class QuadTreeWithSpatialHash<T> : QuadTree<T> where T: IPoint
{
    private readonly SpatialHash<T> SpatialHash;

    public QuadTreeWithSpatialHash(IRectangle bounds, IEqualityComparer<T>? comparer = default)
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
    public override IEnumerable<T> Query(IPoint point) => SpatialHash.Query(point);

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