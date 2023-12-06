using Chaos.Models.Panel;
using Chaos.Models.World;

namespace Chaos.Observers;

public sealed class SkillBookObserver(Aisling aisling) : Abstractions.IObserver<Skill>
{
    private readonly Aisling Aisling = aisling;

    public void OnAdded(Skill obj) => Aisling.Client.SendAddSkillToPane(obj);

    public void OnRemoved(byte slot, Skill obj) => Aisling.Client.SendRemoveSkillFromPane(slot);

    public void OnUpdated(byte originalSlot, Skill obj) => Aisling.Client.SendAddSkillToPane(obj);
}