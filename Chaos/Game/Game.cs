using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Chaos;
using System.Threading;
using System.Collections.Generic;
using System.Text;

namespace Chaos
{
    internal static class Game
    {
        internal static readonly object Sync = new object();
        internal static Server Server = null;
        internal static World World = null;
        internal static Dictionary<string, OnUseDelegate> PanelObjects = null;

        internal static void JoinServer(Client client)
        {
            client.Enqueue(Server.Packets.ConnectionInfo(Server.TableCheckSum, client.Crypto.Seed, client.Crypto.Key));
        }

        internal static void CreateChar1(Client client, string name, string password)
        {
            //checks if the name is 4-12 characters straight, if not... checks if there's a string 7-12 units long that has a space surrounced by at least 3 chars on each side.
            if (!Regex.Match(name, @"[a-zA-Z]{4,12}").Success && (!Regex.Match(name, @"[a-z A-Z]{7, 12}").Success || !Regex.Match(name, @"[a-zA-Z]{3} [a-zA-Z]{3}").Success))
                client.Enqueue(Server.Packets.LoginMessage(LoginMessageType.Message, "Name must be 4-12 characters long, or a space surrounded by at least 3 characters on each side, up to 12 total."));
            //checks if the password is 4-8 units long
            else if (!Regex.Match(password, @".{4,8}").Success)
                client.Enqueue(Server.Packets.LoginMessage(LoginMessageType.Message, "Password must be 4-8 units long."));
            //check if a user already exists with the given valid name
            else if(Server.DataBase.UserExists(name))
                client.Enqueue(Server.Packets.LoginMessage(LoginMessageType.Message, "That name is taken."));
            else
            {   //otherwise set the client's new character fields so CreateChar1 can use the information and send a confirmation to the client
                client.CreateCharName = name;
                client.CreateCharPw = password;
                client.Enqueue(Server.Packets.LoginMessage(LoginMessageType.Confirm));
            }
        }

        internal static void Login(Client client, string name, string password)
        {
            User user;
            //checks the userhash to see if the given name and password exist
            if (!Server.DataBase.CheckHash(name, Crypto.GetHashString(password, "MD5")))
                client.Enqueue(Server.Packets.LoginMessage(LoginMessageType.Message, "Incorrect user name or password."));
            //checks to see if the user is currently logged on
            else if (Server.TryGetUser(name, out user))
            {
                client.Enqueue(Server.Packets.LoginMessage(LoginMessageType.Message, "That character is already logged in."));
                user.Client.Disconnect();
            }
            else
            {   //otherwise, confirms the login, sends the login message, and redirects them to the world
                client.Enqueue(Server.Packets.LoginMessage(LoginMessageType.Confirm));
                client.Enqueue(Server.Packets.ServerMessage(ServerMessageType.ActiveMessage, "Logging in to Chaos"));
                client.Redirect(new Redirect(client, ServerType.World, name));
            }

        }

        internal static void CreateChar2(Client client, byte hairStyle, Gender gender, byte hairColor)
        {
            if (string.IsNullOrEmpty(client.CreateCharName) || string.IsNullOrEmpty(client.CreateCharPw))
                return;

            hairStyle = (byte)(hairStyle < 1 ? 1 : hairStyle > 17 ? 17 : hairStyle);
            hairColor = (byte)(hairColor > 13 ? 13 : hairColor < 0 ? 0 : hairColor);
            gender = gender != Gender.Male && gender != Gender.Female ? Gender.Male : gender;

            User newUser = new User(client.CreateCharName, new Point(20, 20), World.Maps[5031], Direction.South);
            DisplayData data = new DisplayData(newUser, hairStyle, hairColor, (byte)((byte)gender * 16));
            newUser.DisplayData = data;

            if (World.Admins.Contains(newUser.Name, StringComparer.CurrentCultureIgnoreCase))
            {
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
            }

            if (Server.DataBase.TryAddUser(newUser, client.CreateCharPw))
                client.Enqueue(Server.Packets.LoginMessage(LoginMessageType.Confirm));
            else
                client.Enqueue(Server.Packets.LoginMessage(LoginMessageType.Message, "Unable to create character. Possibly already exists???"));
        }

        internal static void RequestMapData(Client client)
        {
            client.Enqueue(Server.Packets.MapData(client.User.Map));
        }

        internal static void Walk(Client client, Direction direction, int stepCount)
        {
            lock(client.User.Map.Sync)
            {
                //if the stepcount matches with what we have
                if (stepCount == client.StepCount)
                {
                    //plus the stepcount
                    client.StepCount++;

                    //check if we can actually walk to the spot
                    if (!client.User.IsAdmin && !client.User.Map.IsWalkable(client.User.Point.Offsetter(direction)))
                    {
                        //if no, set their location back to what it was and return
                        World.Refresh(client, true);
                        return;
                    }
                    //prepare a list of items and monsters
                    List<VisibleObject> itemMonster = new List<VisibleObject>();

                    //get all objects visible before the walk
                    List<VisibleObject> visibleBefore = World.ObjectsVisibleFrom(client.User);

                    //send them the walk
                    foreach (User user in visibleBefore.OfType<User>())
                        user.Client.Enqueue(Server.Packets.CreatureWalk(client.User, direction));

                    //offset the point and validate the walk to the client
                    client.User.Point.Offset(direction);
                    client.Enqueue(Server.Packets.ClientWalk(direction, client.User.Point));

                    //get an updated list of nearby objects
                    List<VisibleObject> visibleAfter = World.ObjectsVisibleFrom(client.User);

                    //compare what's visible before and after the walk, send packets to users and client accordingly
                    foreach (VisibleObject obj in visibleAfter.Except(visibleBefore, new WorldObjectComparer()))
                    {
                        User user = obj as User;

                        if (user != null)
                        {
                            user.Client.Enqueue(Server.Packets.DisplayUser(client.User));
                            client.Enqueue(Server.Packets.DisplayUser(user));
                        }
                        else
                            itemMonster.Add(obj);
                    }

                    if (itemMonster.Count > 0)
                        client.Enqueue(Server.Packets.DisplayItemMonster(itemMonster.ToArray()));

                    foreach (VisibleObject obj in visibleBefore.Except(visibleAfter, new WorldObjectComparer()))
                    {
                        (obj as User)?.Client.Enqueue(Server.Packets.RemoveObject(client.User));
                        client.Enqueue(Server.Packets.RemoveObject(obj));
                    }

                    MapObject mapObj;
                    if (World.TryGetMapObject(client.User.Point, out mapObj, client.User.Map) && mapObj is Warp)
                        World.WarpUser(client.User, mapObj as Warp);

                    WorldMap worldMap;
                    if (World.TryGetWorldMap(client.User.Point, out worldMap, client.User.Map))
                    {
                        World.RemoveObjectFromMap(client.User, true);
                        client.Enqueue(Server.Packets.WorldMap(worldMap));
                    }
                }
            }
        }

        internal static void Pickup(Client client, byte slot, Point groundPoint)
        {
            //see if there's actually an item at the spot
            GroundItem groundItem;

            //if there's an item on the point
            if (World.TryGetGroundItem(groundPoint, out groundItem, client.User.Map))
            {
                groundItem.Item.Slot = slot;
                if (!client.User.Inventory.TryAdd(groundItem.Item))
                    return;

                World.RemoveObjectFromMap(groundItem);
                client.Enqueue(Server.Packets.AddItem(groundItem.Item));
            }
        }

        internal static void Drop(Client client, byte slot, Point groundPoint, int count)
        {
            Map map = client.User.Map;
            if (count == 0 || map.IsWall(groundPoint) || map.Exits.ContainsKey(groundPoint) || map.Doors.ContainsKey(groundPoint))
                return;

            Item item;
            
            //retreived the item
            if(client.User.Inventory.TryGet(slot, out item) && item != null)
            {
                //if we're trying to drop more than we have, return
                if (item.Count < count)
                    return;
                else //subtract the amount we're dropping
                    item.Count = item.Count - count;

                //get the grounditem associated with the item
                GroundItem groundItem = item.GroundItem(groundPoint, client.User.Map, count);

                if (item.Count > 0) //if we're suppose to still be carrying some of this item, update the count
                    client.Enqueue(Server.Packets.AddItem(item));
                else //otherwise remove the item
                {
                    if (client.User.Inventory.TryRemove(slot))
                        client.Enqueue(Server.Packets.RemoveItem(slot));
                    else
                        return;
                }

                //add the grounditem to the map
                World.AddObjectToMap(groundItem, new Location(groundItem.Map.Id, groundPoint));
            }
        }

        internal static void ExitClient(Client client, bool requestExit)
        {
            if (requestExit)
                client.Enqueue(Server.Packets.ConfirmExit());
            else
                client.Redirect(new Redirect(client, ServerType.Login));
        }

        internal static void Ignore(Client client, IgnoreType type, string targetName)
        {
            switch(type)
            {
                //if theyre requesting the user list, send it 1 per line
                case IgnoreType.Request:
                    client.Enqueue(Server.Packets.ServerMessage(ServerMessageType.ScrollWindow, string.Join(Environment.NewLine, client.User.IgnoreList.ToArray())));
                    break;
                //add a user if it's not blank, and isnt already in the list
                case IgnoreType.AddUser:
                    if (string.IsNullOrEmpty(targetName))
                        client.Enqueue(Server.Packets.ServerMessage(ServerMessageType.ActiveMessage, "Blank never loses. He can't be ignored."));
                    else if (client.User.IgnoreList.TryAdd(targetName))
                        client.Enqueue(Server.Packets.ServerMessage(ServerMessageType.ActiveMessage, $@"{targetName} is already on the list."));
                    break;
                //remove a user if it's not blank, and is already in the list
                case IgnoreType.RemoveUser:
                    if (string.IsNullOrEmpty(targetName))
                        client.Enqueue(Server.Packets.ServerMessage(ServerMessageType.ActiveMessage, "Blank never loses. He can't be ignored."));
                    else if (client.User.IgnoreList.TryRemove(targetName))
                        client.Enqueue(Server.Packets.ServerMessage(ServerMessageType.ActiveMessage, $@"{targetName} is not on the list"));
                    break;
            }
        }

        internal static void PublicChat(Client client, ClientMessageType type, string message)
        {
            //ADMIN SLASH COMMANDS (I SHOULD MAKE A TRINKET LUL)
            if (message.StartsWith("/") && client.User.IsAdmin)
            {
                message = message.Replace("/", "");

                //admin commands
                return;
            }

            switch(type)
            {
                case ClientMessageType.Chant:
                    break;
                case ClientMessageType.Normal:
                    message = $@"{client.User.Name}: {message}";
                    break;
                case ClientMessageType.Shout:
                    message = $@"{client.User.Name}: {{={(char)MessageColor.Yellow}{message}";
                    break;
            }
            
            List<VisibleObject> objects = new List<VisibleObject>();
            if (type == ClientMessageType.Normal)
                objects = World.ObjectsVisibleFrom(client.User);
            else
                objects = World.ObjectsVisibleFrom(client.User, 25);
            objects.Add(client.User);

            foreach (var obj in objects)
            {
                if (obj is User)
                {
                    User user = obj as User;
                    if (!user.IgnoreList.Contains(client.User.Name))
                        user.Client.Enqueue(Server.Packets.PublicChat(type, client.User.Id, message));
                }
                else if (obj is Monster)
                {
                    //do things
                }
                else if (obj is Merchant)
                {
                    //do things
                }
                
            }

        }

        internal static void UseSpell(Client client, byte slot, int targetId, Point targetPoint)
        {
            throw new NotImplementedException();
        }

        internal static void JoinClient(Client client, byte seed, byte[] key, string name, uint id)
        {
            client.Crypto = new Crypto(seed, key, name);

            if (client.ServerType == ServerType.World)
            {
                Server.DataBase.GetUser(name).Sync(client);
                List<ServerPacket> packets = new List<ServerPacket>();
                foreach (Spell spell in client.User.SpellBook.Where(spell => spell != null))
                    packets.Add(Server.Packets.AddSpell(spell));
                foreach (Skill skill in client.User.SkillBook.Where(spell => spell != null))
                    packets.Add(Server.Packets.AddSkill(skill));
                foreach (Item item in client.User.Equipment.Where(skill => skill != null))
                    packets.Add(Server.Packets.AddEquipment(item));
                packets.Add(Server.Packets.Attributes(client.User.IsAdmin, StatUpdateFlags.Full, client.User.Attributes));
                foreach (Item item in client.User.Inventory.Where(skill => skill != null))
                    packets.Add(Server.Packets.AddItem(item));
                packets.Add(Server.Packets.LightLevel(Server.LightLevel));
                packets.Add(Server.Packets.UserId(client.User.Id, client.User.BaseClass));

                client.Enqueue(packets.ToArray());
                World.AddObjectToMap(client.User, client.User.Location);
                client.Enqueue(Server.Packets.RequestPersonal());
            }
            else if(client.ServerType == ServerType.Lobby)
                client.Enqueue(Server.Packets.LobbyNotification(false, Server.LoginMessageCheckSum));
        }

        internal static void Turn(Client client, Direction direction)
        {
            client.User.Direction = direction;

            foreach (User user in World.ObjectsVisibleFrom(client.User).OfType<User>())
                user.Client.Enqueue(Server.Packets.CreatureTurn(client.User.Id, direction));
        }

        internal static void SpaceBar(Client client)
        {
            List<ServerPacket> packets = new List<ServerPacket>();
            packets.Add(Server.Packets.CancelCasting());
            foreach (Skill skill in client.User.SkillBook)
                if (skill.IsBasic)
                {
                    packets.Add(Server.Packets.CreatureAnimation(client.User.Id, skill.Animation, 100, false));

                    //damage checking, calculations, etc
                    return;
                }
        }

        internal static void RequestWorldList(Client client)
        {
            client.Enqueue(Server.Packets.WorldList(Server.Clients.Values.Select(cli => cli.User), client.User.Attributes.Level));
        }

        internal static void Whisper(Client client, string targetName, string message)
        {
            User targetUser;

            //if the user isnt online, tell them
            if (!Server.TryGetUser(targetName, out targetUser))
                client.Enqueue(Server.Packets.ServerMessage(ServerMessageType.Whisper, "That user is not online."));
            //otherwise, if the use is ignoring them, dont tell them. Make it seem like theyre succeeding, so they dont bother the person
            else if (targetUser.IgnoreList.Contains(client.User.Name))
                client.Enqueue(Server.Packets.ServerMessage(ServerMessageType.Whisper, $@"{targetName} >> {message}"));
            //otherwise let them know if the target is on Do Not Disturb
            else if (targetUser.SocialStatus == SocialStatus.DoNotDisturb)
                client.Enqueue(Server.Packets.ServerMessage(ServerMessageType.Whisper, $@"{targetName} doesn't want to be bothered right now."));
            //otherwise send the whisper
            else
            {
                client.Enqueue(Server.Packets.ServerMessage(ServerMessageType.Whisper, $@"{targetName} >> {message}"));
                targetUser.Client.Enqueue(Server.Packets.ServerMessage(ServerMessageType.Whisper, $@"{client.User.Name} << {message}"));
            }
        }

        internal static void ToggleUserOption(Client client, UserOption option)
        {
            if (option == UserOption.Request)
                client.Enqueue(Server.Packets.ServerMessage(ServerMessageType.UserOptions, client.User.UserOptions.ToString()));
            else
            {
                client.User.UserOptions.Toggle(option);
                client.Enqueue(Server.Packets.ServerMessage(ServerMessageType.UserOptions, client.User.UserOptions.ToString(option)));
            }
        }

        internal static void UseItem(Client client, byte slot)
        {
            throw new NotImplementedException();
        }

        internal static void AnimateUser(Client client, byte index)
        {
            client.Enqueue(Server.Packets.CreatureAnimation(client.User.Id, index, 100));
        }

        internal static void DropGold(Client client, uint amount, Point groundPoint)
        {
            throw new NotImplementedException();
        }

        internal static void ChangePassword(Client client, string name, string currentPw, string newPw)
        {
            if (Server.DataBase.ChangePassword(name, currentPw, newPw))
                client.Enqueue(Server.Packets.LoginMessage(LoginMessageType.Message, "Password successfully changed."));
        }

        internal static void DropItemOnCreature(Client client, byte inventorySlot, uint targetId, byte count)
        {
            throw new NotImplementedException();
        }

        internal static void DropGoldOnCreature(Client client, uint amount, uint targetId)
        {
            throw new NotImplementedException();
        }

        internal static void RequestProfile(Client client)
        {
            client.Enqueue(Server.Packets.ProfileSelf(client.User));
        }

        internal static void RequestGroup(Client client, GroupRequestType type, string targetName, GroupBox box)
        {
            lock (Sync)
            {
                User targetUser;

                switch (type)
                {
                    case GroupRequestType.Invite:
                        if (!World.TryGetUser(targetName, out targetUser, client.User.Map))                                                                                     //if target user doesnt exist
                            client.Enqueue(Server.Packets.ServerMessage(ServerMessageType.ActiveMessage, $@"{targetName} is not near."));
                        else if (targetUser.IgnoreList.Contains(client.User.Name))                                                     //if theyre on the ignore list, return
                            return;
                        else if (!client.User.UserOptions.Group)                                                                                                                //if your grouping is turned off, let them know
                            client.Enqueue(Server.Packets.ServerMessage(ServerMessageType.ActiveMessage, $@"Grouping is disabled."));
                        else if (!targetUser.UserOptions.Group)                                                                                                                 //if the targets grouping is turned off, let them know
                            client.Enqueue(Server.Packets.ServerMessage(ServerMessageType.ActiveMessage, $@"{targetName} is not accepting group invites."));
                        else if (client.User.Grouped)                                                                                                                                   //else if this we are already in a group
                        {
                            if (client.User == targetUser)                                                                                                                                  //and we're trying to group ourself
                            {
                                if (targetUser.Group.TryRemove(client.User))                                                                                                                    //leave the group
                                    client.Enqueue(Server.Packets.ServerMessage(ServerMessageType.ActiveMessage, "You have left the group."));
                            }
                            else if (!targetUser.Grouped)                                                                                                                                   //else if the target isnt grouped
                                targetUser.Client.Enqueue(Server.Packets.GroupRequest(GroupRequestType.Request, client.User.Name));
                            else if (targetUser.Group != client.User.Group)                                                                                                                 //else if the target's group isnt the same as our group
                                client.Enqueue(Server.Packets.ServerMessage(ServerMessageType.ActiveMessage, $@"{targetName} is already in a group."));
                            else if (client.User.Group.Leader == client.User)                                                                                                               //else if we're the leader of the group
                            {
                                if (client.User.Group.TryRemove(targetUser, true))                                                                                                               //kick them from the group
                                    client.Enqueue(Server.Packets.ServerMessage(ServerMessageType.ActiveMessage, $@"You have kicked {targetName} from the group."));
                            }
                            else                                                                                                                                                            //else we cant kick them, just say theyre in our group
                                client.Enqueue(Server.Packets.ServerMessage(ServerMessageType.ActiveMessage, $@"{targetName} is already in your group."));
                        }
                        else                                                                                                                                                            //else we're not grouped
                        {
                            if (client.User == targetUser)                                                                                                                                  //and we are trying to group ourself
                                client.Enqueue(Server.Packets.ServerMessage(ServerMessageType.ActiveMessage, "You can't form a group alone."));
                            else if (targetUser.Grouped)                                                                                                                                    //else if target is grouped
                                client.Enqueue(Server.Packets.ServerMessage(ServerMessageType.ActiveMessage, $@"{targetName} is already in a group."));
                            else                                                                                                                                                            //else send them a group request
                                targetUser.Client.Enqueue(Server.Packets.GroupRequest(GroupRequestType.Request, client.User.Name));
                        }
                        break;
                    case GroupRequestType.Join:
                        if (!World.TryGetUser(targetName, out targetUser, client.User.Map))
                            client.Enqueue(Server.Packets.ServerMessage(ServerMessageType.ActiveMessage, $@"{targetName} is not near."));
                        else if (targetUser.IgnoreList.Contains(client.User.Name))
                            return;
                        else if (client.User.Grouped)
                            client.Enqueue(Server.Packets.ServerMessage(ServerMessageType.ActiveMessage, "You are already in a group."));
                        else
                        {
                            if (client.User == targetUser)
                                client.Enqueue(Server.Packets.ServerMessage(ServerMessageType.ActiveMessage, "You can't form a group alone."));
                            else if (targetUser.Grouped)
                                client.Enqueue(Server.Packets.ServerMessage(ServerMessageType.ActiveMessage, $@"{targetName} is already in a group."));
                            else
                            {
                                Group group = new Group(targetUser, client.User);
                                targetUser.Client.Enqueue(Server.Packets.ServerMessage(ServerMessageType.ActiveMessage, $@"You form a group with {client.User.Name}"));
                                client.Enqueue(Server.Packets.ServerMessage(ServerMessageType.ActiveMessage, $@"You form a group with {targetName}"));
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
            client.User.UserOptions.Toggle(UserOption.Group);

            if (client.User.Grouped)
                if (client.User.Group.TryRemove(client.User))
                    client.User.Group = null;

            client.Enqueue(Server.Packets.ProfileSelf(client.User));
            client.Enqueue(Server.Packets.ServerMessage(ServerMessageType.UserOptions, client.User.UserOptions.ToString(UserOption.Group)));
        }

        internal static void SwapSlot(Client client, Pane pane, byte origSlot, byte endSlot)
        {
            bool success = false;
            switch(pane)
            {
                case Pane.Inventory:
                    success = client.User.Inventory.TrySwap(origSlot, endSlot);
                    break;
                case Pane.SkillBook:
                    success = client.User.SkillBook.TrySwap(origSlot, endSlot);
                    break;
                case Pane.SpellBook:
                    success = client.User.SpellBook.TrySwap(origSlot, endSlot);
                    break;
                default:
                    success = false;
                    break;
            }

            if (success)
            {
                client.Enqueue(Server.Packets.RemoveItem(origSlot));
                client.Enqueue(Server.Packets.RemoveItem(endSlot));

                if (client.User.Inventory[origSlot] != null)
                    client.Enqueue(Server.Packets.AddItem(client.User.Inventory[origSlot]));
                if (client.User.Inventory[endSlot] != null)
                    client.Enqueue(Server.Packets.AddItem(client.User.Inventory[endSlot]));
            }
        }

        internal static void RequestRefresh(Client client)
        {
            World.Refresh(client);
        }

        internal static void RequestDialog(Client client, byte objType, uint objId, uint pursuitId, byte[] args)
        {
            throw new NotImplementedException();
        }

        internal static void ActiveDialog(Client client, byte objType, uint objId, ushort pursuitId, ushort dialogId)
        {
            throw new NotImplementedException();
        }

        internal static void Boards()
        {
            throw new NotImplementedException();
        }

        internal static void UseSkill(Client client, byte slot)
        {
            throw new NotImplementedException();
        }

        internal static void ClickWorldMap(Client client, ushort mapId, Point point)
        {
            World.AddObjectToMap(client.User, new Location(mapId, point));
        }

        internal static void ClickObject(Client client, int objectId)
        {
            //dont allow this to be spammed
            if (DateTime.UtcNow.Subtract(client.LastClickObj).TotalSeconds < 1)
                return;
            else
                client.LastClickObj = DateTime.UtcNow;

            if (objectId == client.User.Id)
                client.Enqueue(Server.Packets.ProfileSelf(client.User));
            else
            {
                VisibleObject obj;
                if (World.TryGetVisibleObject(objectId, out obj, client.User.Map))
                {
                    if (obj is Monster)
                        client.Enqueue(Server.Packets.ServerMessage(ServerMessageType.OrangeBar1, obj.Name));
                    if (obj is User)
                        client.Enqueue(Server.Packets.Profile(obj as User));
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

            MapObject obj;

            if (World.TryGetMapObject(clickPoint, out obj, client.User.Map))
            {

                if (obj is Door)
                {
                    (obj as Door).Toggle();
                    client.Enqueue(Server.Packets.Door(obj as Door));
                }
                //do things
            }
        }

        internal static void RemoveEquipment(Client client, EquipmentSlot slot)
        {
            Item item;
            if (client.User.Equipment.TryGetRemove((byte)slot, out item))
            {
                client.User.Inventory.AddToNextSlot(item);
                client.Enqueue(Server.Packets.RemoveEquipment(slot));

                //set hp/mp?
                client.Enqueue(Server.Packets.Attributes(client.User.IsAdmin, StatUpdateFlags.Primary, client.User.Attributes));
            }
        }

        internal static void KeepAlive(Client client, byte a, byte b)
        {
            client.Enqueue(Server.Packets.HeartbeatA(a, b));
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

            client.Enqueue(Server.Packets.Attributes(client.User.IsAdmin, StatUpdateFlags.Primary, client.User.Attributes));
        }

        internal static void Exchange(Client client, byte type, uint targetId)
        {
            throw new NotImplementedException();
        }

        internal static void Exchange(Client client, byte type, uint targetId, byte slot)
        {
            throw new NotImplementedException();
        }

        internal static void Exchange(Client client, byte type, uint targetId, byte slot, byte count)
        {
            throw new NotImplementedException();
        }

        internal static void Exchange(Client client, byte type, uint targetId, uint amount)
        {
            throw new NotImplementedException();
        }

        internal static void Exchange(Client client, byte type)
        {
            throw new NotImplementedException();
        }

        internal static void RequestLoginMessage(bool send, Client client)
        {
            client.Enqueue(Server.Packets.LobbyNotification(send, 0, Server.LoginMessage));
        }

        internal static void BeginChant(Client client)
        {
            throw new NotImplementedException();
        }

        internal static void DisplayChant(Client client, string chant)
        {
            throw new NotImplementedException();
        }

        internal static void Personal(Client client, ushort totalLength, ushort portraitLength, byte[] portraitData, string profileMsg)
        {
            client.User.Personal = new Personal(portraitData, profileMsg);
        }

        internal static void RequestServerTable(Client client, byte type)
        {
            if (type == 1)
                client.Enqueue(Server.Packets.ServerTable(Server.Table));
            else
                client.Redirect(new Redirect(client, ServerType.Lobby));
        }

        internal static void RequestHomepage(Client client)
        {
            client.Enqueue(Server.Packets.LobbyControls(3, @"http://www.darkages.com"));
        }

        internal static void SynchronizeTicks(Client client, TimeSpan serverTicks, TimeSpan clientTicks)
        {
            client.Enqueue(Server.Packets.HeartbeatB());
        }

        internal static void ChangeSoocialStatus(Client client, SocialStatus status)
        {
            client.User.SocialStatus = status;
        }

        internal static void RequestMetaFile(Client client, bool all)
        {
            client.Enqueue(Server.Packets.Metafile(all));
        }
    }
}
