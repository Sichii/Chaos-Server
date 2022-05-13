using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Observers.Interfaces;

namespace Chaos.Observers;

public class SkillBookObserver : IPanelObserver<Skill>
{
    private readonly User User;

    public SkillBookObserver(User user) => User = user;

    public void OnAdded(Skill obj) => User.Client.SendAddSkillToPane(obj);

    public void OnRemoved(byte slot, Skill obj) => User.Client.SendRemoveSkillFromPane(slot);

    public void OnUpdated(byte originalSlot, Skill obj) => User.Client.SendAddSkillToPane(obj);
}