using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Observers.Interfaces;

namespace Chaos.Observers;

public class SkillBookObserver : IPanelObserver<Skill>
{
    private readonly Aisling Aisling;

    public SkillBookObserver(Aisling aisling) => Aisling = aisling;

    public void OnAdded(Skill obj) => Aisling.Client.SendAddSkillToPane(obj);

    public void OnRemoved(byte slot, Skill obj) => Aisling.Client.SendRemoveSkillFromPane(slot);

    public void OnUpdated(byte originalSlot, Skill obj) => Aisling.Client.SendAddSkillToPane(obj);
}