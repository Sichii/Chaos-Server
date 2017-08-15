using System;
using System.Linq;

namespace Chaos
{
    internal sealed class ClientPackets
    {
        internal delegate void Handler(Client client, ClientPacket packet);
        internal static Handler[] Handlers
        {
            get
            {
                ClientPackets cp = new ClientPackets();

                Handler[] handles = new Handler[byte.MaxValue];
                handles[0] = new Handler(cp.JoinServer);
                handles[2] = new Handler(cp.CreateChar1);
                handles[3] = new Handler(cp.Login);
                handles[4] = new Handler(cp.CreateChar2);
                handles[5] = new Handler(cp.RequestMapData);
                handles[6] = new Handler(cp.Walk);
                handles[7] = new Handler(cp.Pickup);
                handles[8] = new Handler(cp.Drop);
                handles[11] = new Handler(cp.ExitClient);
                handles[13] = new Handler(cp.Ignore);
                handles[14] = new Handler(cp.PublicChat);
                handles[15] = new Handler(cp.UseSpell);
                handles[16] = new Handler(cp.JoinClient);
                handles[17] = new Handler(cp.Turn);
                handles[19] = new Handler(cp.SpaceBar);
                handles[24] = new Handler(cp.RequestWorldList);
                handles[25] = new Handler(cp.Whisper);
                handles[27] = new Handler(cp.ToggleUserOption);
                handles[28] = new Handler(cp.UseItem);
                handles[29] = new Handler(cp.AnimateUser);
                handles[36] = new Handler(cp.DropGold);
                handles[38] = new Handler(cp.ChangePassword);
                handles[41] = new Handler(cp.DropItemOnCreature);
                handles[42] = new Handler(cp.DropGoldOnCreature);
                handles[45] = new Handler(cp.RequestProfile);
                handles[46] = new Handler(cp.RequestGroup);
                handles[47] = new Handler(cp.ToggleGroup);
                handles[48] = new Handler(cp.SwapSlot);
                handles[56] = new Handler(cp.RequestRefresh);
                handles[57] = new Handler(cp.RequestDialog);
                handles[58] = new Handler(cp.ReplyDialog);
                handles[59] = new Handler(cp.Board);
                handles[62] = new Handler(cp.UseSkill);
                handles[63] = new Handler(cp.ClickWorldMap);
                handles[67] = new Handler(cp.ClickObject);
                handles[68] = new Handler(cp.RemoveEquipment);
                handles[69] = new Handler(cp.KeepAlive);
                handles[71] = new Handler(cp.ChangeStat);
                handles[74] = new Handler(cp.Exchange);
                handles[75] = new Handler(cp.RequestLoginMessage);
                handles[77] = new Handler(cp.BeginChant);
                handles[78] = new Handler(cp.DisplayChant);
                handles[79] = new Handler(cp.Personal);
                handles[87] = new Handler(cp.RequestServerTable);
                handles[104] = new Handler(cp.RequestHomepage);
                handles[117] = new Handler(cp.SynchronizeTicks);
                handles[121] = new Handler(cp.ChangeSocialStatus);
                handles[123] = new Handler(cp.RequestMetaFile);

                return handles;
            }
        }

        internal ClientPackets() { }

        private void JoinServer(Client client, ClientPacket packet)
        {
            Game.JoinServer(client);
        }

        private void CreateChar1(Client client, ClientPacket packet)
        {
            string name = packet.ReadString8();
            string password = packet.ReadString8();

            Game.CreateChar1(client, name, password);
        }

        private void Login(Client client, ClientPacket packet)
        {
            string name = packet.ReadString8();
            string pw = packet.ReadString8();
            //useless crap
            packet.ReadByte();
            packet.ReadByte();
            packet.ReadUInt32();
            packet.ReadUInt16();
            packet.ReadUInt32();
            packet.ReadUInt16();
            packet.ReadByte();

            Game.Login(client, name, pw);
        }

        private void CreateChar2(Client client, ClientPacket packet)
        {
            byte hairStyle = packet.ReadByte(); //1-17
            Gender gender = (Gender)packet.ReadByte(); //1 or 2
            byte hairColor = packet.ReadByte(); //1-13

            Game.CreateChar2(client, hairStyle, gender, hairColor);
        }

        private void RequestMapData(Client client, ClientPacket packet)
        {
            Game.RequestMapData(client);
        }
        private void Walk(Client client, ClientPacket packet)
        {
            Direction direction = (Direction)packet.ReadByte();
            int stepCount = packet.ReadByte();

            Game.Walk(client, direction, stepCount);
        }
        private void Pickup(Client client, ClientPacket packet)
        {
            byte inventorySlot = packet.ReadByte();
            Point groundPoint = packet.ReadPoint();

            Game.Pickup(client, inventorySlot, groundPoint);
        }
        private void Drop(Client client, ClientPacket packet)
        {
            byte inventorySlot = packet.ReadByte();
            Point groundPoint = packet.ReadPoint();
            int count = packet.ReadInt32();

            Game.Drop(client, inventorySlot, groundPoint, count);
        }
        private void ExitClient(Client client, ClientPacket packet)
        {
            bool requestExit = packet.ReadBoolean();

            Game.ExitClient(client, requestExit);
        }
        private void Ignore(Client client, ClientPacket packet)
        {
            IgnoreType type = (IgnoreType)packet.ReadByte();
            string targetName = null;

            if (type != IgnoreType.Request)
                targetName = packet.ReadString8();

            Game.Ignore(client, type, targetName);
        }
        private void PublicChat(Client client, ClientPacket packet)
        {
            PublicMessageType type = (PublicMessageType)packet.ReadByte();
            string message = packet.ReadString8();

            Game.PublicChat(client, type, message);
        }
        private void UseSpell(Client client, ClientPacket packet)
        {
            byte slot = packet.ReadByte();
            int targetId = client.User.Id;
            Point targetPoint = client.User.Point;

            //if this isnt the end of the packet
            if (packet.Position != packet.Data.Length - 1)
            {
                targetId = packet.ReadInt32();
                targetPoint = packet.ReadPoint();
            }

            Game.UseSpell(client, slot, targetId, targetPoint);
        }
        private void JoinClient(Client client, ClientPacket packet)
        {
            byte seed = packet.ReadByte();
            byte keyLength = packet.ReadByte();
            byte[] key = packet.ReadBytes(keyLength);
            string name = packet.ReadString8();
            uint id = packet.ReadUInt32();

            Redirect redirect = client.Server.Redirects.FirstOrDefault(r => r.Id == id);

            if (redirect != null)
            {
                client.ServerType = redirect.Type;
                client.Server.Redirects.Remove(redirect);
                Game.JoinClient(client, seed, key, name, id);
            }
            else
                client.Disconnect();

        }
        private void Turn(Client client, ClientPacket packet)
        {
            Direction direction = (Direction)packet.ReadByte();

            Game.Turn(client, direction);
        }

        private void SpaceBar(Client client, ClientPacket packet)
        {
            Game.SpaceBar(client);
        }
        private void RequestWorldList(Client client, ClientPacket packet)
        {
            Game.RequestWorldList(client);
        }
        private void Whisper(Client client, ClientPacket packet)
        {
            string targetName = packet.ReadString8();
            string message = packet.ReadString8();

            Game.Whisper(client, targetName, message);
        }
        private void ToggleUserOption(Client client, ClientPacket packet)
        {
            UserOption option = (UserOption)packet.ReadByte();

            Game.ToggleUserOption(client, option);
        }
        private void UseItem(Client client, ClientPacket packet)
        {
            byte slot = packet.ReadByte();

            Game.UseItem(client, slot);
        }
        private void AnimateUser(Client client, ClientPacket packet)
        {
            byte index = packet.ReadByte();
            if (index <= 35)
                index += 9;

            Game.AnimateUser(client, index);
        }
        private void DropGold(Client client, ClientPacket packet)
        {
            uint amount = packet.ReadUInt32();
            Point groundPoint = packet.ReadPoint();

            Game.DropGold(client, amount, groundPoint);
        }
        private void ChangePassword(Client client, ClientPacket packet)
        {
            string name = packet.ReadString8();
            string currentPw = packet.ReadString8();
            string newPw = packet.ReadString8();

            Game.ChangePassword(client, name, currentPw, newPw);
        }
        private void DropItemOnCreature(Client client, ClientPacket packet)
        {
            byte inventorySlot = packet.ReadByte();
            int targetId = packet.ReadInt32();
            byte count = packet.ReadByte();

            Game.DropItemOnCreature(client, inventorySlot, targetId, count);

            //if target is an merchant or monster, put it in their drop pile
            //if it's a user start an exchange
        }
        private void DropGoldOnCreature(Client client, ClientPacket packet)
        {
            uint amount = packet.ReadUInt32();
            int targetId = packet.ReadInt32();

            Game.DropGoldOnCreature(client, amount, targetId);
            //if target is an merchant or monster, put it in their drop pile
            //if it's a user start an exchange
        }
        private void RequestProfile(Client client, ClientPacket packet)
        {
            Game.RequestProfile(client);
        }
        private void RequestGroup(Client client, ClientPacket packet)
        {
            GroupBox box = null;
            GroupRequestType type = (GroupRequestType)packet.ReadByte();

            if (type == GroupRequestType.Groupbox)
            {
                string leader = packet.ReadString8();
                string text = packet.ReadString8();
                packet.ReadByte();
                byte minLevel = packet.ReadByte();
                byte maxLevel = packet.ReadByte();
                byte[] maxOfEach = new byte[6];
                maxOfEach[(byte)BaseClass.Warrior] = packet.ReadByte();
                maxOfEach[(byte)BaseClass.Wizard] = packet.ReadByte();
                maxOfEach[(byte)BaseClass.Rogue] = packet.ReadByte();
                maxOfEach[(byte)BaseClass.Priest] = packet.ReadByte();
                maxOfEach[(byte)BaseClass.Monk] = packet.ReadByte();

                box = new GroupBox(client.User, text, maxLevel, maxOfEach);
            }
            string targetName = packet.ReadString8();

            Game.RequestGroup(client, type, targetName, box);
        }
        private void ToggleGroup(Client client, ClientPacket packet)
        {
            Game.ToggleGroup(client);
            //toggle group allowance
        }
        private void SwapSlot(Client client, ClientPacket packet)
        {
            Pane pane = (Pane)packet.ReadByte();
            byte origSlot = packet.ReadByte();
            byte endSlot = packet.ReadByte();

            Game.SwapSlot(client, pane, origSlot, endSlot);
        }
        private void RequestRefresh(Client client, ClientPacket packet)
        {
            //send them things
            //client.Enqueue(client.ServerPackets.RefreshResponse());

            Game.RequestRefresh(client);
        }
        private void RequestDialog(Client client, ClientPacket packet)
        {
            byte objType = packet.ReadByte(); //almost always 1
            uint objId = packet.ReadUInt32(); //id of object
            uint pursuitId = packet.ReadUInt16(); //what they want to do
            /*
            usually this is the end, but sometimes theres more
            the only thing i know uses this is repairing specific items
            format is:
            byte x = packet.ReadByte(); //always 1
            byte slot = packet.Readbyte(); //slot of item to replair
            */
            byte[] args = packet.ReadBytes((packet.Data.Length - 1) - packet.Position);

            Game.RequestDialog(client, objType, objId, pursuitId, args);
        }
        private void ReplyDialog(Client client, ClientPacket packet)
        {
            byte objType = packet.ReadByte(); //almost always 1
            uint objId = packet.ReadUInt32(); //id of object
            ushort pursuitId = packet.ReadUInt16(); //the pursuit theyre on
            ushort dialogId = packet.ReadUInt16(); //id of the dialog that comes next

            Game.ReplyDialog(client, objType, objId, pursuitId, dialogId);
        }

        //this packet is literally retarded
        private void Board(Client client, ClientPacket packet)
        {
            switch (packet.ReadByte()) //request type
            {
                case 1:
                    //Board List
                    //client.Enqueue(client.ServerPackets.BulletinBoard);
                    break;
                case 2:
                    {
                        //Post list for boardNum
                        ushort boardNum = packet.ReadUInt16();
                        ushort startPostNum = packet.ReadUInt16(); //you send the newest mail first, which will have the highest number. startPostNum counts down.
                        //packet.ReadByte() is always 0xF0(240) ???
                        //the client spam requests this like holy fuck, put a timer on this so you only send 1 packet
                        break;
                    }
                case 3:
                    {
                        //Post
                        ushort boardNum = packet.ReadUInt16();
                        ushort postId = packet.ReadUInt16(); //the post number they want, counting up (what the fuck?)
                        //mailbox = boardNum 0
                        //otherwise boardnum is the index of the board you're accessing
                        switch (packet.ReadSByte()) //board controls
                        {
                            case -1: //clicked next for older post
                                break;
                            case 0: //requested a specific post from the post list
                                break;
                            case 1: //clicked previous for newer post
                                break;
                        }
                        break;
                    }
                case 4: //new post
                    {
                        ushort boardNum = packet.ReadUInt16();
                        string subject = packet.ReadString8();
                        string message = packet.ReadString16();
                        break;
                    }
                case 5: //delete post
                    {
                        ushort boardNum = packet.ReadUInt16();
                        ushort postId = packet.ReadUInt16(); //the post number they want to delete, counting up
                        break;
                    }

                case 6: //send mail
                    {
                        ushort boardNum = packet.ReadUInt16();
                        string targetName = packet.ReadString8();
                        string subject = packet.ReadString8();
                        string message = packet.ReadString16();
                        break;
                    }
                case 7: //highlight message
                    {
                        ushort boardNum = packet.ReadUInt16();
                        ushort postId = packet.ReadUInt16();
                        break;
                    }
            }

            Game.Boards();
        }
        private void UseSkill(Client client, ClientPacket packet)
        {
            byte slot = packet.ReadByte();

            Game.UseSkill(client, slot);
        }

        private void ClickWorldMap(Client client, ClientPacket packet)
        {
            uint mapId = packet.ReadUInt32();
            Point point = packet.ReadPoint();

            Game.ClickWorldMap(client, (ushort)mapId, point);
            //theyre clicking a worldMapNode here
        }
        private void ClickObject(Client client, ClientPacket packet)
        {
            byte type = packet.ReadByte();
            switch (type) //click type
            {
                case 1:
                    //they clicked an object, this is it's id
                    int objectId = packet.ReadInt32();
                    Game.ClickObject(client, objectId);
                    break;
                case 3:
                    //they clicked a random spot, or something without an id, this is where
                    Point clickPoint = packet.ReadPoint();
                    Game.ClickObject(client, clickPoint);
                    break;
            }

        }
        private void RemoveEquipment(Client client, ClientPacket packet)
        {
            //slot to take off
            EquipmentSlot slot = (EquipmentSlot)packet.ReadByte();

            Game.RemoveEquipment(client, slot);
        }
        private void KeepAlive(Client client, ClientPacket packet)
        {
            //the server sends a beatA and beatB to the client
            //we receive the same bytes in reverse order from the client
            byte b = packet.ReadByte();
            byte a = packet.ReadByte();

            Game.KeepAlive(client, a, b);
            //check these against what we sent
            //check how long it took to receive them from when we sent them
            //generate new values for the next heartbeat
        }
        private void ChangeStat(Client client, ClientPacket packet)
        {
            Stat stat = (Stat)packet.ReadByte();

            Game.ChangeStat(client, stat);
        }
        private void Exchange(Client client, ClientPacket packet)
        {
            ExchangeType type = (ExchangeType)packet.ReadByte();
            switch (type)
            {
                case ExchangeType.BeginTrade:
                    {
                        uint targetId = packet.ReadUInt32();
                        Game.Exchange(client, type, targetId);
                        break;
                    }
                case ExchangeType.AddNonStackable:
                    {
                        uint targetId = packet.ReadUInt32();
                        byte slot = packet.ReadByte();

                        Game.Exchange(client, type, targetId, 0, slot, 0);
                        break;
                    }
                case ExchangeType.AddStackable:
                    {
                        uint targetId = packet.ReadUInt32();
                        byte slot = packet.ReadByte();
                        byte count = packet.ReadByte();
                        Game.Exchange(client, type, targetId, 0, slot, count);
                        break;
                    }
                case ExchangeType.AddGold: //add gold
                    {
                        uint targetId = packet.ReadUInt32();
                        uint amount = packet.ReadUInt32();
                        Game.Exchange(client, type, targetId, amount);
                        break;
                    }
                case ExchangeType.Cancel:
                    //trade was canceled by this client
                case ExchangeType.Accept:
                    //trade was accepted by this client
                    Game.Exchange(client, type);
                    break;
            }
        }
        private void RequestLoginMessage(Client client, ClientPacket packet)
        {
            Game.RequestLoginMessage(packet.Position == packet.Data.Length, client);
        }

        private void BeginChant(Client client, ClientPacket packet)
        {
            //this client is chanting
            Game.BeginChant(client);
        }
        private void DisplayChant(Client client, ClientPacket packet)
        {
            string chant = packet.ReadString8();

            Game.DisplayChant(client, chant);
            //check if theyre chanting
            //if theyre chanting send a caption
        }
        private void Personal(Client client, ClientPacket packet)
        {
            ushort totalLength = packet.ReadUInt16();
            ushort portraitLength = packet.ReadUInt16();
            byte[] portraitData = packet.ReadBytes(portraitLength);
            string profileMsg = packet.ReadString16();

            Game.Personal(client, portraitData, profileMsg);
        }
        private void RequestServerTable(Client client, ClientPacket packet)
        {
            bool requestTable = packet.ReadBoolean(); //1 = table request, else server number in the table
            Game.RequestServerTable(client, requestTable);
        }
        private void RequestHomepage(Client client, ClientPacket packet)
        {
            //i don't believe there's anything here
            Game.RequestHomepage(client);
        }
        private void SynchronizeTicks(Client client, ClientPacket packet)
        {
            //use this to make sure we're in sync
            TimeSpan serverTicks = new TimeSpan(packet.ReadUInt32()); //server ticks
            TimeSpan clientTicks = new TimeSpan(packet.ReadUInt32()); //client ticks
            Game.SynchronizeTicks(client, serverTicks, clientTicks);
        }
        private void ChangeSocialStatus(Client client, ClientPacket packet)
        {
            SocialStatus status = (SocialStatus)packet.ReadByte();
            Game.ChangeSoocialStatus(client, status);
        }
        private void RequestMetaFile(Client client, ClientPacket packet)
        {
            bool all = packet.ReadBoolean();
            Game.RequestMetaFile(client, all);
        }
    }
}
