using Chaos.Objects.Panel;
using Chaos.Objects.World.Abstractions;

namespace Chaos.Scripting.Abstractions;

public interface ICreatureScript : IScript
{
    bool CanMove();

    bool CanTalk();

    bool CanTurn();

    bool CanUseSkill(Skill skill);

    bool CanUseSpell(Spell spell);

    void OnAttacked(Creature source, int damage);
    void OnHealed(Creature source, int healing);
}