using System;
using System.Linq;
using System.Text;

namespace Chaos
{
    internal class ClientPackets
    {
        internal ClientPacketHandler[] Handlers { get; private set; }

        internal ClientPackets()
        {
            Handlers = new ClientPacketHandler[150];
            Handlers[0] = new ClientPacketHandler(PacketHandler_0x00_JoinServer);
            Handlers[2] = new ClientPacketHandler(PacketHandler_0x02_CreatCharA);
            Handlers[3] = new ClientPacketHandler(PacketHandler_0x03_Login);
            Handlers[4] = new ClientPacketHandler(PacketHandler_0x04_CreateCharB);
            Handlers[5] = new ClientPacketHandler(PacketHandler_0x05_RequestMapData);
            Handlers[6] = new ClientPacketHandler(PacketHandler_0x06_Walk);
            Handlers[7] = new ClientPacketHandler(PacketHandler_0x07_Pickup);
            Handlers[8] = new ClientPacketHandler(PacketHandler_0x08_Drop);
            Handlers[11] = new ClientPacketHandler(PacketHandler_0x0B_ClientExit);
            Handlers[14] = new ClientPacketHandler(PacketHandler_0x0E_PublicChat);
            Handlers[15] = new ClientPacketHandler(PacketHandler_0x0F_UseSpell);
            Handlers[16] = new ClientPacketHandler(PacketHandler_0x10_ClientJoin);
            Handlers[17] = new ClientPacketHandler(PacketHandler_0x11_Turn);
            Handlers[19] = new ClientPacketHandler(PacketHandler_0x13_Spacebar);
            Handlers[24] = new ClientPacketHandler(PacketHandler_0x18_RequestWorldList);
            Handlers[25] = new ClientPacketHandler(PacketHandler_0x19_Whisper);
            Handlers[27] = new ClientPacketHandler(PacketHandler_0x1B_UserOptions);
            Handlers[28] = new ClientPacketHandler(PacketHandler_0x1C_UseItem);
            Handlers[29] = new ClientPacketHandler(PacketHandler_0x1D_Emote);
            Handlers[36] = new ClientPacketHandler(PacketHandler_0x24_DropGold);
            Handlers[38] = new ClientPacketHandler(PacketHandler_0x26_ChangePassword);
            Handlers[41] = new ClientPacketHandler(PacketHandler_0x29_DropItemOnCreature);
            Handlers[42] = new ClientPacketHandler(PacketHandler_0x2A_DropGoldOnCreature);
            Handlers[45] = new ClientPacketHandler(PacketHandler_0x2D_ProfileRequest);
            Handlers[46] = new ClientPacketHandler(PacketHandler_0x2E_GroupRequest);
            Handlers[47] = new ClientPacketHandler(PacketHandler_0x2F_ToggleGroup);
            Handlers[48] = new ClientPacketHandler(PacketHandler_0x30_SwapSlot);
            Handlers[56] = new ClientPacketHandler(PacketHandler_0x38_RefreshRequest);
            Handlers[57] = new ClientPacketHandler(PacketHandler_0x39_Pursuit);
            Handlers[58] = new ClientPacketHandler(PacketHandler_0x3A_DialogResponse);
            Handlers[59] = new ClientPacketHandler(PacketHandler_0x3B_Boards);
            Handlers[62] = new ClientPacketHandler(PacketHandler_0x3E_UseSkill);
            Handlers[63] = new ClientPacketHandler(PacketHandler_0x3F_ClickWorldMap);
            Handlers[67] = new ClientPacketHandler(PacketHandler_0x43_ClickObject);
            Handlers[68] = new ClientPacketHandler(PacketHandler_0x44_RemoveEquipment);
            Handlers[69] = new ClientPacketHandler(PacketHandler_0x45_HeartBeat);
            Handlers[71] = new ClientPacketHandler(PacketHandler_0x47_AdjustStat);
            Handlers[74] = new ClientPacketHandler(PacketHandler_0x4A_ExchangeWindow);
            Handlers[75] = new ClientPacketHandler(PacketHandler_0x4B_RequestNotification);
            Handlers[77] = new ClientPacketHandler(PacketHandler_0x4D_BeginChant);
            Handlers[78] = new ClientPacketHandler(PacketHandler_0x4E_Chant);
            Handlers[79] = new ClientPacketHandler(PacketHandler_0x4F_PortraitText);
            Handlers[87] = new ClientPacketHandler(PacketHandler_0x57_ServerTable);
            Handlers[104] = new ClientPacketHandler(PacketHandler_0x68_RequestHomepage);
            Handlers[117] = new ClientPacketHandler(PacketHandler_0x75_HeartBeatTimer);
            Handlers[121] = new ClientPacketHandler(PacketHandler_0x79_SocialStatus);
            Handlers[123] = new ClientPacketHandler(PacketHandler_0x7B_MetafileRequest);
        }

        private void PacketHandler_0x00_JoinServer(Client client, ClientPacket packet)
        {
            ProcessPacket.JoinServer(client);
        }

        private void PacketHandler_0x02_CreatCharA(Client client, ClientPacket packet)
        {
            string name = packet.ReadString8();
            string password = packet.ReadString8();

            ProcessPacket.CreateCharA(client, name, password);
        }

        private void PacketHandler_0x03_Login(Client client, ClientPacket packet)
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

            ProcessPacket.Login(client, name, pw);
        }

        private void PacketHandler_0x04_CreateCharB(Client client, ClientPacket packet)
        {
            byte hairStyle = packet.ReadByte(); //1-17
            Gender gender = (Gender)packet.ReadByte(); //1 or 2
            byte hairColor = packet.ReadByte(); //1-13

            ProcessPacket.CreateCharB(client, hairStyle, gender, hairColor);
        }

        private void PacketHandler_0x05_RequestMapData(Client client, ClientPacket packet)
        {
            ProcessPacket.RequestMapData(client);
        }
        private void PacketHandler_0x06_Walk(Client client, ClientPacket packet)
        {
            Direction direction = (Direction)packet.ReadByte();
            int stepCount = packet.ReadByte();

            ProcessPacket.Walk(client, direction, stepCount);
        }
        private void PacketHandler_0x07_Pickup(Client client, ClientPacket packet)
        {
            byte inventorySlot = packet.ReadByte();
            Point groundPoint = packet.ReadPoint();

            ProcessPacket.Pickup(client, inventorySlot, groundPoint);
        }
        private void PacketHandler_0x08_Drop(Client client, ClientPacket packet)
        {
            byte inventorySlot = packet.ReadByte();
            Point groundPoint = packet.ReadPoint();
            int count = packet.ReadInt32();

            ProcessPacket.Drop(client, inventorySlot, groundPoint, count);
        }
        private void PacketHandler_0x0B_ClientExit(Client client, ClientPacket packet)
        {
            bool requestExit = packet.ReadBoolean();

            ProcessPacket.ClientExit(client, requestExit);
            //if requestexit, send exit confirmation 4C
            //when the client gets exit confirmation, it will resend this packet except false
            //then log off
        }
        private void PacketHandler_0x0E_PublicChat(Client client, ClientPacket packet)
        {
            ClientMessageType type = (ClientMessageType)packet.ReadByte();
            string message = packet.ReadString8();

            ProcessPacket.PublicChat(client, type, message);
        }
        private void PacketHandler_0x0F_UseSpell(Client client, ClientPacket packet)
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

            ProcessPacket.UseSpell(client, slot, targetId, targetPoint);
        }
        private void PacketHandler_0x10_ClientJoin(Client client, ClientPacket packet)
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
            }

            ProcessPacket.ClientJoin(client, seed, key, name, id);
        }
        private void PacketHandler_0x11_Turn(Client client, ClientPacket packet)
        {
            Direction direction = (Direction)packet.ReadByte();

            ProcessPacket.Turn(client, direction);
        }

        private void PacketHandler_0x13_Spacebar(Client client, ClientPacket packet)
        {
            ProcessPacket.Spacebar(client);
        }
        private void PacketHandler_0x18_RequestWorldList(Client client, ClientPacket packet)
        {
            ProcessPacket.RequestWorldList(client);

            //there's nothing in this packet, when you receive it it's just a request for the userlist
            //userlist format is
            /*
            packet.WriteInt16(numberOfUsers);
            for(int counter = 0; counter < numberOfUsers; counter++)
            {
                packet.WriteByte(class);
                if(leveldifference <= 5)
                    packet.WriteByte(151);
                else
                    packet.WriteSByte(-1);
                packet.WriteByte(SocialStatus);
                packet.WriteString8(title);
                packet.WriteBoolean(isMaster);
                packet.WriteString8(userName);
            }
            */
        }
        private void PacketHandler_0x19_Whisper(Client client, ClientPacket packet)
        {
            string targetName = packet.ReadString8();
            string message = packet.ReadString8();

            ProcessPacket.Whisper(client, targetName, message);
        }
        private void PacketHandler_0x1B_UserOptions(Client client, ClientPacket packet)
        {
            UserOption option = (UserOption)packet.ReadByte();

            ProcessPacket.UserOptions(client, option);
        }
        private void PacketHandler_0x1C_UseItem(Client client, ClientPacket packet)
        {
            byte slot = packet.ReadByte();

            ProcessPacket.UseItem(client, slot);
        }
        private void PacketHandler_0x1D_Emote(Client client, ClientPacket packet)
        {
            byte index = packet.ReadByte();
            if (index <= 35)
                index += 9;

            ProcessPacket.Emote(client, index);
        }
        private void PacketHandler_0x24_DropGold(Client client, ClientPacket packet)
        {
            uint amount = packet.ReadUInt32();
            Point groundPoint = packet.ReadPoint();

            ProcessPacket.DropGold(client, amount, groundPoint);
        }
        private void PacketHandler_0x26_ChangePassword(Client client, ClientPacket packet)
        {
            string name = packet.ReadString8();
            string currentPw = packet.ReadString8();
            string newPw = packet.ReadString8();

            ProcessPacket.ChangePassword(client, name, currentPw, newPw);
        }
        private void PacketHandler_0x29_DropItemOnCreature(Client client, ClientPacket packet)
        {
            byte inventorySlot = packet.ReadByte();
            uint targetId = packet.ReadUInt32();
            byte count = packet.ReadByte();

            ProcessPacket.DropItemOnCreature(client, inventorySlot, targetId, count);

            //if target is an merchant or monster, put it in their drop pile
            //if it's a user start an exchange
        }
        private void PacketHandler_0x2A_DropGoldOnCreature(Client client, ClientPacket packet)
        {
            uint amount = packet.ReadUInt32();
            uint targetId = packet.ReadUInt32();

            ProcessPacket.DropGoldOnCreature(client, amount, targetId);
            //if target is an merchant or monster, put it in their drop pile
            //if it's a user start an exchange
        }
        private void PacketHandler_0x2D_ProfileRequest(Client client, ClientPacket packet)
        {
            ProcessPacket.ProfileRequest(client);
        }
        private void PacketHandler_0x2E_GroupRequest(Client client, ClientPacket packet)
        {
            Objects.GroupBox box = null;
            //2 = invite, 3 = join, 4 = groupBox, 6 = remove group box
            byte type = packet.ReadByte();
            if (type == 4)
            {
                string leader = packet.ReadString8();
                string groupName = packet.ReadString8();
                packet.ReadByte();
                byte minLevel = packet.ReadByte();
                byte maxLevel = packet.ReadByte();
                byte[] maxOfEach = new byte[6];
                maxOfEach[(byte)BaseClass.Warrior] = packet.ReadByte();
                maxOfEach[(byte)BaseClass.Wizard] = packet.ReadByte();
                maxOfEach[(byte)BaseClass.Rogue] = packet.ReadByte();
                maxOfEach[(byte)BaseClass.Priest] = packet.ReadByte();
                maxOfEach[(byte)BaseClass.Monk] = packet.ReadByte();

                box = new Objects.GroupBox(client.User, groupName, maxLevel, maxOfEach);
            }
            string targetName = packet.ReadString8();

            ProcessPacket.GroupRequest(client, type, targetName, box);
        }
        private void PacketHandler_0x2F_ToggleGroup(Client client, ClientPacket packet)
        {
            ProcessPacket.ToggleGroup(client);
            //toggle group allowance
        }
        private void PacketHandler_0x30_SwapSlot(Client client, ClientPacket packet)
        {
            Pane pane = (Pane)packet.ReadByte();
            byte origSlot = packet.ReadByte();
            byte endSlot = packet.ReadByte();

            ProcessPacket.SwapSlot(client, pane, origSlot, endSlot);
        }
        private void PacketHandler_0x38_RefreshRequest(Client client, ClientPacket packet)
        {
            //send them things
            //client.Enqueue(client.ServerPackets.RefreshResponse());

            ProcessPacket.RefreshRequest(client);
        }
        private void PacketHandler_0x39_Pursuit(Client client, ClientPacket packet)
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

            ProcessPacket.Pursuit(client, objType, objId, pursuitId, args);
        }
        private void PacketHandler_0x3A_DialogResponse(Client client, ClientPacket packet)
        {
            byte objType = packet.ReadByte(); //almost always 1
            uint objId = packet.ReadUInt32(); //id of object
            ushort pursuitId = packet.ReadUInt16(); //the pursuit theyre on
            ushort dialogId = packet.ReadUInt16(); //id of the dialog that comes next

            ProcessPacket.DialogResponse(client, objType, objId, pursuitId, dialogId);
        }

        //this packet is literally retarded
        private void PacketHandler_0x3B_Boards(Client client, ClientPacket packet)
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

            ProcessPacket.Boards();
        }
        private void PacketHandler_0x3E_UseSkill(Client client, ClientPacket packet)
        {
            byte slot = packet.ReadByte();

            ProcessPacket.UseSkill(client, slot);
        }

        private void PacketHandler_0x3F_ClickWorldMap(Client client, ClientPacket packet)
        {
            uint mapId = packet.ReadUInt32();
            Point point = packet.ReadPoint();

            ProcessPacket.ClickWorldMap(client, (ushort)mapId, point);
            //theyre clicking a worldMapNode here
        }
        private void PacketHandler_0x43_ClickObject(Client client, ClientPacket packet)
        {
            byte type = packet.ReadByte();
            switch (type) //click type
            {
                case 1:
                    //they clicked an object, this is it's id
                    int objectId = packet.ReadInt32();
                    ProcessPacket.ClickObject(client, objectId);
                    break;
                case 3:
                    //they clicked a random spot, or something without an id, this is where
                    Point clickPoint = packet.ReadPoint();
                    ProcessPacket.ClickObject(client, clickPoint);
                    break;
            }

        }
        private void PacketHandler_0x44_RemoveEquipment(Client client, ClientPacket packet)
        {
            //slot to take off
            EquipmentSlot slot = (EquipmentSlot)packet.ReadByte();

            ProcessPacket.RemoveEquipment(client, slot);
        }
        private void PacketHandler_0x45_HeartBeat(Client client, ClientPacket packet)
        {
            //the server sends a beatA and beatB to the client
            //we receive the same bytes in reverse order from the client
            byte b = packet.ReadByte();
            byte a = packet.ReadByte();

            ProcessPacket.HeartBeat(client, a, b);
            //check these against what we sent
            //check how long it took to receive them from when we sent them
            //generate new values for the next heartbeat
        }
        private void PacketHandler_0x47_AdjustStat(Client client, ClientPacket packet)
        {
            //Possibly create an enum to show which stat was improved last to allow for a *correct* and fast allocation of stats later on.
            Stat stat = (Stat)packet.ReadByte();

            ProcessPacket.AdjustStat(client, stat);
        }
        private void PacketHandler_0x4A_ExchangeWindow(Client client, ClientPacket packet)
        {
            byte type = packet.ReadByte();
            switch (type) //opt
            {
                case 0: //begin trade
                    {
                        uint targetId = packet.ReadUInt32();
                        ProcessPacket.ExchangeWindow(client, type, targetId);
                        break;
                    }
                case 1: //add nonstackable item
                    {
                        uint targetId = packet.ReadUInt32();
                        byte slot = packet.ReadByte();

                        ProcessPacket.ExchangeWindow(client, type, targetId, slot);
                        break;
                    }
                case 2: //add stackable item
                    {
                        uint targetId = packet.ReadUInt32();
                        byte slot = packet.ReadByte();
                        byte count = packet.ReadByte();
                        ProcessPacket.ExchangeWindow(client, type, targetId, slot, count);
                        break;
                    }
                case 3: //add gold
                    {
                        uint targetId = packet.ReadUInt32();
                        uint amount = packet.ReadUInt32();
                        ProcessPacket.ExchangeWindow(client, type, targetId, amount);
                        break;
                    }
                case 4: //cancel trade
                    //trade was canceled by this client
                case 5: //accept trade
                    //trade was accepted by this client
                    ProcessPacket.ExchangeWindow(client, type);
                    break;
            }
        }
        private void PacketHandler_0x4B_RequestNotification(Client client, ClientPacket packet)
        {
            ProcessPacket.RequestNotification(packet.Position == packet.Data.Length, client);
        }

        private void PacketHandler_0x4D_BeginChant(Client client, ClientPacket packet)
        {
            //this client is chanting
            ProcessPacket.BeginChant(client);
        }
        private void PacketHandler_0x4E_Chant(Client client, ClientPacket packet)
        {
            string chant = packet.ReadString8();

            ProcessPacket.Chant(client, chant);
            //check if theyre chanting
            //if theyre chanting send a caption
        }
        private void PacketHandler_0x4F_PortraitText(Client client, ClientPacket packet)
        {
            ushort totalLength = packet.ReadUInt16();
            ushort portraitLength = packet.ReadUInt16();
            byte[] portraitData = packet.ReadBytes(portraitLength);
            string profileMsg = packet.ReadString16();

            ProcessPacket.PortraitText(client, totalLength, portraitLength, portraitData, profileMsg);
        }
        private void PacketHandler_0x57_ServerTable(Client client, ClientPacket packet)
        {
            byte type = packet.ReadByte(); //1 = table request, else server number in the table
            ProcessPacket.ServerTable(client, type);
        }
        private void PacketHandler_0x68_RequestHomepage(Client client, ClientPacket packet)
        {
            ProcessPacket.RequestHomepage(client);
            //i don't believe there's anything here
        }
        private void PacketHandler_0x75_HeartBeatTimer(Client client, ClientPacket packet)
        {
            //use this to make sure we're in sync
            TimeSpan serverTicks = new TimeSpan(packet.ReadUInt32()); //server ticks
            TimeSpan clientTicks = new TimeSpan(packet.ReadUInt32()); //client ticks
            ProcessPacket.HeartBeatTimer(client, serverTicks, clientTicks);
        }
        private void PacketHandler_0x79_SocialStatus(Client client, ClientPacket packet)
        {
            SocialStatus status = (SocialStatus)packet.ReadByte();
            ProcessPacket.SocialStatus(client, status);
        }
        private void PacketHandler_0x7B_MetafileRequest(Client client, ClientPacket packet)
        {
            bool all = packet.ReadBoolean();
            ProcessPacket.MetafileRequest(client, all);
        }
    }
}
