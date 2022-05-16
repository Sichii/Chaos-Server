using Chaos.Containers;
using Chaos.Effects.Interfaces;

namespace Chaos.Objects.World.Abstractions;

public abstract class Creature : NamedObject, IEffected
{
    public Direction Direction { get; set; } = Direction.South;
    public IEffectsBar Effects { get; init; }
    public int GamePoints { get; set; }
    public int Gold { get; set; }
    public ConcurrentDictionary<uint, DateTime> LastClicked { get; init; } = new();
    public Status Status { get; set; }
    public virtual bool IsAlive => StatSheet.CurrentHp > 0;

    public virtual StatSheet StatSheet { get; } = new();
    public abstract CreatureType Type { get; }

    protected Creature(
        string name,
        MapInstance mapInstance,
        Point point,
        ushort sprite
    )
        : base(
            name,
            mapInstance,
            point,
            sprite) => Effects = new EffectsBar(this, Enumerable.Empty<IEffect>());

    public bool ClickIsValid(uint fromId) =>
        !LastClicked.TryGetValue(fromId, out var lastClick) || (DateTime.UtcNow.Subtract(lastClick).TotalMilliseconds > 500);

    public abstract void GoldDroppedOn(int amount, User source);

    public abstract void ItemDroppedOn(byte slot, byte count, User source);

    public virtual void Update(TimeSpan delta) => Effects.Update(delta);
}