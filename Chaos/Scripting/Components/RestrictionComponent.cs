using Chaos.Objects.Panel;
using Chaos.Objects.World;
using Chaos.Objects.World.Abstractions;

namespace Chaos.Scripting.Components;

public class RestrictionComponent
{
    public virtual bool CanMove(Creature creature) => creature.IsAlive;

    public virtual bool CanTalk(Creature creature) => creature.IsAlive;

    public virtual bool CanTurn(Creature creature) => creature.IsAlive;

    public virtual bool CanUseItem(Aisling aisling, Item item) => aisling.IsAlive;

    public virtual bool CanUseSkill(Creature creature, Skill skill) => creature.IsAlive;

    public virtual bool CanUseSpell(Creature creature, Spell spell) => creature.IsAlive;
}