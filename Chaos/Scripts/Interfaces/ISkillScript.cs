using Chaos.Objects.World;

namespace Chaos.Scripts.Interfaces;

public interface ISkillScript : IScript
{
    void OnForgotten(Aisling aisling);

    void OnLearned(Aisling aisling);
    void OnUse(Aisling aisling);
}