using Chaos.Models.Panel;
using Chaos.Models.World;

namespace Chaos.Observers;

public sealed class SpellBookObserver(Aisling aisling) : Abstractions.IObserver<Spell>
{
    private readonly Aisling Aisling = aisling;

    public void OnAdded(Spell obj) => Aisling.Client.SendAddSpellToPane(obj);

    public void OnRemoved(byte slot, Spell obj) => Aisling.Client.SendRemoveSpellFromPane(slot);

    public void OnUpdated(byte originalSlot, Spell obj) => Aisling.Client.SendAddSpellToPane(obj);
}