#region
using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;
#endregion

namespace Chaos.Scripting.Behaviors;

public class RestrictionBehavior
{
    public virtual bool CanDropItem(Aisling aisling, Item item) => aisling.IsAlive;

    public virtual bool CanDropItemOn(Aisling aisling, Item item, Creature target) => aisling.IsAlive;

    public virtual bool CanDropMoney(Aisling aisling, int amount) => aisling.IsAlive;

    public virtual bool CanDropMoneyOn(Aisling aisling, int amount, Creature target) => aisling.IsAlive;
    public virtual bool CanMove(Creature creature) => creature.IsAlive;

    public virtual bool CanPickupItem(Aisling aisling, GroundItem groundItem) => aisling.IsAlive;

    public virtual bool CanPickupMoney(Aisling aisling, Money money) => aisling.IsAlive;

    public virtual bool CanTalk(Creature creature) => creature.IsAlive;

    public virtual bool CanTurn(Creature creature) => creature.IsAlive;

    public virtual bool CanUseItem(Aisling aisling, Item item) => aisling.IsAlive;

    public virtual bool CanUseSkill(Creature creature, Skill skill) => creature.IsAlive;

    public virtual bool CanUseSpell(Creature creature, Spell spell) => creature.IsAlive;
}