using Chaos.Collections;
using Chaos.Geometry.Abstractions;
using Chaos.Models.World.Abstractions;
using Chaos.Time.Abstractions;

namespace Chaos.Models.World;

public class ProxyEntity : MapEntity, IDeltaUpdatable
{
    public IProxy Proxy { get; }

    /// <inheritdoc />
    public ProxyEntity(MapInstance mapInstance, IPoint point, IProxy proxy)
        : base(mapInstance, point)
    {
        Proxy = proxy;
        Proxy.Entity = this;
    }

    /// <inheritdoc />
    public virtual void Update(TimeSpan delta) => Proxy.Update(delta);

    public interface IProxy : IDeltaUpdatable
    {
        MapEntity Entity { set; }
    }

    public class ExternalizedProxy : IProxy
    {
        public MapEntity Entity { get; set; } = null!;
        public event Action<MapEntity, TimeSpan>? OnUpdated;
        public virtual void Update(TimeSpan delta) => OnUpdated?.Invoke(Entity, delta);
    }

    public class ExternalizedProxy<T> : IProxy
    {
        public T Data { get; set; }
        public MapEntity Entity { get; set; } = null!;
        public event Action<MapEntity, TimeSpan, T>? OnUpdated;

        public ExternalizedProxy(T data) => Data = data;

        public virtual void Update(TimeSpan delta) => OnUpdated?.Invoke(Entity, delta, Data);
    }
}