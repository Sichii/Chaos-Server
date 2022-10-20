using Chaos.Objects.Panel;
using Chaos.Objects.World;

namespace Chaos.Observers;

public sealed class SpellBookObserver : Abstractions.IObserver<Spell>
{
    private readonly Aisling Aisling;

    public SpellBookObserver(Aisling aisling) => Aisling = aisling;

    public void OnAdded(Spell obj) => Aisling.Client.SendAddSpellToPane(obj);

    public void OnRemoved(byte slot, Spell obj) => Aisling.Client.SendRemoveSpellFromPane(slot);

    public void OnUpdated(byte originalSlot, Spell obj) => Aisling.Client.SendAddSpellToPane(obj);
}