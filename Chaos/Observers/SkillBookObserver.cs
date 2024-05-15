using Chaos.Models.Panel;
using Chaos.Models.World;

namespace Chaos.Observers;

public sealed class SkillBookObserver(Aisling aisling) : Abstractions.IObserver<Skill>
{
    private readonly Aisling Aisling = aisling;

    /// <inheritdoc />
    public bool Equals(Abstractions.IObserver<Skill>? other)
    {
        if (ReferenceEquals(null, other))
            return false;

        if (ReferenceEquals(this, other))
            return true;

        return other is SkillBookObserver equipmentObserver && Aisling.Equals(equipmentObserver.Aisling);
    }

    public void OnAdded(Skill obj) => Aisling.Client.SendAddSkillToPane(obj);

    public void OnRemoved(byte slot, Skill obj) => Aisling.Client.SendRemoveSkillFromPane(slot);

    public void OnUpdated(byte originalSlot, Skill obj) => Aisling.Client.SendAddSkillToPane(obj);

    /// <inheritdoc />
    public override bool Equals(object? obj) => ReferenceEquals(this, obj) || (obj is SkillBookObserver other && Equals(other));

    /// <inheritdoc />
    public override int GetHashCode() => HashCode.Combine(Aisling, typeof(SkillBookObserver));

    public static bool operator ==(SkillBookObserver? left, SkillBookObserver? right) => Equals(left, right);
    public static bool operator !=(SkillBookObserver? left, SkillBookObserver? right) => !Equals(left, right);
}