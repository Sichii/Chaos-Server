// ****************************************************************************
// This file belongs to the Chaos-Server project.
// 
// This project is free and open-source, provided that any alterations or
// modifications to any portions of this project adhere to the
// Affero General Public License (Version 3).
// 
// A copy of the AGPLv3 can be found in the project directory.
// You may also find a copy at <https://www.gnu.org/licenses/agpl-3.0.html>
// ****************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;

namespace Chaos
{
    internal class Extensions
    {
        private Server Server { get; set; }
        private World World { get; set; }

        internal Extensions(Server server, World world)
        {
            Server = server;
            World = world;
        }

        internal void KillUser(Client client, User user)
        {
            user.Attributes.CurrentHP = 0;
            user.IsAlive = false;
            //strip buffs
            //disable casting
            //death stuff

            World.Refresh(client, true);
        }

        internal void Revive(Client client, Server server, object args)
        {
            if (!client.User.IsAlive)
            {
                client.User.Attributes.CurrentHP = client.User.Attributes.MaximumHP;
                client.User.Attributes.CurrentMP = client.User.Attributes.MaximumMP;
                client.User.IsAlive = true;
                Game.World.Refresh(client, true);
            }
        }

        internal void ReviveUser(Client client, User user)
        {
            user.Attributes.CurrentHP = user.Attributes.MaximumHP;
            user.Attributes.CurrentMP = user.Attributes.MaximumMP;
            user.IsAlive = true;
            Game.World.Refresh(client, true);
        }

        internal Func<Creature, bool> SkillTargets(Client client, SkillType type)
        {
            switch (type)
            {
                case SkillType.Self:
                case SkillType.Effect:
                    return (Creature c) => { return false; };
                case SkillType.Front:
                    return (Creature c) => { return c.Point == client.User.Point.Offsetter(client.User.Direction); };
                case SkillType.Surround:
                    return (Creature c) => { return c.Point.Distance(client.User.Point) == 1; };
                case SkillType.Cleave:
                    return (Creature c) => { return (c.Point.Distance(client.User.Point) == 1 && c.Point.Relation(client.User.Point) != DirectionExtensions.Reverse(client.User.Direction)) || client.User.DiagonalPoints(1, client.User.Direction).Contains(c.Point); };
                default:
                    return (Creature c) => { return false; };
            }
        }

        internal void ApplySpell(Client client, int amount, Spell spell, params Creature[] targets)
        {
            //send the spell cooldown to the spell user
            client.Enqueue(ServerPackets.Cooldown(false, spell.Slot, (uint)spell.Cooldown.TotalSeconds));
            //send the spell user and all nearby users the bodyanimation
            client.Enqueue(ServerPackets.AnimateCreature(client.User.Id, spell.BodyAnimation, 25));
            foreach (User u in World.ObjectsVisibleFrom(client.User).OfType<User>())
                u.Client.Enqueue(ServerPackets.AnimateCreature(client.User.Id, spell.BodyAnimation, 25));

            //for each target of the spell
            foreach (Creature target in targets)
            {
                //apply the damage and grab the animation
                ApplyDamage(target, amount);
                Animation animation = new Animation(spell.EffectAnimation, target.Id, client.User.Id);

                //if the target is a user, send them their own updated information
                User user = target as User;
                user?.Client.Enqueue(ServerPackets.Animation(animation));
                user?.Client.Enqueue(ServerPackets.HealthBar(target));
                user?.Client.SendAttributes(StatUpdateType.Vitality);

                //send all nearby users this updated information as well
                foreach (User u in World.ObjectsVisibleFrom(target).OfType<User>())
                {
                    u.Client.Enqueue(ServerPackets.Animation(animation));
                    u.Client.Enqueue(ServerPackets.HealthBar(target));
                }
            }
        }

        internal void ApplySkill(Client client, int amount, Skill skill)
        {
            //grab all nearby creatures
            List<Creature> nearbyCreatures = World.ObjectsVisibleFrom(client.User).OfType<Creature>().ToList();

            //send the skill cooldown to the skill user
            client.Enqueue(ServerPackets.Cooldown(true, skill.Slot, (uint)skill.Cooldown.TotalSeconds));
            //send the skill user and all nearby users the bodyanimation
            client.Enqueue(ServerPackets.AnimateCreature(client.User.Id, skill.BodyAnimation, 25));
            foreach (User u in nearbyCreatures.OfType<User>())
                u.Client.Enqueue(ServerPackets.AnimateCreature(client.User.Id, skill.BodyAnimation, 25));

            //grab targets of skill based on function from skill type
            List<Creature> targets = nearbyCreatures.Where(SkillTargets(client, skill.Type)).ToList();

            //for each target of the skill
            foreach (Creature target in targets)
            {
                //apply the damage and grab the animation
                ApplyDamage(target, amount);
                Animation animation = new Animation(skill.EffectAnimation, target.Id, client.User.Id);

                //if the target is a user, send them their own updated information
                User user = target as User;
                user?.Client.Enqueue(ServerPackets.Animation(animation));
                user?.Client.Enqueue(ServerPackets.HealthBar(target));
                user?.Client.SendAttributes(StatUpdateType.Vitality);

                //send all nearby users this updated information as well
                foreach (User u in World.ObjectsVisibleFrom(target).OfType<User>())
                {
                    u.Client.Enqueue(ServerPackets.Animation(animation));
                    u.Client.Enqueue(ServerPackets.HealthBar(target));
                }
            }
        }

        internal void ApplyDamage(Creature obj, int amount)
        {
            //ac, damage, other shit

            obj.CurrentHP = Utility.Clamp<uint>((int)(obj.CurrentHP + amount), 0, (int)obj.MaximumHP);
        }
    }
}