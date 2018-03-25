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
                default:
                    return (Creature c) => { return false; };
            }
        }

        internal void ApplySpell(Client client, int amount, Spell spell, params Creature[] targets)
        {
            foreach (var target in targets)
            {
                Animation animation = new Animation(spell.EffectAnimation, target.Id, client.User.Id);

                target.CurrentHP = Utility.Clamp<uint>((int)(target.CurrentHP + amount), 0, (int)target.MaximumHP);

                User user = target as User;
                user?.Client.Enqueue(ServerPackets.Animation(animation));
                user?.Client.Enqueue(ServerPackets.HealthBar(target));
                user?.Client.Enqueue(ServerPackets.AnimateCreature(animation.SourceId, spell.BodyAnimation, 25));
                user?.Client.SendAttributes(StatUpdateType.Vitality);

                foreach (User u in World.ObjectsVisibleFrom(target).OfType<User>().Except(targets.OfType<User>()))
                {
                    u.Client.Enqueue(ServerPackets.Animation(animation));
                    u.Client.Enqueue(ServerPackets.AnimateCreature(animation.SourceId, spell.BodyAnimation, 25));
                    u.Client.Enqueue(ServerPackets.HealthBar(target));
                }
            }
            client.Enqueue(ServerPackets.Cooldown(false, spell.Slot, (uint)spell.Cooldown.TotalSeconds));
        }

        internal void ApplySkill(Client client, int amount, Skill skill)
        {
            List<Creature> nearbyCreatures = World.ObjectsVisibleFrom(client.User).OfType<Creature>().ToList();

            client.Enqueue(ServerPackets.AnimateCreature(client.User.Id, skill.BodyAnimation, 25));
            foreach (var user in nearbyCreatures.OfType<User>())
                user.Client.Enqueue(ServerPackets.AnimateCreature(client.User.Id, skill.BodyAnimation, 25));

            List<Creature> targets = nearbyCreatures.Where(SkillTargets(client, skill.Type)).ToList();

            foreach (Creature c in targets)
            {
                c.CurrentHP = Utility.Clamp<uint>((int)(c.CurrentHP + amount), 0, (int)c.MaximumHP);
                Animation animation = new Animation(skill.EffectAnimation, c.Id, client.User.Id);

                User user = c as User;
                user?.Client.Enqueue(ServerPackets.Animation(new Animation(animation, c.Id, client.User.Id)));
                user?.Client.Enqueue(ServerPackets.HealthBar(c));
                user?.Client.SendAttributes(StatUpdateType.Vitality);

                foreach (var u in World.ObjectsVisibleFrom(c).OfType<User>())
                {
                    u.Client.Enqueue(ServerPackets.Animation(new Animation(animation, c.Id, client.User.Id)));
                    u.Client.Enqueue(ServerPackets.HealthBar(c));
                }
            }

            client.Enqueue(ServerPackets.Cooldown(true, skill.Slot, (uint)skill.Cooldown.TotalSeconds));
        }
    }
}