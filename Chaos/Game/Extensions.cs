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

        /// <summary>
        /// Death effects for a user, strips all buffs, disables casting, maybe drops items, etc?
        /// </summary>
        /// <param name="client"></param>
        /// <param name="user"></param>
        internal void KillUser(User user)
        {
            //strip buffs
            //disable casting
            //death stuff

            user.DeathDisplayed = true;
            Game.Extensions.WarpObj(user, new Warp(user.Location, CONSTANTS.DEATH_LOCATION));
        }

        internal void KillMonster(Monster monster)
        {
            //distribute exp
            //other things?

            //remove from screen
            World.RemoveObjectFromMap(monster);
        }

        /// <summary>
        /// Revives a user, restoring their hp, mp and alive state.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="user"></param>
        internal void ReviveUser(User user)
        {
            user.Attributes.CurrentHP = user.Attributes.MaximumHP;
            user.Attributes.CurrentMP = user.Attributes.MaximumMP;
            user.DeathDisplayed = false;
            Refresh(user.Client, true);
        }

        /// <summary>
        /// Gets targets based on the target type of the effect.
        /// </summary>
        /// <param name="client">The client source of the effect.</param>
        /// <param name="targetPoint">The target point of the effect.</param>
        /// <param name="type">The target type to base the return on.</param>
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
                    if(World.TryGetObject(client.User.Point.NewOffset(client.User.Direction), out creature, client.User.Map))
                        creatures.Add(creature);
                    break;
                case TargetsType.Surround:
                    creatures.AddRange(World.ObjectsVisibleFrom(client.User, false, 1).OfType<Creature>());
                    break;
                case TargetsType.Cleave:
                    creatures.AddRange(World.ObjectsVisibleFrom(client.User, false, 2).OfType<Creature>().Where(c =>
                        (c.Point.Distance(client.User.Point) == 1 && c.Point.Relation(client.User.Point) != DirectionExtensions.Reverse(client.User.Direction)) || 
                        client.User.DiagonalPoints(1, client.User.Direction).Contains(c.Point)));
                    break;
                case TargetsType.StraightProjectile:
                    int distance = 13;
                    List<Point> line = client.User.LinePoints(13, client.User.Direction);

                    foreach (Creature c in World.ObjectsVisibleFrom(client.User))
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
                    creatures.AddRange(World.ObjectsVisibleFrom(targetPoint, client.User.Map, true, 1).OfType<Creature>());
                    break;
                case TargetsType.Cluster2:
                    creatures.AddRange(World.ObjectsVisibleFrom(targetPoint, client.User.Map, true, 2).OfType<Creature>());
                    break;
                case TargetsType.Cluster3:
                    creatures.AddRange(World.ObjectsVisibleFrom(targetPoint, client.User.Map, true, 3).OfType<Creature>());
                    break;
                case TargetsType.Screen:
                    creatures.AddRange(World.ObjectsVisibleFrom(targetPoint, client.User.Map, true).OfType<Creature>());
                    break;
            }

            return creatures.Where(c => c.IsAlive).ToList();
        }

        /// <summary>
        /// Updates the targets and nearby uses of each target to reflect the current state of each changed object.
        /// </summary>
        /// <param name="client">The client source of the changes.</param>
        /// <param name="obj">The object effect causing the change.</param>
        /// <param name="targets">The targets of the effect.</param>
        /// <param name="sfx">Special effects, generally point based animations.</param>
        /// <param name="displayHealth">Whether or not to display health for the targets.</param>
        /// <param name="refreshClient">Whether or not to refresh the source of change.</param>
        /// <param name="refreshTargets">Whether or not to refresh the targets.</param>
        internal void ApplyEffect(Client client, PanelObject obj, List<Creature> targets, List<Animation> sfx, bool displayHealth = false, bool refreshClient = false, bool refreshTargets = false)
        {
            lock (obj)
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
                else
                    targets = targets.Where(c => c.IsAlive).ToList();

                //refresh the client if needed
                if (refreshClient)
                    Refresh(client);

                //refresh targets if needed
                if (refreshTargets)
                    foreach (User u in targets.OfType<User>())
                        Refresh(u.Client);

                //send all nearby clients the bodyanimation
                if (obj.BodyAnimation != BodyAnimation.None)
                    foreach (User u in nearbyUsers)
                        u.Client.Enqueue(ServerPackets.AnimateCreature(client.User.Id, obj.BodyAnimation, 25));

                //for each target
                foreach (Creature c in targets)
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
            }
            return;
        }

        /// <summary>
        /// Applies an amount of damage to a creature obj. Calculates defenses, stats, and other things. Changes the creature's alive state if necessary.
        /// </summary>
        /// <param name="obj">The creature to apply the damage to.</param>
        /// <param name="amount">The flat amount of damage to apply.</param>
        /// <param name="ignoreDefense">Whether to ignore defenses or not.</param>
        internal void ApplyDamage(Creature obj, int amount, bool ignoreDefense = false)
        {
            lock (obj)
            {
                //ac, damage, other shit
                //damage additions based on stats will be moved here later probably

                obj.CurrentHP = Utility.Clamp<uint>((int)(obj.CurrentHP - amount), 0, (int)obj.MaximumHP);
            }
        }

       

        /// <summary>
        /// Moves a user from one map to another
        /// </summary>
        /// <param name="obj">The user to warp from and to a position/map.</param>
        /// <param name="warp">The warp the user is using to warp.</param>
        /// <param name="worldMap">Whether or not they are using a worldMap to warp.</param>
        internal void WarpObj(VisibleObject obj, Warp warp, bool worldMap = false)
        {
            if (warp.Location == obj.Location && World.Maps.ContainsKey(warp.TargetMapId))
            {
                if ((obj as User)?.IsAdmin != true && World.Maps[warp.TargetMapId].IsWall(warp.TargetPoint))
                {
                    Point nearestPoint = new Point(ushort.MaxValue, ushort.MaxValue);
                    int distance = int.MaxValue;
                    ushort x = Utility.Clamp<ushort>(warp.TargetPoint.X - 25, 0, World.Maps[warp.TargetMapId].SizeX);
                    int width = Math.Min(x + 50, World.Maps[warp.TargetMapId].SizeX);
                    ushort y = Utility.Clamp<ushort>(warp.TargetPoint.Y - 25, 0, World.Maps[warp.TargetMapId].SizeY);
                    int height = Math.Min(y + 50, World.Maps[warp.TargetMapId].SizeY);

                    //search up to 2500 tiles for a non wall
                    for (; x < width; x++)
                        for (; y < height; y++)
                        {
                            Point newPoint = new Point(x, y);
                            if (!World.Maps[warp.TargetMapId].IsWall(newPoint))
                            {
                                distance = warp.TargetPoint.Distance(newPoint);
                                nearestPoint = newPoint;
                            }
                        }

                    warp = new Warp(warp.Location, new Location(warp.TargetMapId, nearestPoint));
                }

                if (!worldMap)
                    World.RemoveObjectFromMap(obj);

                World.AddObjectToMap(obj, warp.TargetLocation);
            }
        }

        /// <summary>
        /// Resends all the current information for the given user.
        /// </summary>
        /// <param name="user">The user to refresh.</param>
        internal void Refresh(Client client, bool byPassTimer = false)
        {
            if (client == null)
                return;

            if (!byPassTimer && DateTime.UtcNow.Subtract(client.LastRefresh).TotalMilliseconds < CONSTANTS.REFRESH_DELAY_MS)
                return;
            else
                client.LastRefresh = DateTime.UtcNow;

            lock (client.User.Map.Sync)
            {
                client.Enqueue(ServerPackets.MapInfo(client.User.Map));
                client.Enqueue(ServerPackets.Location(client.User.Point));
                client.SendAttributes(StatUpdateType.Full);
                List<VisibleObject> itemMonsterToSend = new List<VisibleObject>();

                //get all objects that would be visible to this object and send this user to them / send them to this user
                foreach (VisibleObject obj in World.ObjectsVisibleFrom(client.User))
                    if (obj is User)
                    {
                        User user = obj as User;
                        client.Enqueue(ServerPackets.DisplayUser(user));
                        user.Client.Enqueue(ServerPackets.DisplayUser(client.User));
                    }
                    else
                        itemMonsterToSend.Add(obj);

                client.Enqueue(ServerPackets.DisplayItemMonster(itemMonsterToSend.ToArray()));
                client.Enqueue(ServerPackets.MapLoadComplete());
                client.Enqueue(ServerPackets.DisplayUser(client.User));
                client.Enqueue(ServerPackets.RefreshResponse());
            }
        }
    }
}