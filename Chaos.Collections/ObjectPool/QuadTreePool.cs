using Chaos.Collections.Specialized;
using Chaos.Geometry;
using Chaos.Geometry.Abstractions;
using Microsoft.Extensions.ObjectPool;

namespace Chaos.Collections.ObjectPool;

internal sealed class QuadTreePool<T> : DefaultObjectPool<QuadTree<T>> where T: IPoint
{
    public QuadTreePool(int poolCapacity)
        : base(new QuadTreePoolPolicy<T>(), poolCapacity) { }

    public QuadTree<T> Get(Rectangle bounds)
    {
        var ret = Get();
        ret.SetBounds(bounds);

        return ret;
    }
}