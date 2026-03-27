#region
using Chaos.Collections.Specialized;
using Chaos.Geometry;
using Chaos.Geometry.Abstractions;
using Microsoft.Extensions.ObjectPool;
#endregion

namespace Chaos.Collections.ObjectPool;

internal sealed class QuadTreePoolPolicy<T> : PooledObjectPolicy<QuadTree<T>> where T: IPoint
{
    public Lock Sync { get; } = new();

    public override QuadTree<T> Create()
    {
        using var @lock = Sync.EnterScope();

        return new QuadTree<T>(new Rectangle());
    }

    public override bool Return(QuadTree<T> obj)
    {
        using var @lock = Sync.EnterScope();

        obj.Clear();

        return true;
    }
}