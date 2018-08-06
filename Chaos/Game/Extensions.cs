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

        internal List<Creature> GetTargetsFromType(Client client, Point targetPoint, TargetsType type = TargetsType.None)
        {
            Creature creature = null;
            List<Creature> creatures = new List<Creature>();

            switch (type)
            {
                //generally skill types
                case TargetsType.None:
                    break;
                case TargetsType.Self:
                    creatures.Add(client.User);
                    break;
                case TargetsType.Front:
                    creature = Game.World.ObjectsVisibleFrom(client.User).OfType<Creature>().FirstOrDefault(c => c.Point == client.User.Point.NewOffset(client.User.Direction));
                    if (creature != null)
                        creatures.Add(creature);
                    break;
                case TargetsType.Surround:
                    creatures.AddRange(Game.World.ObjectsVisibleFrom(client.User, false, 1).OfType<Creature>());
                    break;
                case TargetsType.Cleave:
                    creatures.AddRange(Game.World.ObjectsVisibleFrom(client.User, false, 2).OfType<Creature>().Where(c =>
                        (c.Point.Distance(client.User.Point) == 1 && c.Point.Relation(client.User.Point) != DirectionExtensions.Reverse(client.User.Direction)) || 
                        client.User.DiagonalPoints(1, client.User.Direction).Contains(c.Point)));
                    break;
                case TargetsType.StraightProjectile:
                    int distance = 13;
                    List<Point> line = client.User.LinePoints(13, client.User.Direction);

                    foreach (Creature c in Game.World.ObjectsVisibleFrom(client.User))
                    {
                        if (line.Contains(c.Point))
                        {
                            int dist = c.Point.Distance(client.User.Point);

                            if (dist < distance)
                            {
                                distance = dist;
                                creature = c;
                            }
                        }
                    }

                    if (creature != null)
                        creatures.Add(creature);
                    break;




                //generally spell types
                case TargetsType.Cluster1:
                    creatures.AddRange(Game.World.ObjectsVisibleFrom(targetPoint, client.User.Map, true, 1).OfType<Creature>());
                    break;
                case TargetsType.Cluster2:
                    creatures.AddRange(Game.World.ObjectsVisibleFrom(targetPoint, client.User.Map, true, 2).OfType<Creature>());
                    break;
                case TargetsType.Cluster3:
                    creatures.AddRange(Game.World.ObjectsVisibleFrom(targetPoint, client.User.Map, true, 3).OfType<Creature>());
                    break;
                case TargetsType.Screen:
                    creatures.AddRange(Game.World.ObjectsVisibleFrom(targetPoint, client.User.Map, true).OfType<Creature>());
                    break;
            }

            return creatures;
        }

        internal void ApplyEffect(Client client, PanelObject obj, List<Creature> targets, List<Animation> sfx, bool displayHealth = false, bool refreshClient = false, bool refreshTargets = false)
        {
            //grab all nearby Users
            List<User> nearbyUsers = World.ObjectsVisibleFrom(client.User, true).OfType<User>().ToList();
            bool isSkill = false;
            //send the skill cooldown to the skill user
            if (obj is Skill)
                isSkill = true;

            client.Enqueue(ServerPackets.Cooldown(isSkill, obj.Slot, (uint)obj.Cooldown.TotalSeconds));

            if (targets == null)
                targets = new List<Creature>();

            //refresh the client if needed
            if (refreshClient)
                Game.World.Refresh(client);

            //refresh targets if needed
            if (refreshTargets)
                foreach (User u in targets.OfType<User>())
                    Game.World.Refresh(u.Client);

            //send all nearby clients the bodyanimation
            if (obj.BodyAnimation != BodyAnimation.None)
                foreach (User u in nearbyUsers)
                    u.Client.Enqueue(ServerPackets.AnimateCreature(client.User.Id, obj.BodyAnimation, 25));

            //for each target
            foreach(Creature c in targets)
            {
                //get all users they can see including self
                List<User> usersNearC = Game.World.ObjectsVisibleFrom(c, true).OfType<User>().ToList();

                //if animation should be displayed
                if (obj.EffectAnimation != Animation.None)
                {
                    //create new animation for this target
                    Animation newAnimation = new Animation(obj.EffectAnimation, c.Id, client.User.Id);

                    //send this animation to all visible users
                    foreach (User u in usersNearC)
                        u.Client.Enqueue(ServerPackets.Animation(newAnimation));
                }

                //if health should be displayed
                if (displayHealth)
                {
                    //update the targets vitality if theyre a user
                    (c as User)?.Client.SendAttributes(StatUpdateType.Vitality);

                    //send all visible users the target's healthbar
                    foreach (User u in usersNearC)
                        u.Client.Enqueue(ServerPackets.HealthBar(c));
                }
            }

            //if there are any additional special effects with the spell, display them to all users who should be able to see them
            if (sfx != null)
                foreach (Animation ani in sfx)
                    foreach (User u in Game.World.ObjectsVisibleFrom(ani.TargetPoint, client.User.Map, true).OfType<User>())
                        u.Client.Enqueue(ServerPackets.Animation(ani));
            return;
        }

        internal void ApplyDamage(Creature obj, int amount)
        {
            //ac, damage, other shit

            obj.CurrentHP = Utility.Clamp<uint>((int)(obj.CurrentHP - amount), 0, (int)obj.MaximumHP);
        }
    }
}