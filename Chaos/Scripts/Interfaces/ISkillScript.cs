using Chaos.Objects.World;

namespace Chaos.Scripts.Interfaces;

public interface ISkillScript : IScript
{
    void OnUse(Aisling aisling);
}