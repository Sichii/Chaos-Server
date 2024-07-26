using Chaos.Models.Panel;
using Chaos.Models.World;
using Chaos.Models.World.Abstractions;

namespace Chaos.Scripting.Behaviors;

public class RestrictionBehavior
{
    public virtual bool CanMove(Creature creature) => !creature.Effects.Contains("skulled");

    public virtual bool CanTalk(Creature creature) => creature.IsAlive;

    public virtual bool CanTurn(Creature creature) => !creature.Effects.Contains("skulled");

    public virtual bool CanUseItem(Aisling aisling, Item item) => aisling.IsAlive;

    public virtual bool CanUseSkill(Creature creature, Skill skill) => creature.IsAlive;

    public virtual bool CanUseSpell(Creature creature, Spell spell) => creature.IsAlive;
}