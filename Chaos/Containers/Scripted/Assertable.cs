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
using System.Threading.Tasks;

namespace Chaos                                                                                                                                                                          
{
    #pragma warning disable IDE0022
    internal sealed class Assertables
    {
        private readonly Server Server;
        private readonly World World;
        private readonly Task StatusController;
        private readonly Task EffectController;

        internal Assertables(Server server, World world)
        {
            Server = server;
            World = world;

            Server.WriteLogAsync("Starting game controllers...");
            StatusController = Task.Run(AssertStatusesAsync);
            EffectController = Task.Run(AssertEffectsAsync);
        }

        ~Assertables()
        {
            StatusController.Dispose();
            EffectController.Dispose();
        }

        /// <summary>
        /// Game thread. Checks user and monster states, and applies them as needed.
        /// </summary>
        private async Task AssertStatusesAsync()
        {
            var rate = new RateController(10);

            while (Server.Running)
            {
                foreach (KeyValuePair<ushort, Map> kvp in World.Maps)
                    foreach (Creature creature in kvp.Value.GetLockedInstance<Creature>())
                    {
                        if (creature.Type == CreatureType.Merchant)
                            continue;

                        if (!creature.IsAlive)
                            KillCreature(creature);
                    }

                await rate.ThrottleAsync();
            }
        }

        /// <summary>
        /// Game thread. Applies persistent effect on creatures and maps.
        /// </summary>
        private async Task AssertEffectsAsync()
        {
            var rate = new RateController(10);

            while (Server.Running)
            {
                //for each map
                foreach (Map map in World.Maps.Values)
                    map.ApplyPersistantEffects();

                await rate.ThrottleAsync();
            }
        }

        /// <summary>
        /// Applies death to a creature.
        /// </summary>
        /// <param name="creature"></param>
        internal void KillCreature(Creature creature)
        {
            if (creature.HasFlag(Status.Dead))
                return;

            creature.AddFlag(Status.Dead);
            creature.EffectsBar.Clear();


            //distribute exp?
            if (creature is User tUser)
                Warp(tUser, Chaos.Warp.Death(tUser));
            else
                creature.Map.RemoveObject(creature);
        }

        /// <summary>
        /// Revives a user, restoring their hp, and mp.
        /// </summary>
        /// <param name="user">The user to revive.</param>
        internal void ReviveUser(User user)
        {
            if (user.HasFlag(Status.Dead))
            {
                user.Attributes.CurrentHP = user.Attributes.MaximumHP;
                user.Attributes.CurrentMP = user.Attributes.MaximumMP;
                user.RemoveFlag(Status.Dead);
                user.Client.Refresh(true);
            }
        }

        /// <summary>
        /// Updates the targets and nearby users of each target to reflect the current state of each changed object.
        /// </summary>
        /// <param name="client">The client source of the changes.</param>
        /// <param name="obj">The object effect causing the change.</param>
        /// <param name="targets">The targets of the effect.</param>
        /// <param name="sfx">Special effects, generally point based animations.</param>
        /// <param name="updateType">Stat upate to send to the targets. No need to use vitality.</param>
        /// <param name="displayHealth">Whether or not to display health for the targets.</param>
        /// <param name="refreshClient">Whether or not to refresh the source of change.</param>
        /// <param name="refreshTargets">Whether or not to refresh the targets.</param>
        internal void Activate(Client client, PanelObject obj, List<Creature> targets, List<Animation> sfx, StatUpdateType updateType, bool displayHealth = false, bool refreshClient = false, bool refreshTargets = false)
        {
            if (targets.Count == 0)
                return;

            lock (obj)
            {
                obj.LastUse = DateTime.UtcNow;
                //grab all nearby Users
                var nearbyUsers = client.User.Map.ObjectsVisibleFrom(client.User.Point, true).OfType<User>().ToList();
                //send the skill cooldown to the skill user
                client.Enqueue(ServerPackets.Cooldown(obj));

                //refresh the client if needed
                if (refreshClient)
                    client.Refresh(true);

                //refresh targets if needed
                foreach (User u in targets.OfType<User>())
                {
                    if (refreshTargets)
                        u.Client.Refresh(true);
                    else if (updateType != StatUpdateType.None)
                        u.Client.SendAttributes(updateType);

                }

                //send all nearby clients the bodyanimation
                if (obj.BodyAnimation != BodyAnimation.None)
                    foreach (User u in nearbyUsers)
                        u.Client.Enqueue(ServerPackets.AnimateCreature(client.User.ID, obj.BodyAnimation, 25));

                //for each target
                foreach (Creature c in targets)
                {
                    //get all users they can see including self
                    var usersNearC = client.User.Map.ObjectsVisibleFrom(c.Point, true).OfType<User>().ToList();

                    //if animation should be displayed
                    if (obj.Animation != Animation.None)
                    {
                        //create new animation for this target
                        Animation newAnimation = obj.Animation.GetTargetedAnimation(c.ID, client.User.ID);
                        c.AnimationHistory[newAnimation.GetHashCode()] = DateTime.UtcNow;
                        //send this animation to all visible users
                        foreach (User u in usersNearC)
                        {
                            u.Client.SendAnimation(newAnimation);
                        }
                    }

                    //if health should be displayed
                    if (displayHealth)
                    {
                        //send all visible users the target's healthbar
                        foreach (User u in usersNearC)
                            u.Client.Enqueue(ServerPackets.HealthBar(c));
                    }
                }

                //if there are any additional special effects with the spell, display them to all users who should be able to see them
                if (sfx != null)
                    foreach (Animation ani in sfx)
                        foreach (User u in client.User.Map.ObjectsVisibleFrom(ani.TargetPoint, true).OfType<User>())
                            u.Client.SendAnimation(ani);
            }
            return;
        }

        /// <summary>
        /// Applies an amount of damage to a creature. Calculates defenses, stats, and other things. Changes the creature's alive state if necessary.
        /// </summary>
        /// <param name="obj">The creature to apply the damage to.</param>
        /// <param name="amount">The flat amount of damage to apply.</param>
        /// <param name="ignoreDefense">Whether to ignore defenses or not.</param>
        /// <param name="mana">Whather the damage dealt is to mana or not.</param>
        internal void Damage(Creature obj, int amount, bool ignoreDefense = false, bool mana = false)
        {
            User user = null;

            if ((user = obj as User) == null && mana)
                return;

            lock (obj)
            {
                //ac, damage, other shit
                //damage additions based on stats will be moved here later probably
                if (mana)
                    user.CurrentMP = Utilities.Clamp<uint>(user.CurrentMP - amount, 0, (int)user.MaximumMP);
                else
                    obj.CurrentHP = Utilities.Clamp<uint>(obj.CurrentHP - amount, 0, (int)obj.MaximumHP);

                if (user != null)
                    user.Client.SendAttributes(StatUpdateType.Vitality);
            }
        }

        /// <summary>
        /// Moves a user from one location to another.
        /// </summary>
        /// <param name="obj">The user to warp from and to a position/map.</param>
        /// <param name="warp">The warp the user is using to warp.</param>
        /// <param name="unsourced">Whether or not they are using a worldMap to warp.</param>
        internal void Warp(VisibleObject obj, Warp warp)
        {
            if (World.Maps.TryGetValue(warp.TargetLocation.MapID, out Map targetMap))
            {
                bool unsourced = warp.Location == Location.None;

                if (warp.Location == obj.Location || unsourced)
                {
                    if ((obj as User)?.IsAdmin != true && targetMap.IsWall(warp.TargetPoint))
                    {
                        Point nearestPoint = Point.None;
                        int distance = int.MaxValue;
                        ushort x = Utilities.Clamp<ushort>(warp.TargetLocation.Point.X - 25, 0, targetMap.SizeX);
                        int width = Math.Min(x + 50, targetMap.SizeX);
                        ushort y = Utilities.Clamp<ushort>(warp.TargetLocation.Point.Y - 25, 0, targetMap.SizeY);
                        int height = Math.Min(y + 50, targetMap.SizeY);

                        //search up to 2500 tiles for a non wall
                        for (; x < width; x++)
                            for (; y < height; y++)
                            {
                                Point newPoint = targetMap[x, y];
                                if (!targetMap.IsWall(newPoint))
                                {
                                    distance = warp.TargetLocation.Point.Distance(newPoint);
                                    nearestPoint = newPoint;
                                }
                            }

                        warp = new Warp(warp.Location, (warp.TargetLocation.MapID, nearestPoint));
                    }

                    if (!unsourced)
                        obj.Map.RemoveObject(obj);

                    World.Maps[warp.TargetLocation.MapID].AddObject(obj, warp.TargetLocation.Point);
                }
            }
        }
    }
}