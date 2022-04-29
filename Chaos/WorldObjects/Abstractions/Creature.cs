using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Chaos.Containers;
using Chaos.Core.Definitions;
using Chaos.Core.Geometry;
using Chaos.DataObjects;
using Chaos.Effects.Interfaces;
using Chaos.Interfaces;

namespace Chaos.WorldObjects.Abstractions;

public abstract class Creature : NamedObject, IEffected, ICanDropGoldOn, ICanDropItemOn
{
    public Direction Direction { get; set; } = Direction.South;
    public EffectsBar Effects { get; init; } = new(Enumerable.Empty<IEffect>());
    public int GamePoints { get; set; }
    public int Gold { get; set; }
    public ConcurrentDictionary<uint, DateTime> LastClicked { get; init; } = new();
    public Status Status { get; set; }
    public virtual bool IsAlive => StatSheet.CurrentHp > 0;

    public virtual StatSheet StatSheet { get; } = new();
    public abstract CreatureType Type { get; }

    protected Creature(string name, MapInstance mapInstance, Point point, ushort sprite)
        : base(name, mapInstance, point, sprite) { }

    public bool ClickIsValid(uint fromId) =>
        !LastClicked.TryGetValue(fromId, out var lastClick) || (DateTime.UtcNow.Subtract(lastClick).TotalMilliseconds > 500);

    public virtual ValueTask OnUpdated(TimeSpan delta) => Effects.OnUpdated(delta);
    public abstract void GoldDroppedOn(int amount, User source);

    public abstract void ItemDroppedOn(byte slot, int count, User source);
}