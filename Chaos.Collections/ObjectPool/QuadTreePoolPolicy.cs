using Chaos.Collections.Specialized;
using Chaos.Geometry;
using Chaos.Geometry.Abstractions;
using Microsoft.Extensions.ObjectPool;

namespace Chaos.Collections.ObjectPool;

internal sealed class QuadTreePoolPolicy<T> : PooledObjectPolicy<QuadTree<T>> where T: IPoint
{
    public override QuadTree<T> Create() => new(new Rectangle());

    public override bool Return(QuadTree<T> obj)
    {
        obj.Clear();

        return true;
    }
}