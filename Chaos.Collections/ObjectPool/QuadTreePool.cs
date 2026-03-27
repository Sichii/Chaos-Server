#region
using Chaos.Collections.Specialized;
using Chaos.Geometry;
using Chaos.Geometry.Abstractions;
using Microsoft.Extensions.ObjectPool;
#endregion

namespace Chaos.Collections.ObjectPool;

internal sealed class QuadTreePool<T> : DefaultObjectPool<QuadTree<T>> where T: IPoint
{
    private readonly Lock Sync;

    public QuadTreePool(QuadTreePoolPolicy<T> policy, int poolCapacity)
        : base(policy, poolCapacity)
        => Sync = policy.Sync;

    public QuadTree<T> Get(Rectangle bounds)
    {
        using var @lock = Sync.EnterScope();

        var ret = Get();
        ret.SetBounds(bounds);

        return ret;
    }

    /// <inheritdoc />
    public override QuadTree<T> Get()
    {
        using var @lock = Sync.EnterScope();

        return base.Get();
    }

    /// <inheritdoc />
    public override void Return(QuadTree<T> obj)
    {
        using var @lock = Sync.EnterScope();

        base.Return(obj);
    }
}