using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Insert_Creative_Name
{
    internal class ClientPackets
    {
        internal ClientPacketHandler[] Handlers { get; private set; }

        internal ClientPackets()
        {
            Handlers[3] = new ClientPacketHandler(PacketHandler_0x03_Login);
            Handlers[4] = new ClientPacketHandler(PacketHandler_0x04_CreateCharacter);
            Handlers[6] = new ClientPacketHandler(PacketHandler_0x06_Walk);
            Handlers[7] = new ClientPacketHandler(PacketHandler_0x07_Pickup);
            Handlers[14] = new ClientPacketHandler(PacketHandler_0x0E_PublicChat);
            Handlers[15] = new ClientPacketHandler(PacketHandler_0x0F_UseSpell);
            Handlers[16] = new ClientPacketHandler(PacketHandler_0x10_ClientJoin);
            Handlers[17] = new ClientPacketHandler(PacketHandler_0x11_Turn);
            Handlers[24] = new ClientPacketHandler(PacketHandler_0x18_RequestWorldList);
            Handlers[25] = new ClientPacketHandler(PacketHandler_0x19_Whisper);
            Handlers[27] = new ClientPacketHandler(PacketHandler_0x1B_UserOptions);
            Handlers[28] = new ClientPacketHandler(PacketHandler_0x1C_UseItem);
            Handlers[48] = new ClientPacketHandler(PacketHandler_0x30_SwapSlot);
            Handlers[57] = new ClientPacketHandler(PacketHandler_0x39_Pursuit);
            Handlers[58] = new ClientPacketHandler(PacketHandler_0x3A_DialogResponse);
            Handlers[59] = new ClientPacketHandler(PacketHandler_0x3B_Boards);
            Handlers[62] = new ClientPacketHandler(PacketHandler_0x3E_UseSkill);
            Handlers[67] = new ClientPacketHandler(PacketHandler_0x43_ClickObject);
            Handlers[71] = new ClientPacketHandler(PacketHandler_0x47_AdjustStat);
            Handlers[74] = new ClientPacketHandler(PacketHandler_0x4A_ExchangeWindow);
            Handlers[87] = new ClientPacketHandler(PacketHandler_0x57_ServerSelect);
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
        private bool PacketHandler_0x0E_PublicChat(Client client, ClientPacket packet)
        {
            //shout= 1  normal=0
            byte chatType = packet.ReadByte();
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
            int key2 = packet.ReadInt32();
            client.Crypto = new Crypto(seed, key, name);

            return true;
        }
        private bool PacketHandler_0x11_Turn(Client client, ClientPacket packet)
        {
            Direction direction = (Direction)packet.ReadByte();
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
                packet.WriteByte();
                packet.WriteByte(); //one of these is your status (lfg, do not disturb, etc)
                packet.WriteByte();
                packet.WriteString8(title);
                packet.WriteBoolean(hasTitle);
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
        private bool PacketHandler_0x30_SwapSlot(Client client, ClientPacket packet)
        {
            //0 = Items
            //1 = All Spells
            //2 = All Skills
            byte pane = packet.ReadByte();
            //(tem)1 - 35, (med)37 - 71, , (world)73 - 88 
            byte origSlot = packet.ReadByte();
            byte endSlot = packet.ReadByte();
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
        private bool PacketHandler_0x3B_Boards(Client client, ClientPacket packet)
        {
            switch (packet.ReadByte())
            {
                case 1:
                    //Clicking a board
                    break;
                case 2:
                    int boardid = packet.ReadUInt16();
                    int postNum = packet.ReadUInt16();
                    //Asking a board for its contents?
                    break;
            }
            return true;
        }
        private bool PacketHandler_0x3E_UseSkill(Client client, ClientPacket packet)
        {
            byte slot = packet.ReadByte();

            return true;
        }
        private bool PacketHandler_0x43_ClickObject(Client client, ClientPacket packet)
        {
            byte opt = packet.ReadByte();

            switch (opt)
            {
                case 1:
                    //they clicked an object, this is it's id
                    int objectId = packet.ReadInt32();
                    break;
                case 3:
                    //they clicked a random spot, or something without an id, this is where
                    Point clickPoint = packet.ReadPoint();
                    break;
            }
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
            byte opt = packet.ReadByte();

            switch (opt)
            {
                case 1:
                    int targetId = packet.ReadInt32();
                    byte slot = packet.ReadByte();
                    break;
                case 4:
                    //trade was canceled by this client
                    break;
                case 5:
                    //trade was accepted by this client
                    break;
            }
            return true;
        }
        private bool PacketHandler_0x57_ServerSelect(Client client, ClientPacket packet)
        {
            //no idea
            return true;
        }
    }
}
