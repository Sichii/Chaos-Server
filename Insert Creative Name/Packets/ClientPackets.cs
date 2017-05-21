using System;

namespace Insert_Creative_Name
{
    internal class ClientPackets
    {
        internal ClientPacketHandler[] Handlers { get; private set; }

        internal ClientPackets()
        {
            Handlers[3] = new ClientPacketHandler(PacketHandler_0x03_Login);
            Handlers[4] = new ClientPacketHandler(PacketHandler_0x04_CreateCharacter);
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
            Handlers[77] = new ClientPacketHandler(PacketHandler_0x4D_BeginChant);
            Handlers[78] = new ClientPacketHandler(PacketHandler_0x4E_Chant);
            Handlers[79] = new ClientPacketHandler(PacketHandler_0x4F_PortraitText);
            Handlers[87] = new ClientPacketHandler(PacketHandler_0x57_ServerSelect);
            Handlers[117] = new ClientPacketHandler(PacketHandler_0x75_HeartBeatTimer);
            Handlers[121] = new ClientPacketHandler(PacketHandler_0x79_SocialStatus);
            Handlers[123] = new ClientPacketHandler(PacketHandler_0x7B_MetafileRequest);
        }

        private bool PacketHandler_0x03_Login(Client client, ClientPacket packet)
        {
            string name = packet.ReadString8();
            string pw = packet.ReadString8();
            packet.ReadByte();
            packet.ReadByte();
            packet.ReadUInt32();
            packet.ReadUInt16();
            packet.ReadUInt32();
            packet.ReadUInt16();
            packet.ReadByte();
            return true;
        }

        private bool PacketHandler_0x04_CreateCharacter(Client client, ClientPacket packet)
        {
            byte hairStyle = packet.ReadByte(); //1-17
            byte gender = packet.ReadByte(); //1 or 2
            byte hairColor = packet.ReadByte(); //1-13
            return true;
        }
        private bool PacketHandler_0x05_RequestMapData(Client client, ClientPacket packet)
        {
            /*
            client.Enqueue(client.ServerPackets.MapData);
            */
            return true;
        }
        private bool PacketHandler_0x06_Walk(Client client, ClientPacket packet)
        {
            Direction direction = (Direction)packet.ReadByte();
            int stepCount = packet.ReadByte();
            return true;
        }
        private bool PacketHandler_0x07_Pickup(Client client, ClientPacket packet)
        {
            byte inventorySlot = packet.ReadByte();
            Point groundPoint = packet.ReadPoint();
            return true;
        }
        private bool PacketHandler_0x08_Drop(Client client, ClientPacket packet)
        {
            byte inventorySlot = packet.ReadByte();
            Point groundPoint = packet.ReadPoint();
            int count = packet.ReadInt32();
            return true;
        }
        private bool PacketHandler_0x0B_ClientExit(Client client, ClientPacket packet)
        {
            bool requestExit = packet.ReadBoolean();
            //if requestexit, send exit confirmation 4C
            //when the client gets exit confirmation, it will resend this packet except false
            //then log off
            return true;
        }
        private bool PacketHandler_0x0E_PublicChat(Client client, ClientPacket packet)
        {
            //shout= 1  normal=0
            bool isShout = packet.ReadBoolean();
            string message = packet.ReadString8();

            return false;
        }
        private bool PacketHandler_0x0F_UseSpell(Client client, ClientPacket packet)
        {
            byte slot = packet.ReadByte();
            //if this is the end of the packet
            if (packet.Position == packet.Data.Length - 1)
            {
                //then they casted the spell on themself
            }
            else
            {
                //otherwise, they casted on a target
                int targetId = packet.ReadInt32();
                Point targetPoint = packet.ReadPoint();
            }
            return true;
        }

        private bool PacketHandler_0x10_ClientJoin(Client client, ClientPacket packet)
        {
            byte seed = packet.ReadByte();
            string key = packet.ReadString8();
            string name = packet.ReadString8();
            uint id = packet.ReadUInt32();
            client.Crypto = new Crypto(seed, key, name);

            return true;
        }
        private bool PacketHandler_0x11_Turn(Client client, ClientPacket packet)
        {
            Direction direction = (Direction)packet.ReadByte();
            return true;
        }

        private bool PacketHandler_0x13_Spacebar(Client client, ClientPacket packet)
        {
            //use all assails, cancel casting
            return true;
        }
        private bool PacketHandler_0x18_RequestWorldList(Client client, ClientPacket packet)
        {
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
            return true;
        }
        private bool PacketHandler_0x19_Whisper(Client client, ClientPacket packet)
        {
            string targetName = packet.ReadString8();
            string message = packet.ReadString8();
            return true;
        }
        private bool PacketHandler_0x1B_UserOptions(Client client, ClientPacket packet)
        {
            /*
            0 = options request
            the request sends the client a long string
            SendSystemMessage(7, options);
            where options is literally a giant string containing the text for the whole pane
            Use Group Window and the rest are client side
            */

            UserOption option = (UserOption)packet.ReadByte();
            return true;
        }
        private bool PacketHandler_0x1C_UseItem(Client client, ClientPacket packet)
        {
            byte slot = packet.ReadByte();

            return true;
        }
        private bool PacketHandler_0x1D_Emote(Client client, ClientPacket packet)
        {
            byte index = packet.ReadByte();
            if (index <= 35)
                index += 9;

            //client.Enqueue(client.ServerPackets.CreatureAnimation(client.user.id, index, 120));
            return true;
        }
        private bool PacketHandler_0x24_DropGold(Client client, ClientPacket packet)
        {
            uint amount = packet.ReadUInt32();
            Point groundPoint = packet.ReadPoint();

            return true;
        }
        private bool PacketHandler_0x29_DropItemOnCreature(Client client, ClientPacket packet)
        {
            byte inventorySlot = packet.ReadByte();
            uint targetId = packet.ReadUInt32();
            byte count = packet.ReadByte();

            //if target is an merchant or monster, put it in their drop pile
            //if it's a user start an exchange
            return true;
        }
        private bool PacketHandler_0x2A_DropGoldOnCreature(Client client, ClientPacket packet)
        {
            uint amount = packet.ReadUInt32();
            uint targetId = packet.ReadUInt32();

            //if target is an merchant or monster, put it in their drop pile
            //if it's a user start an exchange
            return true;
        }
        private bool PacketHandler_0x2D_ProfileRequest(Client client, ClientPacket packet)
        {
            /*
             client.Enqueue(client.ServerPackets.ProfileSelf);
             */
            return true;
        }
        private bool PacketHandler_0x2E_GroupRequest(Client client, ClientPacket packet)
        {
            //2 = invite, 3 = join
            byte type = packet.ReadByte();
            string targetName = packet.ReadString8();

            return true;
        }
        private bool PacketHandler_0x2F_ToggleGroup(Client client, ClientPacket packet)
        {
            //toggle group allowance

            return true;
        }
        private bool PacketHandler_0x30_SwapSlot(Client client, ClientPacket packet)
        {
            //0 = Items
            //1 = All Spells
            //2 = All Skills
            byte pane = packet.ReadByte();
            //(tem)1 - 35, (med)37 - 71, , (world)73 - 88
            //items: 1-59
            byte origSlot = packet.ReadByte();
            byte endSlot = packet.ReadByte();
            return true;
        }
        private bool PacketHandler_0x38_RefreshRequest(Client client, ClientPacket packet)
        {
            //send them things
            //client.Enqueue(client.ServerPackets.RefreshResponse());
            return true;
        }
        private bool PacketHandler_0x39_Pursuit(Client client, ClientPacket packet)
        {
            byte objType = packet.ReadByte(); //almost always 1
            int objId = packet.ReadInt32(); //id of object
            int pursuitId = packet.ReadUInt16(); //what they want to do
            /*
            usually this is the end, but sometimes theres more
            the only thing i know uses this is repairing specific items
            format is:
            byte x = packet.ReadByte(); //always 1
            byte slot = packet.Readbyte(); //slot of item to replair
            */
            byte[] args = packet.ReadBytes((packet.Data.Length - 1) - packet.Position);

            return true;
        }
        private bool PacketHandler_0x3A_DialogResponse(Client client, ClientPacket packet)
        {
            byte objType = packet.ReadByte(); //almost always 1
            int objId = packet.ReadInt32(); //id of object
            ushort pursuitId = packet.ReadUInt16(); //the pursuit theyre on
            ushort dialogId = packet.ReadUInt16(); //id of the dialog that comes next

            return true;
        }

        //this packet is literally retarded
        private bool PacketHandler_0x3B_Boards(Client client, ClientPacket packet)
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
                        switch(packet.ReadSByte()) //board controls
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
            return true;
        }
        private bool PacketHandler_0x3E_UseSkill(Client client, ClientPacket packet)
        {
            byte slot = packet.ReadByte();

            return true;
        }

        private bool PacketHandler_0x3F_ClickWorldMap(Client client, ClientPacket packet)
        {
            uint mapId = packet.ReadUInt32();
            Point point = packet.ReadPoint();

            //theyre clicking a worldMapNode here
            return true;
        }
        private bool PacketHandler_0x43_ClickObject(Client client, ClientPacket packet)
        {
            switch (packet.ReadByte()) //click type
            {
                case 1:
                    //they clicked an object, this is it's id
                    uint objectId = packet.ReadUInt32();
                    break;
                case 3:
                    //they clicked a random spot, or something without an id, this is where
                    Point clickPoint = packet.ReadPoint();
                    break;
            }
            return true;
        }
        private bool PacketHandler_0x44_RemoveEquipment(Client client, ClientPacket packet)
        {
            //slot to take off
            byte slot = packet.ReadByte();

            return true;
        }
        private bool PacketHandler_0x45_HeartBeat(Client client, ClientPacket packet)
        {
            //the server sends a beatA and beatB to the client
            //we receive the same bytes in reverse order from the client
            byte b = packet.ReadByte();
            byte a = packet.ReadByte();
            //check these against what we sent
            //check how long it took to receive them from when we sent them
            //generate new values for the next heartbeat

            return true;
        }
        private bool PacketHandler_0x47_AdjustStat(Client client, ClientPacket packet)
        {
            //Possibly create an enum to show which stat was improved last to allow for a *correct* and fast allocation of stats later on.
            Stat stat = (Stat)packet.ReadByte();

            return true;
        }
        private bool PacketHandler_0x4A_ExchangeWindow(Client client, ClientPacket packet)
        {
            switch (packet.ReadByte()) //opt
            {
                case 0: //begin trade
                    {
                        uint targetId = packet.ReadUInt32();
                        //send exchange to target
                        break;
                    }
                case 1: //add nonstackable item
                    {
                        uint targetId = packet.ReadUInt32();
                        byte slot = packet.ReadByte();
                        break;
                    }
                case 2: //add stackable item
                    {
                        uint targetId = packet.ReadUInt32();
                        byte slot = packet.ReadByte();
                        byte count = packet.ReadByte();
                        break;
                    }
                case 3: //add gold
                    {
                        uint targetId = packet.ReadUInt32();
                        uint amount = packet.ReadUInt32();
                        break;
                    }
                case 4: //cancel trade
                    //trade was canceled by this client
                    break;
                case 5: //accept trade
                    //trade was accepted by this client
                    break;
            }
            return true;
        }
        private bool PacketHandler_0x4D_BeginChant(Client client, ClientPacket packet)
        {
            //this client is chanting

            return true;
        }
        private bool PacketHandler_0x4E_Chant(Client client, ClientPacket packet)
        {
            string chant = packet.ReadString8();
            //check if theyre chanting
            //if theyre chanting send a caption

            return true;
        }
        private bool PacketHandler_0x4F_PortraitText(Client client, ClientPacket packet)
        {
            packet.ReadInt16(); //dunno
            Portrait portrait = new Portrait(packet.ReadBytes(packet.ReadUInt16()));
            string profileText = packet.ReadString16();

            return true;
        }
        private bool PacketHandler_0x57_ServerSelect(Client client, ClientPacket packet)
        {
            //no idea
            return true;
        }
        private bool PacketHandler_0x75_HeartBeatTimer(Client client, ClientPacket packet)
        {
            TimeSpan serverTimer = new TimeSpan(packet.ReadUInt32());
            TimeSpan clientTimer = new TimeSpan(packet.ReadUInt32());
            return true;
        }
        private bool PacketHandler_0x79_SocialStatus(Client client, ClientPacket packet)
        {
            SocialStatus status = (SocialStatus)packet.ReadByte();
            return true;
        }
        private bool PacketHandler_0x7B_MetafileRequest(Client client, ClientPacket packet)
        {
            /*
            if (packet.ReadBoolean())
                client.Enqueue(client.ServerPackets.Metafile);
                */

            return true;
        }
    }
}
