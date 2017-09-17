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
                handles[(byte)ClientOpCodes.JoinServer] = new Handler(cp.JoinServer);
                handles[(byte)ClientOpCodes.CreateChar1] = new Handler(cp.CreateChar1);
                handles[(byte)ClientOpCodes.Login] = new Handler(cp.Login);
                handles[(byte)ClientOpCodes.CreateChar2] = new Handler(cp.CreateChar2);
                handles[(byte)ClientOpCodes.RequestMapData] = new Handler(cp.RequestMapData);
                handles[(byte)ClientOpCodes.Walk] = new Handler(cp.Walk);
                handles[(byte)ClientOpCodes.Pickup] = new Handler(cp.Pickup);
                handles[(byte)ClientOpCodes.Drop] = new Handler(cp.Drop);
                handles[(byte)ClientOpCodes.ExitClient] = new Handler(cp.ExitClient);
                handles[(byte)ClientOpCodes.Ignore] = new Handler(cp.Ignore);
                handles[(byte)ClientOpCodes.PublicChat] = new Handler(cp.PublicChat);
                handles[(byte)ClientOpCodes.UseSpell] = new Handler(cp.UseSpell);
                handles[(byte)ClientOpCodes.JoinClient] = new Handler(cp.JoinClient);
                handles[(byte)ClientOpCodes.Turn] = new Handler(cp.Turn);
                handles[(byte)ClientOpCodes.SpaceBar] = new Handler(cp.SpaceBar);
                handles[(byte)ClientOpCodes.RequestWorldList] = new Handler(cp.RequestWorldList);
                handles[(byte)ClientOpCodes.Whisper] = new Handler(cp.Whisper);
                handles[(byte)ClientOpCodes.ToggleUserOption] = new Handler(cp.ToggleUserOption);
                handles[(byte)ClientOpCodes.UseItem] = new Handler(cp.UseItem);
                handles[(byte)ClientOpCodes.AnimateUser] = new Handler(cp.AnimateUser);
                handles[(byte)ClientOpCodes.DropGold] = new Handler(cp.DropGold);
                handles[(byte)ClientOpCodes.ChangePassword] = new Handler(cp.ChangePassword);
                handles[(byte)ClientOpCodes.DropItemOnCreature] = new Handler(cp.DropItemOnCreature);
                handles[(byte)ClientOpCodes.DropGoldOnCreature] = new Handler(cp.DropGoldOnCreature);
                handles[(byte)ClientOpCodes.RequestProfile] = new Handler(cp.RequestProfile);
                handles[(byte)ClientOpCodes.RequestGroup] = new Handler(cp.RequestGroup);
                handles[(byte)ClientOpCodes.ToggleGroup] = new Handler(cp.ToggleGroup);
                handles[(byte)ClientOpCodes.SwapSlot] = new Handler(cp.SwapSlot);
                handles[(byte)ClientOpCodes.RequestRefresh] = new Handler(cp.RequestRefresh);
                handles[(byte)ClientOpCodes.RequestPursuit] = new Handler(cp.RequestPursuit);
                handles[(byte)ClientOpCodes.ReplyDialog] = new Handler(cp.ReplyDialog);
                handles[(byte)ClientOpCodes.Board] = new Handler(cp.Board);
                handles[(byte)ClientOpCodes.UseSkill] = new Handler(cp.UseSkill);
                handles[(byte)ClientOpCodes.ClickWorldMap] = new Handler(cp.ClickWorldMap);
                handles[(byte)ClientOpCodes.ClickObject] = new Handler(cp.ClickObject);
                handles[(byte)ClientOpCodes.RemoveEquipment] = new Handler(cp.RemoveEquipment);
                handles[(byte)ClientOpCodes.KeepAlive] = new Handler(cp.KeepAlive);
                handles[(byte)ClientOpCodes.ChangeStat] = new Handler(cp.ChangeStat);
                handles[(byte)ClientOpCodes.Exchange] = new Handler(cp.Exchange);
                handles[(byte)ClientOpCodes.RequestLoginMessage] = new Handler(cp.RequestLoginMessage);
                handles[(byte)ClientOpCodes.BeginChant] = new Handler(cp.BeginChant);
                handles[(byte)ClientOpCodes.DisplayChant] = new Handler(cp.DisplayChant);
                handles[(byte)ClientOpCodes.Personal] = new Handler(cp.Personal);
                handles[(byte)ClientOpCodes.RequestServerTable] = new Handler(cp.RequestServerTable);
                handles[(byte)ClientOpCodes.RequestHomepage] = new Handler(cp.RequestHomepage);
                handles[(byte)ClientOpCodes.SynchronizeTicks] = new Handler(cp.SynchronizeTicks);
                handles[(byte)ClientOpCodes.ChangeSocialStatus] = new Handler(cp.ChangeSocialStatus);
                handles[(byte)ClientOpCodes.RequestMetaFile] = new Handler(cp.RequestMetaFile);

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
        private void RequestPursuit(Client client, ClientPacket packet)
        {
            GameObjectType objType = (GameObjectType)packet.ReadByte(); //gameObjectType
            int objId = packet.ReadInt32(); //id of object
            ushort pid = packet.ReadUInt16(); //what they want to do
            PursuitIds pursuitId = Enum.IsDefined(typeof(PursuitIds), pid) ? (PursuitIds)pid : PursuitIds.None;
            /*
            usually this is the end, but sometimes theres more
            the only thing i know uses this is repairing specific items
            format is:
            byte x = packet.ReadByte(); //always 1
            byte slot = packet.Readbyte(); //slot of item to replair
            */
            byte[] args = packet.ReadBytes(packet.Data.Length - packet.Position);

            Game.RequestPursuit(client, objType, objId, pursuitId, args);
        }
        private void ReplyDialog(Client client, ClientPacket packet)
        {
            GameObjectType objType = (GameObjectType)packet.ReadByte();
            int objId = packet.ReadInt32(); //id of object
            ushort pid = packet.ReadUInt16(); //what they want to do
            PursuitIds pursuitId = Enum.IsDefined(typeof(PursuitIds), pid) ? (PursuitIds)pid : PursuitIds.None;
            ushort dialogId = packet.ReadUInt16(); //+1 current id if next, -1 if previous, same if close

            int position = packet.Position;
            byte[] args = packet.ReadBytes(packet.Data.Length - packet.Position); //other arguments
            packet.Position = position;

            DialogArgsType argsType = DialogArgsType.None;
            byte opt = 0;
            string input = "";
            if(args.Count() > 0)
            {
                argsType = (DialogArgsType)packet.ReadByte();
                switch(argsType)
                {
                    case DialogArgsType.MenuResponse:
                        opt = packet.ReadByte();
                        break;
                    case DialogArgsType.TextResponse:
                        input = packet.ReadString8();
                        break;
                }
            }

            Game.ReplyDialog(client, objType, objId, pursuitId, dialogId, argsType, opt, input);
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
