using Chaos.Models.Panel;
using Chaos.Models.World;

namespace Chaos.Observers;

public sealed class SpellBookObserver(Aisling aisling) : Abstractions.IObserver<Spell>
{
    private readonly Aisling Aisling = aisling;

    /// <inheritdoc />
    public bool Equals(Abstractions.IObserver<Spell>? other)
    {
        if (ReferenceEquals(null, other))
            return false;

        if (ReferenceEquals(this, other))
            return true;

        return other is SpellBookObserver equipmentObserver && Aisling.Equals(equipmentObserver.Aisling);
    }

    public void OnAdded(Spell obj) => Aisling.Client.SendAddSpellToPane(obj);

    public void OnRemoved(byte slot, Spell obj) => Aisling.Client.SendRemoveSpellFromPane(slot);

    public void OnUpdated(byte originalSlot, Spell obj) => Aisling.Client.SendAddSpellToPane(obj);

    /// <inheritdoc />
    public override bool Equals(object? obj) => ReferenceEquals(this, obj) || (obj is SpellBookObserver other && Equals(other));

    /// <inheritdoc />
    public override int GetHashCode() => HashCode.Combine(Aisling, typeof(SpellBookObserver));

    public static bool operator ==(SpellBookObserver? left, SpellBookObserver? right) => Equals(left, right);
    public static bool operator !=(SpellBookObserver? left, SpellBookObserver? right) => !Equals(left, right);
}