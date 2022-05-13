using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Observers.Interfaces;

namespace Chaos.Observers;

public class SpellBookObserver : IPanelObserver<Spell>
{
    private readonly User User;

    public SpellBookObserver(User user) => User = user;

    public void OnAdded(Spell obj) => User.Client.SendAddSpellToPane(obj);

    public void OnRemoved(byte slot, Spell obj) => User.Client.SendRemoveSpellFromPane(slot);

    public void OnUpdated(byte originalSlot, Spell obj) => User.Client.SendAddSpellToPane(obj);
}