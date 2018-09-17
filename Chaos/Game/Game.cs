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
using System.Linq;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;

namespace Chaos
{
    /// <summary>
    /// Object representing the game. API. Is the medium between the networking interface and clients. Contains container objects that contain game objects, as well as the world.
    /// </summary>
#pragma warning disable IDE0022
    internal static class Game
    {
        private static readonly object Sync = new object();
        internal static CreationEngine CreationEngine { get; set; }
        internal static Merchants Merchants { get; set; }
        internal static Dialogs Dialogs { get; set; }
        internal static Server Server { get; set; }
        internal static World World { get; set; }
        internal static Activatables Activatables { get; set; }

        /// <summary>
        /// Pseudo-constructor for the server. Prepares the world for population.
        /// </summary>
        internal static void Set(Server server)
        {
            Server.WriteLog("Initializing game...");

            Server = server;
            World = new World(Server);
            World.Load();
            CreationEngine = new CreationEngine();
            Merchants = new Merchants();
            Dialogs = new Dialogs();
            Activatables = new Activatables(Server, World);
            World.Populate();

            Task.Factory.StartNew(AssertEffectsAsync, TaskCreationOptions.LongRunning);
            Task.Factory.StartNew(AssertStatesAsync, TaskCreationOptions.LongRunning);
        }

        /// <summary>
        /// Game thread. Checks user and monster states, and applies them as needed.
        /// </summary>
        private static async void AssertStatesAsync()
        {
            while (Server.Running)
            {
                long oldTime = Utility.CurrentTicks;

                lock (Server.Sync)
                {
                    foreach (User user in Server.WorldClients.Select(c => c.User))
                        if (!user.IsAlive && !user.HasFlag(UserState.DeathDisplayed))
                            Activatables.KillUser(user);

                    foreach (Map map in World.Maps.Values)
                        foreach (Monster monster in map.GetAllObjects<Monster>())
                            if (!monster.IsAlive)
                                Activatables.KillMonster(monster);
                }

                await Task.Delay(Utility.ClampedDeltaTime(oldTime, 100));
            }
        }

        /// <summary>
        /// Game thread. Applies persistent effect on creatures and maps.
        /// </summary>
        private static async void AssertEffectsAsync()
        {
            while (Server.Running)
            {
                long oldTime = Utility.CurrentTicks;
                //for each map
                foreach (Map map in World.Maps.Values)
                    map.ApplyPersistantEffects();

                await Task.Delay(Utility.ClampedDeltaTime(oldTime, 100));
            }
        }

        #region Packet Processing
        internal static void RequestConnectionInfo(Client client)
        {
            client.Enqueue(ServerPackets.ConnectionInfo(Server.TableCheckSum, client.Crypto.Seed, client.Crypto.Key));
        }

        internal static void CreateChar1(Client client, string name, string password)
        {
            //checks if the name is 4-12 characters straight, if not... checks if there's a string 7-12 units long that has a space surrounced by at least 3 chars on each side.
            if (!Regex.Match(name, @"(:?^[a-zA-Z]{4,}$|[a-zA-Z]{3,} ?[a-zA-Z]{3,})").Success || name.Length > 12)
                client.SendLoginMessage(LoginMessageType.ClearNameMessage, "Name must be 4-12 characters long, or a space surrounded by at least 3 characters on each side, up to 12 total.");
            //checks if the password is 4-8 units long
            else if (!Regex.Match(password, @".{4,8}").Success)
                client.SendLoginMessage(LoginMessageType.ClearPswdMessage, "Password must be 4-8 units long.");
            //check if a user already exists with the given valid name
            else if(Server.DataBase.UserExists(name))
                client.SendLoginMessage(LoginMessageType.ClearNameMessage, "That name is taken.");
            else
            {   //otherwise set the client's new character fields so CreateChar1 can use the information and send a confirmation to the client
                client.CreateCharName = name;
                client.CreateCharPw = password;
                client.SendLoginMessage(LoginMessageType.Confirm);
            }
        }

        internal static void Login(Client client, string name, string password)
        {
            //checks the userhash to see if the given name and password exist
            if (!Server.DataBase.CheckHash(name, Crypto.GetMD5Hash(password)))
                client.SendLoginMessage(LoginMessageType.ClearNameMessage, "Incorrect user name or password.");
            //checks to see if the user is currently logged on
            else if (Server.TryGetUser(name, out User user))
            {
                client.SendLoginMessage(LoginMessageType.ClearPswdMessage, "That character is already logged in.");
                user.Client.Disconnect();
            }
            else
            {   //otherwise, confirms the login, sends the login message, and redirects them to the world
                client.SendLoginMessage(LoginMessageType.Confirm);
                client.SendServerMessage(ServerMessageType.ActiveMessage, "Logging in to Chaos");
                client.Redirect(new Redirect(client, ServerType.World, name));
            }
        }

        internal static void CreateChar2(Client client, byte hairStyle, Gender gender, byte hairColor)
        {
            //if either is null, return
            if (string.IsNullOrEmpty(client.CreateCharName) || string.IsNullOrEmpty(client.CreateCharPw))
                return;

            //check the data given
            hairStyle = (byte)(hairStyle < 1 ? 1 : hairStyle > 17 ? 17 : hairStyle);
            hairColor = (byte)(hairColor > 13 ? 13 : hairColor < 0 ? 0 : hairColor);
            gender = gender != Gender.Male && gender != Gender.Female ? Gender.Male : gender;

            //create a new user, and it's display data
            var newUser = new User(client.CreateCharName, CONSTANTS.STARTING_LOCATION.Point, World.Maps[CONSTANTS.STARTING_LOCATION.MapId], Direction.South, gender);
            var data = new DisplayData(newUser, hairStyle, hairColor, (BodySprite)((byte)gender * 16))
            {
                User = newUser
            };
            newUser.DisplayData = data;
            //if the user is an admin character, apply godmode
            if (Server.Admins.Contains(newUser.Name, StringComparer.CurrentCultureIgnoreCase))
            {
                newUser.SpellBook.AddToNextSlot(CreationEngine.CreateSpell("Admin Create"));
                newUser.Inventory.AddToNextSlot(CreationEngine.CreateItem("Admin Trinket"));
                newUser.Legend.Add(new LegendMark(DateTime.UtcNow, "I'm a fuckin bawss", "gm", MarkIcon.Yay, MarkColor.Yellow));

                newUser.IsAdmin = true;
                newUser.Attributes.BaseHP = 1333337;
                newUser.Attributes.BaseMP = 1333337;
                newUser.Attributes.CurrentHP = 1333337;
                newUser.Attributes.CurrentMP = 1333337;
                newUser.Attributes.BaseStr = 255;
                newUser.Attributes.BaseInt = 255;
                newUser.Attributes.BaseWis = 255;
                newUser.Attributes.BaseCon = 255;
                newUser.Attributes.BaseDex = 255;
                newUser.Titles.Add("Game Master");
                newUser.IsMaster = true;
                newUser.BaseClass = BaseClass.Admin;
                newUser.Nation = Nation.Noes;
                newUser.Guild = World.Guilds[CONSTANTS.DEVELOPER_GUILD_NAME];

                newUser.Attributes.Gold += 500000000;
            }

            //try to save the new user to the database
            if (Server.DataBase.TryAddUser(newUser, client.CreateCharPw))
                client.SendLoginMessage(LoginMessageType.Confirm);
            else
                client.SendLoginMessage(LoginMessageType.ClearNameMessage, "Unable to create character. Name is already taken.");
        }

        internal static void RequestMapData(Client client)
        {
            client.Enqueue(ServerPackets.MapData(client.User.Map));
        }

        internal static void Walk(Client client, Direction direction, int stepCount)
        {
            if (stepCount == client.StepCount)
                client.User.Map.Walk(client, direction);
        }

        internal static void Pickup(Client client, byte slot, Point groundPoint)
        {
            //if there's an item on the point
            if (client.User.Map.TryGet(groundPoint, out GroundObject groundItem))
            {
                if (groundItem.Point.Distance(client.User.Point) > CONSTANTS.PICKUP_RANGE)
                    return;

                //if its gold
                if(groundItem is Gold tGold)
                {
                    client.User.Attributes.Gold += tGold.Amount;

                    client.SendAttributes(StatUpdateType.ExpGold);
                    groundItem.Map.RemoveObject(groundItem);

                    return;
                }
                Item item = groundItem.Item;

                if (client.User.Attributes.CurrentWeight + item.Weight > client.User.Attributes.MaximumWeight)
                    client.SendServerMessage(ServerMessageType.ActiveMessage, $@"You need {item.Weight} available weight to carry this item.");
                else if(client.User.Inventory.IsFull)
                    client.SendServerMessage(ServerMessageType.ActiveMessage, $@"You have no space for that.");
                else
                {
                    item.Slot = slot;
                    if (!client.User.Inventory.TryAdd(item) && !client.User.Inventory.AddToNextSlot(item))
                        return;

                    groundItem.Map.RemoveObject(groundItem);
                    client.Enqueue(ServerPackets.AddItem(item));
                }
            }
        }

        internal static void Drop(Client client, byte slot, Point groundPoint, int count)
        {
            Map map = client.User.Map;

            //dont drop if too far, or on walls, warps, or doors
            if (count == 0 || groundPoint.Distance(client.User.Point) > CONSTANTS.DROP_RANGE || map.IsWall(groundPoint) || map.Warps.ContainsKey(groundPoint) || map.Doors.ContainsKey(groundPoint))
                return;
            
            //retreive the item
            if(client.User.Inventory.TryGet(slot, out Item item))
            {
                //if we're trying to drop more than we have, return
                if (item.Count < count || item.AccountBound)
                    return;
                else //subtract the amount we're dropping
                    item.Count -= count;

                //get the grounditem associated with the item
                GroundObject groundItem = item.GroundItem(groundPoint, client.User.Map, count);

                if (item.Count > 0) //if we're suppose to still be carrying some of this item, update the count
                    client.Enqueue(ServerPackets.AddItem(item));
                else //otherwise remove the item
                {
                    if (client.User.Inventory.TryRemove(slot))
                    {
                        client.Enqueue(ServerPackets.RemoveItem(slot));
                        client.SendAttributes(StatUpdateType.Primary);
                    }
                    else
                        return;
                }

                //add the grounditem to the map
                groundItem.Map.AddObject(groundItem, groundPoint);
            }
        }

        internal static void ExitClient(Client client, bool requestExit)
        {
            //client requests to exit first, you have to confirm
            if (requestExit)
                client.Enqueue(ServerPackets.ConfirmExit());
            else
                client.Redirect(new Redirect(client, ServerType.Login));
        }

        internal static void Ignore(Client client, IgnoreType type, string targetName)
        {
            switch(type)
            {
                //if theyre requesting the user list, send it 1 per line
                case IgnoreType.Request:
                    client.SendServerMessage(ServerMessageType.ScrollWindow, client.User.IgnoreList.ToString());
                    break;
                //add a user if it's not blank, and isnt already in the list
                case IgnoreType.AddUser:
                    if (string.IsNullOrEmpty(targetName))
                        client.SendServerMessage(ServerMessageType.ActiveMessage, "Blank never loses. He can't be ignored.");
                    else if (client.User.IgnoreList.TryAdd(targetName))
                        client.SendServerMessage(ServerMessageType.ActiveMessage, $@"{targetName} is already on the list.");
                    break;
                //remove a user if it's not blank, and is already in the list
                case IgnoreType.RemoveUser:
                    if (string.IsNullOrEmpty(targetName))
                        client.SendServerMessage(ServerMessageType.ActiveMessage, "Blank never loses. He can't be ignored.");
                    else if (client.User.IgnoreList.TryRemove(targetName))
                        client.SendServerMessage(ServerMessageType.ActiveMessage, $@"{targetName} is not on the list.");
                    break;
            }
        }

        internal static void PublicChat(Client client, PublicMessageType type, string message)
        {
            //normal messages are white, shouts are yellow
            switch(type)
            {
                case PublicMessageType.Chant:
                    break;
                case PublicMessageType.Normal:
                    Match m;

                    if ((m = Regex.Match(message, @"^/([a-zA-Z]+)(?: ([a-zA-Z0-9]+))?")).Success)
                    {
                        int num = int.Parse(m.Groups[2].Value);

                        switch (m.Groups[1].Value.ToLower())
                        {
                            case "commands":
                                client.SendServerMessage(ServerMessageType.ActiveMessage, "COMMANDS:");

                                foreach (string cmd in CONSTANTS.COMMAND_LIST)
                                    client.SendServerMessage(ServerMessageType.ActiveMessage, cmd);

                                return;
                            case "hair":
                                if (CONSTANTS.HAIR_SPRITES.Contains((ushort)num))
                                    client.User.DisplayData.HairSprite = (ushort)num;
                                break;
                            case "haircolor":
                                client.User.DisplayData.HairColor = (byte)num;
                                break;
                            case "skincolor":
                                if (num >= 0 && num <= 9)
                                    client.User.DisplayData.BodyColor = (BodyColor)num;
                                break;
                            case "face":
                                client.User.DisplayData.FaceSprite = (byte)num;
                                break;
                            case "gender":
                                string str = Utility.FirstUpper(m.Groups[2].Value);

                                Gender newGender;
                                if (Enum.TryParse(str, out newGender))
                                {
                                    client.User.Gender = newGender;
                                    client.User.DisplayData.BodySprite = newGender == Gender.Male ? BodySprite.Male : BodySprite.Female;
                                }
                                break;
                        }

                        client.Refresh(true);
                        return;
                    }

                    message = $@"{client.User.Name}: {message}";
                    break;
                case PublicMessageType.Shout:
                    message = $@"{client.User.Name}: {{={(char)MessageColor.Yellow}{message}";
                    break;
            }
            
            var vObjects = new List<VisibleObject>();

            //normal messages display to everyone in 13 spaces, shouts 25
            if (type == PublicMessageType.Normal)
                vObjects = client.User.Map.ObjectsVisibleFrom(client.User, true).ToList();
            else
                vObjects = client.User.Map.ObjectsVisibleFrom(client.User, true, 25).ToList();

            //for each object within range
            foreach (VisibleObject obj in vObjects)
            {
                //if it's a user
                if (obj is User tUser)
                {
                    //if we're not being ignored, send them the message
                    if (!tUser.IgnoreList.Contains(client.User.Name))
                        tUser.Client.SendPublicMessage(type, client.User.Id, message);
                }
                //if it's a monster
                else if (obj is Monster tMonster)
                {
                    //do things
                }
                //if it's a merchant
                else if (obj is Merchant tMerchant)
                {
                    //do things
                }
            }
        }

        internal static void UseSpell(Client client, byte slot, int targetId, Point targetPoint, string prompt)
        {
            Spell spell = client.User.SpellBook[slot];

            if (spell != null && spell.CanUse && client.User.IsAlive && !(spell.CastLines > 0 && !client.User.HasFlag(UserState.IsChanting)))
            {
                if (targetId == client.User.Id)
                    spell.Activate(client, Server, spell, client.User, prompt);
                else if (client.User.Map.TryGet(targetId, out Creature target) && target.Point.Distance(targetPoint) < 5 && target.IsAlive)
                    spell.Activate(client, Server, spell, target, prompt);
            }
            client.User.RemoveFlag(UserState.IsChanting);
        }

        internal static void JoinClient(Client client, byte seed, byte[] key, string name, uint id)
        {
            //create a new crypto using the crypto information sent to us
            client.Crypto = new Crypto(seed, key, name);

            //if we're being redirected to the world
            if (client.ServerType == ServerType.World)
            {
                //retreive the user and resync it with this client
                Server.DataBase.GetUser(name).Resync(client);

                var packets = new List<ServerPacket>();

                //put all the necessary packets to log in in the list, and send them off
                foreach (Spell spell in client.User.SpellBook.Where(spell => spell != null))
                {
                    packets.Add(ServerPackets.AddSpell(spell));
                    if (!spell.CanUse)
                        packets.Add(ServerPackets.Cooldown(spell));
                }
                foreach (Skill skill in client.User.SkillBook.Where(skill => skill != null))
                {
                    packets.Add(ServerPackets.AddSkill(skill));
                    if (!skill.CanUse)
                        packets.Add(ServerPackets.Cooldown(skill));
                }
                foreach (Item item in client.User.Equipment.Where(equip => equip != null))
                    packets.Add(ServerPackets.AddEquipment(item));
                packets.Add(ServerPackets.Attributes(client.User.IsAdmin, StatUpdateType.Full, client.User.Attributes));
                foreach (Item item in client.User.Inventory.Where(item => item != null))
                    packets.Add(ServerPackets.AddItem(item));
                packets.Add(ServerPackets.LightLevel(Server.LightLevel));
                packets.Add(ServerPackets.UserId(client.User.Id, client.User.BaseClass));

                client.Enqueue(packets.ToArray());

                //add the user to the map that it's supposed to be on
                client.User.Map.AddObject(client.User, client.User.Point);
                //request their profile picture and text so we can update it if they changed it
                client.Enqueue(ServerPackets.RequestPersonal());
            }
            //otherwise if theyre in the lobby, send them the notification
            else if(client.ServerType == ServerType.Login)
                client.Enqueue(ServerPackets.LoginNotification(false, Server.LoginMessageCheckSum));
        }

        internal static void Turn(Client client, Direction direction)
        {
            //set the user's direction, and display the turn to everyone in range to see
            client.User.Direction = direction;
            foreach (User user in client.User.Map.ObjectsVisibleFrom(client.User).OfType<User>())
                user.Client.Enqueue(ServerPackets.CreatureTurn(client.User.Id, direction));
        }

        internal static void SpaceBar(Client client)
        {
            if (!client.User.IsAlive)
                return;

            var packets = new List<ServerPacket>();

            //cancel casting
            client.User.RemoveFlag(UserState.IsChanting);
            packets.Add(ServerPackets.CancelCasting());

            //use all basic skills (otherwise known as assails)
            foreach (Skill skill in client.User.SkillBook)
                if (skill?.IsBasic == true && skill.CanUse)
                {
                    skill.Activate(client, Server, skill);
                    skill.LastUse = DateTime.UtcNow;
                }

            client.Enqueue(packets.ToArray());
        }

        internal static void RequestWorldList(Client client)
        {
            client.Enqueue(ServerPackets.WorldList(Server.WorldClients.Select(cli => cli.User), client.User.Attributes.Level));
        }

        internal static void Whisper(Client client, string targetName, string message)
        {
            //if the user isnt online, tell them
            if (!Server.TryGetUser(targetName, out User targetUser))
                client.SendServerMessage(ServerMessageType.Whisper, "That user is not online.");
            //otherwise, if the use is ignoring them, dont tell them. Make it seem like theyre succeeding, so they dont bother the person
            else if (targetUser.IgnoreList.Contains(client.User.Name))
                client.SendServerMessage(ServerMessageType.Whisper, $@"{targetName} > {message}");
            //otherwise let them know if the target is on Do Not Disturb
            else if (targetUser.SocialStatus == SocialStatus.DoNotDisturb)
                client.SendServerMessage(ServerMessageType.Whisper, $@"{targetName} doesn't want to be bothered right now.");
            //otherwise send the whisper
            else
            {
                client.SendServerMessage(ServerMessageType.Whisper, $@"{targetName} > {message}");
                targetUser.Client.SendServerMessage(ServerMessageType.Whisper, $@"{client.User.Name} < {message}");
            }
        }

        internal static void ToggleUserOption(Client client, UserOption option)
        {
            //request is for the whole list, send the whole thing
            if (option == UserOption.Request)
                client.SendServerMessage(ServerMessageType.UserOptions, client.User.UserOptions.ToString());
            else //otherwise send the single option they toggled
            {
                client.User.UserOptions.Toggle(option);
                client.SendServerMessage(ServerMessageType.UserOptions, client.User.UserOptions.ToString(option));
            }
        }

        internal static void UseItem(Client client, byte slot)
        {
            Item item = client.User.Inventory[slot];

            if (item != null && item.CanUse)
                item.Activate(client, Server, item);
        }

        internal static void AnimateCreature(Client client, BodyAnimation animNum)
        {
            foreach (User user in client.User.Map.ObjectsVisibleFrom(client.User, true).OfType<User>())
                user.Client.Enqueue(ServerPackets.AnimateCreature(client.User.Id, animNum, 100));
        }

        internal static void DropGold(Client client, uint amount, Point groundPoint)
        {
            Map map = client.User.Map;
            //dont drop on walls, warps, or doors
            if (amount == 0 || groundPoint.Distance(client.User.Point) > CONSTANTS.DROP_RANGE || amount > client.User.Attributes.Gold || map.IsWall(groundPoint) || map.Warps.ContainsKey(groundPoint) || map.Doors.ContainsKey(groundPoint))
                return;

            client.User.Attributes.Gold -= amount;
            map.AddObject(CreationEngine.CreateGold(client, amount, groundPoint), groundPoint);
            client.SendAttributes(StatUpdateType.ExpGold);
        }

        internal static void ChangePassword(Client client, string name, string currentPw, string newPw)
        {
            if (Server.DataBase.ChangePassword(name, currentPw, newPw))
                client.SendLoginMessage(LoginMessageType.ClearNameMessage, "Password successfully changed.");
        }

        internal static readonly object ExchangeLock = new object();
        internal static void DropItemOnCreature(Client client, byte slot, int targetId, byte count)
        {
            lock (ExchangeLock)
            {
                if (client.User.Map.TryGet(targetId, out VisibleObject obj) && obj is Creature && client.User.Inventory.TryGet(slot, out Item item) && item != null && obj.Point.Distance(client.User.Point) <= CONSTANTS.DROP_RANGE)
                {
                    if (item.AccountBound)
                        return;

                    if (obj is Monster tMonster)
                    {
                        client.User.Inventory.TryRemove(slot);
                        tMonster.Items.Add(item);
                    }
                    else if (obj is User tUser)
                    {
                        if (tUser == client.User)
                            return;

                        var ex = new Exchange(client.User, tUser);
                        if (World.Exchanges.TryAdd(ex.ExchangeId, ex))
                        {
                            ex.Activate();
                            ex.AddItem(client.User, slot);
                        }
                    }
                }
            }
        }

        internal static void DropGoldOnCreature(Client client, uint amount, int targetId)
        {
            lock (ExchangeLock)
            {
                if (client.User.Attributes.Gold > amount && client.User.Map.TryGet(targetId, out VisibleObject obj) && obj is Creature && obj.Point.Distance(client.User.Point) <= CONSTANTS.DROP_RANGE)
                {
                    if (obj is Monster tMonster)
                    {
                        tMonster.Gold += amount;
                        client.User.Attributes.Gold -= amount;
                    }
                    else if(obj is User tUser)
                    {
                        if (tUser == client.User)
                            return;

                        var ex = new Exchange(client.User, tUser);
                        if (World.Exchanges.TryAdd(ex.ExchangeId, ex))
                        {
                            ex.Activate();
                            ex.SetGold(client.User, amount);
                        }
                    }
                }
            }
        }

        internal static void RequestProfile(Client client)
        {
            client.Enqueue(ServerPackets.ProfileSelf(client.User));
        }

        internal static readonly object GroupLock = new object();
        internal static void RequestGroup(Client client, GroupRequestType type, string targetName, GroupBox box)
        {
            lock (GroupLock)
            {
                User targetUser;

                switch (type)
                {
                    case GroupRequestType.Invite:
                        if (!client.User.Map.TryGet(targetName, out targetUser))                                                                                     //if target user doesnt exist
                            client.SendServerMessage(ServerMessageType.ActiveMessage, $@"{targetName} is not near.");
                        else if (targetUser.IgnoreList.Contains(client.User.Name))                                                                                              //if theyre on the ignore list, return
                            return;
                        else if (!client.User.UserOptions.Group)                                                                                                                //else if your grouping is turned off, let them know
                            client.SendServerMessage(ServerMessageType.ActiveMessage, $@"Grouping is disabled.");
                        else if (!targetUser.UserOptions.Group)                                                                                                                 //else if the targets grouping is turned off, let them know
                            client.SendServerMessage(ServerMessageType.ActiveMessage, $@"{targetName} is not accepting group invites.");
                        else if (client.User.IsGrouped)                                                                                                                           //else if this we are already in a group
                        {
                            if (client.User == targetUser)                                                                                                                          //and we're trying to group ourself
                            {
                                if (targetUser.Group.TryRemove(client.User))                                                                                                            //leave the group
                                    client.SendServerMessage(ServerMessageType.ActiveMessage, "You have left the group.");
                            }
                            else if (!targetUser.IsGrouped)                                                                                                                           //else if the target isnt grouped
                                targetUser.Client.Enqueue(ServerPackets.GroupRequest(GroupRequestType.Request, client.User.Name));
                            else if (targetUser.Group != client.User.Group)                                                                                                         //else if the target's group isnt the same as our group
                                client.SendServerMessage(ServerMessageType.ActiveMessage, $@"{targetName} is already in a group.");
                            else if (client.User.Group.Leader == client.User)                                                                                                       //else if we're the leader of the group
                            {
                                if (client.User.Group.TryRemove(targetUser, true))                                                                                                      //kick them from the group
                                    client.SendServerMessage(ServerMessageType.ActiveMessage, $@"You have kicked {targetName} from the group.");
                            }
                            else                                                                                                                                                    //else we cant kick them, just say theyre in our group
                                client.SendServerMessage(ServerMessageType.ActiveMessage, $@"{targetName} is already in your group.");
                        }
                        else                                                                                                                                                     //else we're not grouped
                        {
                            if (client.User == targetUser)                                                                                                                          //and we are trying to group ourself
                                client.SendServerMessage(ServerMessageType.ActiveMessage, "You can't form a group alone.");
                            else if (targetUser.IsGrouped)                                                                                                                            //else if target is grouped
                                client.SendServerMessage(ServerMessageType.ActiveMessage, $@"{targetName} is already in a group.");
                            else                                                                                                                                                    //else send them a group request
                                targetUser.Client.Enqueue(ServerPackets.GroupRequest(GroupRequestType.Request, client.User.Name));
                        }
                        break;
                    case GroupRequestType.Join:
                        if (!client.User.Map.TryGet(targetName, out targetUser))
                            client.SendServerMessage(ServerMessageType.ActiveMessage, $@"{targetName} is not near.");
                        else if (targetUser.IgnoreList.Contains(client.User.Name))
                            return;
                        else if (client.User.IsGrouped)
                            client.SendServerMessage(ServerMessageType.ActiveMessage, "You are already in a group.");
                        else
                        {
                            if (client.User == targetUser)
                                client.SendServerMessage(ServerMessageType.ActiveMessage, "You can't form a group alone.");
                            else if (targetUser.IsGrouped)
                            { //force group, hmmm
                                if(targetUser.Group.TryAdd(client.User))
                                {
                                    foreach (User user in targetUser.Group)
                                        user.Client.SendServerMessage(ServerMessageType.ActiveMessage, $@"{client.User.Name} has joined the group.");
                                    client.SendServerMessage(ServerMessageType.ActiveMessage, $@"You join {targetName}'s group.");
                                }
                            }
                            else
                            {
                                client.User.Group = new Group(targetUser, client.User);
                                targetUser.Client.SendServerMessage(ServerMessageType.ActiveMessage, $@"You form a group with {client.User.Name}");
                                client.SendServerMessage(ServerMessageType.ActiveMessage, $@"You form a group with {targetName}.");
                            }
                        }
                        break;
                    case GroupRequestType.Groupbox:

                        break;

                    case GroupRequestType.RemoveGroupBox:

                        break;
                }
            }
        }

        internal static void ToggleGroup(Client client)
        {
            //toggle the group useroption
            client.User.UserOptions.Toggle(UserOption.Group);

            //remove yourself from the group, if you're in one
            if (client.User.IsGrouped)
                if (client.User.Group.TryRemove(client.User))
                    client.User.Group = null;

            //send the profile so the group button will change, also send the useroption that was changed
            client.Enqueue(ServerPackets.ProfileSelf(client.User));
            client.SendServerMessage(ServerMessageType.UserOptions, client.User.UserOptions.ToString(UserOption.Group));
        }

        internal static void SwapSlot(Client client, Pane pane, byte origSlot, byte endSlot)
        {
            //attempt to swap the objects at origSlot and endSlot in the given pane
            switch (pane)
            {
                case Pane.Inventory:
                    if (!client.User.Inventory.TrySwap(origSlot, endSlot))
                        return;

                    //if it succeeds, update the user's panels
                    client.Enqueue(ServerPackets.RemoveItem(origSlot));
                    client.Enqueue(ServerPackets.RemoveItem(endSlot));

                    //check for null, incase we were simply moving an obj to an already empty slot
                    if (client.User.Inventory[origSlot] != null)
                        client.Enqueue(ServerPackets.AddItem(client.User.Inventory[origSlot]));
                    if (client.User.Inventory[endSlot] != null)
                        client.Enqueue(ServerPackets.AddItem(client.User.Inventory[endSlot]));
                    break;
                case Pane.SkillBook:
                    if (!client.User.SkillBook.TrySwap(origSlot, endSlot))
                        return;

                    //if it succeeds, update the user's panels
                    client.Enqueue(ServerPackets.RemoveSkill(origSlot));
                    client.Enqueue(ServerPackets.RemoveSkill(endSlot));

                    //check for null, incase we were simply moving an obj to an already empty slot
                    if (client.User.SkillBook[origSlot] != null)
                        client.Enqueue(ServerPackets.AddSkill(client.User.SkillBook[origSlot]));
                    if (client.User.SkillBook[endSlot] != null)
                        client.Enqueue(ServerPackets.AddSkill(client.User.SkillBook[endSlot]));
                    break;
                case Pane.SpellBook:
                    if (!client.User.SpellBook.TrySwap(origSlot, endSlot))
                        return;

                    //if it succeeds, update the user's panels
                    client.Enqueue(ServerPackets.RemoveSpell(origSlot));
                    client.Enqueue(ServerPackets.RemoveSpell(endSlot));

                    //check for null, incase we were simply moving an obj to an already empty slot
                    if (client.User.SpellBook[origSlot] != null)
                        client.Enqueue(ServerPackets.AddSpell(client.User.SpellBook[origSlot]));
                    if (client.User.SpellBook[endSlot] != null)
                        client.Enqueue(ServerPackets.AddSpell(client.User.SpellBook[endSlot]));
                    break;
                default:
                    return;
            }
        }

        internal static void RequestRefresh(Client client)
        {
            client.Refresh();
        }

        internal static void RequestPursuit(Client client, GameObjectType objType, int objId, PursuitIds pursuitId, byte[] args)
        {
            if(client.User.Map.TryGet(objId, out VisibleObject obj) && client.User.WithinRange(obj))
                Dialogs.ActivatePursuit(pursuitId)(client, Server);
        }

        internal static void ReplyDialog(Client client, GameObjectType objType, int objId, PursuitIds pursuitId, ushort dialogId, DialogArgsType argsType, byte option, string userInput)
        {
            Dialog dialog = client.CurrentDialog;
            PursuitIds dialogPursuit = dialog.PursuitId;
            option--;

            //if there's no active dialog or object, what are we replying to?
            //if the active object is no longer valid, cease the dialog
            if (client.CurrentDialog == null || client.ActiveObject == null ||
                (client.ActiveObject is Merchant tMerchant && !tMerchant.WithinRange(client.User)) ||
                (client.ActiveObject is Item tItem && !client.User.Inventory.Contains(tItem)))
            {
                client.CurrentDialog = null;
                client.ActiveObject = null;
                return;
            }

            DialogOption opt = Enum.IsDefined(typeof(DialogOption), (dialogId - dialog.Id)) ? (DialogOption)(dialogId - dialog.Id) : DialogOption.Close;

            switch (opt)
            {
                case DialogOption.Previous:
                    client.CurrentDialog = client.CurrentDialog.Previous();
                    break;
                case DialogOption.Close:
                    client.ActiveObject = null;
                    client.CurrentDialog = null;
                    //Dialogs.ActivatePursuit(dialog.PursuitId)(client, Server, true);
                    return;
                case DialogOption.Next:
                    switch(argsType)
                    {
                        case DialogArgsType.None:
                            client.CurrentDialog = client.CurrentDialog.Next();
                            break;
                        case DialogArgsType.MenuResponse:
                            DialogMenuItem menuItem = client.CurrentDialog.Menu[option];
                            if (menuItem.PursuitId != PursuitIds.None)
                            {
                                dialogPursuit = menuItem.PursuitId;
                                client.CurrentDialog = client.CurrentDialog.Next();
                            }
                            else
                                client.CurrentDialog = client.CurrentDialog.Next(option);
                            break;
                        case DialogArgsType.TextResponse:
                            client.CurrentDialog = client.CurrentDialog.Next();
                            break;
                    }

                    Dialogs.ActivatePursuit(dialogPursuit)(client, Server, false, option, userInput);
                    break;
            }

            client.SendDialog(client.ActiveObject, client.CurrentDialog);
        }

        internal static void Boards(Client client)
        {
            //work in progress
            client.Enqueue(ServerPackets.BulletinBoard());
        }

        internal static void UseSkill(Client client, byte slot)
        {
            Skill skill = client.User.SkillBook[slot];

            if (skill != null && skill.CanUse && client.User.IsAlive)
                skill.Activate(client, Server, skill);
        }

        internal static void ClickWorldMap(Client client, ushort nodeCheckSum, ushort mapId, Point point)
        {
            if (client.User.Map.WorldMaps[client.User.Point].Nodes.FirstOrDefault(n => n.CheckSum == nodeCheckSum).Point == point)
                World.Maps[mapId].AddObject(client.User, point);
        }

        internal static void ClickObject(Client client, int objectId)
        {
            //if we're clicking ourself, send profileSelf
            if (objectId == client.User.Id)
                client.Enqueue(ServerPackets.ProfileSelf(client.User));
            else
            {   //otherwise, get the object we're clicking
                if (client.User.Map.TryGet(objectId, out VisibleObject obj))
                {
                    //if it's a monster, display it's name
                    if (obj is Monster)
                        client.SendServerMessage(ServerMessageType.OrangeBar1, obj.Name);
                    //if it's a user, send us their profile
                    else if (obj is User tUser)
                    {
                        if (tUser.ShouldDisplay)
                            client.Enqueue(ServerPackets.Profile(tUser));
                        else
                            tUser.LastClicked = DateTime.UtcNow;
                    }
                    //if its a merchant, send us the merchant menu
                    else if (obj is Merchant tMerchant)
                    {
                        if (tMerchant.ShouldDisplay)
                        {
                            if (tMerchant.Menu != null)
                                client.SendMenu(tMerchant);
                            else
                                client.SendDialog(tMerchant, Dialogs[tMerchant.NextDialogId]);
                        }
                        else
                            tMerchant.LastClicked = DateTime.UtcNow;

                    }
                }
            }
        }

        internal static void ClickObject(Client client, Point clickPoint)
        {
            //dont allow this to be spammed
            if (DateTime.UtcNow.Subtract(client.LastClickObj).TotalSeconds < 1)
                return;
            else
                client.LastClickObj = DateTime.UtcNow;

            if (client.User.Map.TryGet(clickPoint, out Door door))
                client.User.Map.ToggleDoor(door);
        }

        internal static void RemoveEquipment(Client client, EquipmentSlot slot)
        {
            if (client.User.Inventory.IsFull)
            {
                client.SendServerMessage(ServerMessageType.ActiveMessage, "You have no space for that.");
                return;
            }

            //attempt to remove equipment at the given slot
            if (client.User.Equipment.TryGetRemove((byte)slot, out Item item))
            {
                //if it succeeds, display the item in the user's inventory, and remove it from the equipment panel
                client.User.Inventory.AddToNextSlot(item);
                client.Enqueue(ServerPackets.RemoveEquipment(slot));
                client.Enqueue(ServerPackets.AddItem(item));
                //set hp/mp?
                client.SendAttributes(StatUpdateType.Primary);

                foreach (User user in client.User.Map.ObjectsVisibleFrom(client.User, true).OfType<User>())
                    user.Client.Enqueue(ServerPackets.DisplayUser(client.User));
            }
        }

        internal static void KeepAlive(Client client, byte a, byte b)
        {
            client.Enqueue(ServerPackets.KeepAlive(a, b));
        }

        internal static void ChangeStat(Client client, Stat stat)
        {
            switch(stat)
            {
                case Stat.STR:
                    client.User.Attributes.BaseStr++;
                    break;
                case Stat.INT:
                    client.User.Attributes.BaseInt++;
                    break;
                case Stat.WIS:
                    client.User.Attributes.BaseWis++;
                    break;
                case Stat.CON:
                    client.User.Attributes.BaseCon++;
                    break;
                case Stat.DEX:
                    client.User.Attributes.BaseDex++;
                    break;
            }

            client.SendAttributes(StatUpdateType.Primary);
        }

        internal static void Exchange(Client client, ExchangeType type, int targetId = 0, uint goldAmount = 0, byte itemSlot = 0, byte itemCount = 0)
        {
            switch (type)
            {
                case ExchangeType.StartExchange:
                    User targetUser;

                    if (client.User.Map.TryGet(targetId, out targetUser))
                        if (client.User.Exchange == null && targetUser.Exchange == null)
                        {
                            var ex = new Exchange(client.User, targetUser);
                            World.Exchanges[ex.ExchangeId] = ex;
                            ex.Activate();
                        }
                    break;
                case ExchangeType.RequestAmount:
                    client.User.Exchange.AddItem(client.User, itemSlot);
                    break;
                case ExchangeType.AddItem:
                    client.User.Exchange.AddStackableItem(client.User, itemSlot, itemCount);
                    break;
                case ExchangeType.SetGold:
                    client.User.Exchange.SetGold(client.User, goldAmount);
                    break;
                case ExchangeType.Cancel:
                    client.User.Exchange.Cancel(client.User);
                    break;
                case ExchangeType.Accept:
                    client.User.Exchange.Accept(client.User);
                    break;
            }
        }

        internal static void RequestLoginNotification(bool request, Client client)
        {
            client.Enqueue(ServerPackets.LoginNotification(request, Server.LoginMessageCheckSum, Server.LoginMessage));
        }

        internal static void BeginChant(Client client)
        {
            client.User.AddFlag(UserState.IsChanting);
        }

        internal static void DisplayChant(Client client, string chant)
        {
            foreach (User user in client.User.Map.ObjectsVisibleFrom(client.User, true).OfType<User>())
                user.Client.SendPublicMessage(PublicMessageType.Chant, client.User.Id, chant);
        }

        internal static void Personal(Client client, byte[] portraitData, string profileMsg)
        {
            if (portraitData.Length > 0)
                using (var imageData = new MemoryStream(portraitData))
                using (var portrait = Image.FromStream(imageData))
                    if (portrait.Width != 48 || portrait.Height != 56)
                        portraitData = new byte[] { };

            client.User.Personal = new Personal(portraitData, profileMsg);
        }

        internal static void RequestServerTable(Client client, bool request)
        {
            if (request)
                client.Enqueue(ServerPackets.ServerTable(Server.Table));
            else
                client.Redirect(new Redirect(client, ServerType.Login));
        }

        internal static void RequestHomepage(Client client)
        {
            client.Enqueue(ServerPackets.LobbyControls(3, @"http://www.darkages.com"));
        }

        internal static void SynchronizeTicks(Client client, TimeSpan serverTicks, TimeSpan clientTicks)
        {
            client.Enqueue(ServerPackets.SynchronizeTicks());
        }

        internal static void ChangeSoocialStatus(Client client, SocialStatus status)
        {
            client.User.SocialStatus = status;
        }

        internal static void RequestMetaFile(Client client, bool all)
        {
            client.Enqueue(ServerPackets.Metafile(all, Server.MetaFiles.ToArray()));
        }
        #endregion
    }
}
