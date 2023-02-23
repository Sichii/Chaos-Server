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

    void OnAttacked(Creature attacker, int damage);
}