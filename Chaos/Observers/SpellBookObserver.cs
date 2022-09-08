using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Observers.Abstractions;

namespace Chaos.Observers;

public class SpellBookObserver : IPanelObserver<Spell>
{
    private readonly Aisling Aisling;

    public SpellBookObserver(Aisling aisling) => Aisling = aisling;

    public void OnAdded(Spell obj) => Aisling.Client.SendAddSpellToPane(obj);

    public void OnRemoved(byte slot, Spell obj) => Aisling.Client.SendRemoveSpellFromPane(slot);

    public void OnUpdated(byte originalSlot, Spell obj) => Aisling.Client.SendAddSpellToPane(obj);
}