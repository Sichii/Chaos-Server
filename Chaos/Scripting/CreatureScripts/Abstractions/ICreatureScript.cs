using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
using Chaos.Scripting.Abstractions;
using Chaos.Time.Abstractions;

namespace Chaos.Scripting.CreatureScripts.Abstractions;

public interface ICreatureScript : IScript, IDeltaUpdatable
{
    bool CanDropItemOn(Aisling source, Item item);
    bool CanMove();
    bool CanSee(VisibleEntity entity);
    bool CanTalk();
    bool CanTurn();
    bool CanUseSkill(Skill skill);
    bool CanUseSpell(Spell spell);
    bool IsBlind();
    bool IsFriendlyTo(Creature creature);
    bool IsHostileTo(Creature creature);
    void OnApproached(Creature source);
    void OnAttacked(Creature source, int damage);
    void OnClicked(Aisling source);
    void OnDeath();
    void OnDeparture(Creature source);
    void OnGoldDroppedOn(Aisling source, int amount);
    void OnHealed(Creature source, int healing);
    void OnItemDroppedOn(Aisling source, Item item);
    void OnPublicMessage(Creature source, string message);
}