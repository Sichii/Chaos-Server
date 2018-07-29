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
using System.Text;

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
                handles[(byte)ClientOpCodes.Emote] = new Handler(cp.Emote);
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
            Server.WriteLog($@"[{Enum.GetName(typeof(ClientOpCodes), packet.OpCode)}] Recv> ", client);
            Game.JoinServer(client);
        }

        private void CreateChar1(Client client, ClientPacket packet)
        {
            string name = packet.ReadString8();
            string pw = packet.ReadString8();

            Server.WriteLog($@"Recv [{Enum.GetName(typeof(ClientOpCodes), packet.OpCode).ToUpper()}] Name: {name} | Password: {pw}", client);
            Game.CreateChar1(client, name, pw);
        }

        private void Login(Client client, ClientPacket packet)
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

            Server.WriteLog($@"Recv [{Enum.GetName(typeof(ClientOpCodes), packet.OpCode).ToUpper()}] Name: {name} | Password: {pw}", client);
            Game.Login(client, name, pw);
        }

        private void CreateChar2(Client client, ClientPacket packet)
        {
            byte hairStyle = packet.ReadByte();
            Gender gender = (Gender)packet.ReadByte();
            byte hairColor = packet.ReadByte();

            Server.WriteLog($@"Recv [{Enum.GetName(typeof(ClientOpCodes), packet.OpCode).ToUpper()}] Style: {hairStyle} | Gender: {gender} | Color: {hairColor}", client);
            Game.CreateChar2(client, hairStyle, gender, hairColor);
        }

        private void RequestMapData(Client client, ClientPacket packet)
        {
            Server.WriteLog($@"Recv [{Enum.GetName(typeof(ClientOpCodes), packet.OpCode).ToUpper()}] ", client);
            Game.RequestMapData(client);
        }
        private void Walk(Client client, ClientPacket packet)
        {
            Direction direction = (Direction)packet.ReadByte();
            int stepCount = packet.ReadByte();

            Server.WriteLog($@"Recv [{Enum.GetName(typeof(ClientOpCodes), packet.OpCode).ToUpper()}] Direction: {direction} | StepCount: {stepCount}", client);
            Game.Walk(client, direction, stepCount);
        }
        private void Pickup(Client client, ClientPacket packet)
        {
            byte slot = packet.ReadByte();
            Point groundPoint = packet.ReadPoint();

            Server.WriteLog($@"Recv [{Enum.GetName(typeof(ClientOpCodes), packet.OpCode).ToUpper()}] Slot: {slot} | Point: {groundPoint}", client);
            Game.Pickup(client, slot, groundPoint);
        }
        private void Drop(Client client, ClientPacket packet)
        {
            byte slot = packet.ReadByte();
            Point groundPoint = packet.ReadPoint();
            int count = packet.ReadInt32();

            Server.WriteLog($@"Recv [{Enum.GetName(typeof(ClientOpCodes), packet.OpCode).ToUpper()}] Slot: {slot} | Point: {groundPoint} | Count: {count}", client);
            Game.Drop(client, slot, groundPoint, count);
        }
        private void ExitClient(Client client, ClientPacket packet)
        {
            bool requestExit = packet.ReadBoolean();

            Server.WriteLog($@"Recv [{Enum.GetName(typeof(ClientOpCodes), packet.OpCode).ToUpper()}] Exit: {requestExit}", client);
            Game.ExitClient(client, requestExit);
        }
        private void Ignore(Client client, ClientPacket packet)
        {
            IgnoreType type = (IgnoreType)packet.ReadByte();
            string targetName = null;

            if (type != IgnoreType.Request)
                targetName = packet.ReadString8();

            Server.WriteLog($@"Recv [{Enum.GetName(typeof(ClientOpCodes), packet.OpCode).ToUpper()}] Type: {type} | Target: {targetName ?? "none"}", client);
            Game.Ignore(client, type, targetName);
        }
        private void PublicChat(Client client, ClientPacket packet)
        {
            PublicMessageType type = (PublicMessageType)packet.ReadByte();
            string message = packet.ReadString8();

            Server.WriteLog($@"Recv [{Enum.GetName(typeof(ClientOpCodes), packet.OpCode).ToUpper()}] Type: {type} | Message: {message}", client);
            Game.PublicChat(client, type, message);
        }
        private void UseSpell(Client client, ClientPacket packet)
        {
            byte slot = packet.ReadByte();
            int targetId = client.User.Id;
            Point targetPoint = client.User.Point;

            if (packet.Position != packet.Data.Length)
            {
                targetId = packet.ReadInt32();
                targetPoint = packet.ReadPoint();
            }

            Server.WriteLog($@"Recv [{Enum.GetName(typeof(ClientOpCodes), packet.OpCode).ToUpper()}] Slot: {slot} | TID: {targetId} | TPT: {targetPoint}", client);
            Game.UseSpell(client, slot, targetId, targetPoint);
        }
        private void JoinClient(Client client, ClientPacket packet)
        {
            byte seed = packet.ReadByte();
            byte[] key = packet.ReadData8();
            string name = packet.ReadString8();
            uint id = packet.ReadUInt32();

            Server.WriteLog($@"Recv [{Enum.GetName(typeof(ClientOpCodes), packet.OpCode).ToUpper()}] Seed: {seed} | Key: {Encoding.ASCII.GetString(key)} | Name: {name} | ID: {id}", client);
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

            Server.WriteLog($@"Recv [{Enum.GetName(typeof(ClientOpCodes), packet.OpCode).ToUpper()}] Direction: {direction}", client);
            Game.Turn(client, direction);
        }

        private void SpaceBar(Client client, ClientPacket packet)
        {
            Server.WriteLog($@"Recv [{Enum.GetName(typeof(ClientOpCodes), packet.OpCode).ToUpper()}] ", client);
            Game.SpaceBar(client);
        }
        private void RequestWorldList(Client client, ClientPacket packet)
        {
            Server.WriteLog($@"Recv [{Enum.GetName(typeof(ClientOpCodes), packet.OpCode).ToUpper()}] ", client);
            Game.RequestWorldList(client);
        }
        private void Whisper(Client client, ClientPacket packet)
        {
            string targetName = packet.ReadString8();
            string message = packet.ReadString8();

            Server.WriteLog($@"Recv [{Enum.GetName(typeof(ClientOpCodes), packet.OpCode).ToUpper()}] Target: {targetName} | Message: {message}", client);
            Game.Whisper(client, targetName, message);
        }
        private void ToggleUserOption(Client client, ClientPacket packet)
        {
            UserOption option = (UserOption)packet.ReadByte();

            Server.WriteLog($@"Recv [{Enum.GetName(typeof(ClientOpCodes), packet.OpCode).ToUpper()}] Option: {option}", client);
            Game.ToggleUserOption(client, option);
        }
        private void UseItem(Client client, ClientPacket packet)
        {
            byte slot = packet.ReadByte();

            Server.WriteLog($@"Recv [{Enum.GetName(typeof(ClientOpCodes), packet.OpCode).ToUpper()}] Slot: {slot}", client);
            Game.UseItem(client, slot);
        }
        private void Emote(Client client, ClientPacket packet)
        {
            byte animNum = packet.ReadByte();
            BodyAnimation anim = (BodyAnimation)(animNum + 9);

            if (animNum > 35) return;

            Server.WriteLog($@"Recv [{Enum.GetName(typeof(ClientOpCodes), packet.OpCode).ToUpper()}] Emote: {anim}", client);
            Game.AnimateCreature(client, anim);
        }
        private void DropGold(Client client, ClientPacket packet)
        {
            uint amount = packet.ReadUInt32();
            Point groundPoint = packet.ReadPoint();

            Server.WriteLog($@"Recv [{Enum.GetName(typeof(ClientOpCodes), packet.OpCode).ToUpper()}] Amount: {amount} | Point: {groundPoint}", client);
            Game.DropGold(client, amount, groundPoint);
        }
        private void ChangePassword(Client client, ClientPacket packet)
        {
            string name = packet.ReadString8();
            string currentPw = packet.ReadString8();
            string newPw = packet.ReadString8();

            Server.WriteLog($@"Recv [{Enum.GetName(typeof(ClientOpCodes), packet.OpCode).ToUpper()}] Name: {name} | Current: {currentPw} | New: {newPw}", client);
            Game.ChangePassword(client, name, currentPw, newPw);
        }
        private void DropItemOnCreature(Client client, ClientPacket packet)
        {
            byte slot = packet.ReadByte();
            int targetId = packet.ReadInt32();
            byte count = packet.ReadByte();

            Server.WriteLog($@"Recv [{Enum.GetName(typeof(ClientOpCodes), packet.OpCode).ToUpper()}] Slot: {slot} | Target: {targetId} | Count: {count}", client);
            Game.DropItemOnCreature(client, slot, targetId, count);
        }
        private void DropGoldOnCreature(Client client, ClientPacket packet)
        {
            uint amount = packet.ReadUInt32();
            int targetId = packet.ReadInt32();

            Server.WriteLog($@"Recv [{Enum.GetName(typeof(ClientOpCodes), packet.OpCode).ToUpper()}] Amount: {amount} | Target: {targetId}", client);
            Game.DropGoldOnCreature(client, amount, targetId);
        }
        private void RequestProfile(Client client, ClientPacket packet)
        {
            if (client.ServerType == ServerType.World)
            {
                Server.WriteLog($@"Recv [{Enum.GetName(typeof(ClientOpCodes), packet.OpCode).ToUpper()}] ", client);
                Game.RequestProfile(client);
            }
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

            Server.WriteLog($@"Recv [{Enum.GetName(typeof(ClientOpCodes), packet.OpCode).ToUpper()}] Type: {type} | Target: {targetName}", client);
            Game.RequestGroup(client, type, targetName, box);
        }
        private void ToggleGroup(Client client, ClientPacket packet)
        {
            Server.WriteLog($@"Recv [{Enum.GetName(typeof(ClientOpCodes), packet.OpCode).ToUpper()}] ", client);
            Game.ToggleGroup(client);
        }
        private void SwapSlot(Client client, ClientPacket packet)
        {
            Pane pane = (Pane)packet.ReadByte();
            byte origSlot = packet.ReadByte();
            byte endSlot = packet.ReadByte();

            Server.WriteLog($@"Recv [{Enum.GetName(typeof(ClientOpCodes), packet.OpCode).ToUpper()}] Pane: {pane} | From: {origSlot} | To: {endSlot}", client);
            Game.SwapSlot(client, pane, origSlot, endSlot);
        }
        private void RequestRefresh(Client client, ClientPacket packet)
        {
            Server.WriteLog($@"Recv [{Enum.GetName(typeof(ClientOpCodes), packet.OpCode).ToUpper()}] ", client);
            Game.RequestRefresh(client);
        }
        private void RequestPursuit(Client client, ClientPacket packet)
        {
            GameObjectType objType = (GameObjectType)packet.ReadByte();
            int objId = packet.ReadInt32();
            ushort pid = packet.ReadUInt16();
            PursuitIds pursuitId = Enum.IsDefined(typeof(PursuitIds), pid) ? (PursuitIds)pid : PursuitIds.None;
            byte[] args = packet.ReadBytes(packet.Data.Length - packet.Position);

            Server.WriteLog($@"Recv [{Enum.GetName(typeof(ClientOpCodes), packet.OpCode).ToUpper()}] ObjType: {objType} | ObjId: {objId} | Pursuit: {pursuitId} | Args: {args.Count() > 0}", client);
            Game.RequestPursuit(client, objType, objId, pursuitId, args);
        }
        private void ReplyDialog(Client client, ClientPacket packet)
        {
            GameObjectType objType = (GameObjectType)packet.ReadByte();
            int objId = packet.ReadInt32();
            ushort pid = packet.ReadUInt16();
            PursuitIds pursuitId = Enum.IsDefined(typeof(PursuitIds), pid) ? (PursuitIds)pid : PursuitIds.None;
            ushort dialogId = packet.ReadUInt16();

            DialogArgsType argsType = DialogArgsType.None;
            byte opt = 0;
            string input = "";

            if(packet.Position != packet.Data.Length)
            {
                argsType = (DialogArgsType)packet.ReadByte();

                switch (argsType)
                {
                    case DialogArgsType.MenuResponse:
                        opt = packet.ReadByte();
                        break;
                    case DialogArgsType.TextResponse:
                        input = packet.ReadString8();
                        break;
                }
            }

            Server.WriteLog($@"Recv [{Enum.GetName(typeof(ClientOpCodes), packet.OpCode).ToUpper()}] ObjType: {objType} | ObjId: {objId} | Pursuit: {pursuitId} | DialogId: {dialogId} | Args: { Enum.GetName(typeof(DialogArgsType), argsType) }", client);
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

            Server.WriteLog($@"Recv [{Enum.GetName(typeof(ClientOpCodes), packet.OpCode).ToUpper()}] ", client);
            Game.Boards(client);
        }
        private void UseSkill(Client client, ClientPacket packet)
        {
            byte slot = packet.ReadByte();

            Server.WriteLog($@"Recv [{Enum.GetName(typeof(ClientOpCodes), packet.OpCode).ToUpper()}] Slot: {slot}", client);
            Game.UseSkill(client, slot);
        }

        private void ClickWorldMap(Client client, ClientPacket packet)
        {
            uint mapId = packet.ReadUInt32();
            Point point = packet.ReadPoint();

            Server.WriteLog($@"Recv [{Enum.GetName(typeof(ClientOpCodes), packet.OpCode).ToUpper()}] MapId: {mapId} | Point: {point}", client);
            Game.ClickWorldMap(client, (ushort)mapId, point);
        }
        private void ClickObject(Client client, ClientPacket packet)
        {
            byte type = packet.ReadByte();
            switch (type)
            {
                case 1:
                    int objectId = packet.ReadInt32();
                    Server.WriteLog($@"Recv [{Enum.GetName(typeof(ClientOpCodes), packet.OpCode).ToUpper()}] Type: {type} | ObjId: {objectId}", client);
                    Game.ClickObject(client, objectId);
                    break;
                case 3:
                    Point clickPoint = packet.ReadPoint();
                    Server.WriteLog($@"Recv [{Enum.GetName(typeof(ClientOpCodes), packet.OpCode).ToUpper()}] Type: {type} | Point: {clickPoint}", client);
                    Game.ClickObject(client, clickPoint);
                    break;
            }

        }
        private void RemoveEquipment(Client client, ClientPacket packet)
        {
            EquipmentSlot slot = (EquipmentSlot)packet.ReadByte();

            Server.WriteLog($@"Recv [{Enum.GetName(typeof(ClientOpCodes), packet.OpCode).ToUpper()}] Slot: {slot}", client);
            Game.RemoveEquipment(client, slot);
        }
        private void KeepAlive(Client client, ClientPacket packet)
        {
            byte b = packet.ReadByte();
            byte a = packet.ReadByte();

            Server.WriteLog($@"Recv [{Enum.GetName(typeof(ClientOpCodes), packet.OpCode).ToUpper()}] ", client);
            Game.KeepAlive(client, a, b);
        }
        private void ChangeStat(Client client, ClientPacket packet)
        {
            Stat stat = (Stat)packet.ReadByte();

            Server.WriteLog($@"Recv [{Enum.GetName(typeof(ClientOpCodes), packet.OpCode).ToUpper()}] Stat: {stat}", client);
            Game.ChangeStat(client, stat);
        }
        private void Exchange(Client client, ClientPacket packet)
        {
            ExchangeType type = (ExchangeType)packet.ReadByte();
            int targetId = packet.ReadInt32();

            switch (type)
            {
                case ExchangeType.StartExchange:
                    {
                        Server.WriteLog($@"Recv [{Enum.GetName(typeof(ClientOpCodes), packet.OpCode).ToUpper()}] Type: {type} | TID: {targetId}", client);
                        Game.Exchange(client, type, targetId);
                        break;
                    }
                case ExchangeType.RequestAmount:
                    {
                        byte slot = packet.ReadByte();

                        Server.WriteLog($@"Recv [{Enum.GetName(typeof(ClientOpCodes), packet.OpCode).ToUpper()}] Type: {type} | TID: {targetId} | Slot: {slot}", client);
                        Game.Exchange(client, type, targetId, 0, slot, 0);
                        break;
                    }
                case ExchangeType.AddItem:
                    {
                        byte slot = packet.ReadByte();
                        byte count = packet.ReadByte();

                        Server.WriteLog($@"Recv [{Enum.GetName(typeof(ClientOpCodes), packet.OpCode).ToUpper()}] Type: {type} | TID: {targetId} | Slot: {slot} | Count: {count}", client);
                        Game.Exchange(client, type, targetId, 0, slot, count);
                        break;
                    }
                case ExchangeType.SetGold:
                    {
                        uint amount = packet.ReadUInt32();

                        Server.WriteLog($@"Recv [{Enum.GetName(typeof(ClientOpCodes), packet.OpCode).ToUpper()}] Type: {type} | TID: {targetId} | Amount: {amount}", client);
                        Game.Exchange(client, type, targetId, amount);
                        break;
                    }
                case ExchangeType.Cancel:
                case ExchangeType.Accept:
                    Server.WriteLog($@"Recv [{Enum.GetName(typeof(ClientOpCodes), packet.OpCode).ToUpper()}] Type: {type}", client);
                    Game.Exchange(client, type);
                    break;
            }
        }
        private void RequestLoginMessage(Client client, ClientPacket packet)
        {
            Server.WriteLog($@"Recv [{Enum.GetName(typeof(ClientOpCodes), packet.OpCode).ToUpper()}] ", client);
            Game.RequestLoginMessage(packet.Position == packet.Data.Length, client);
        }

        private void BeginChant(Client client, ClientPacket packet)
        {
            Server.WriteLog($@"Recv [{Enum.GetName(typeof(ClientOpCodes), packet.OpCode).ToUpper()}] ", client);
            Game.BeginChant(client);
        }
        private void DisplayChant(Client client, ClientPacket packet)
        {
            string chant = packet.ReadString8();

            Server.WriteLog($@"Recv [{Enum.GetName(typeof(ClientOpCodes), packet.OpCode).ToUpper()}] Chant: {chant}", client);
            Game.DisplayChant(client, chant);
        }
        private void Personal(Client client, ClientPacket packet)
        {
            ushort totalLength = packet.ReadUInt16();
            ushort portraitLength = packet.ReadUInt16();
            byte[] portraitData = packet.ReadBytes(portraitLength);
            string profileMsg = packet.ReadString16();

            Server.WriteLog($@"Recv [{Enum.GetName(typeof(ClientOpCodes), packet.OpCode).ToUpper()}] ", client);
            Game.Personal(client, portraitData, profileMsg);
        }
        private void RequestServerTable(Client client, ClientPacket packet)
        {
            bool requestTable = packet.ReadBoolean();
            Server.WriteLog($@"Recv [{Enum.GetName(typeof(ClientOpCodes), packet.OpCode).ToUpper()}] Table: {requestTable}", client);
            Game.RequestServerTable(client, requestTable);
        }
        private void RequestHomepage(Client client, ClientPacket packet)
        {
            Server.WriteLog($@"Recv [{Enum.GetName(typeof(ClientOpCodes), packet.OpCode).ToUpper()}] ", client);
            Game.RequestHomepage(client);
        }
        private void SynchronizeTicks(Client client, ClientPacket packet)
        {
            TimeSpan serverTicks = new TimeSpan(packet.ReadUInt32());
            TimeSpan clientTicks = new TimeSpan(packet.ReadUInt32());

            Game.SynchronizeTicks(client, serverTicks, clientTicks);
        }
        private void ChangeSocialStatus(Client client, ClientPacket packet)
        {
            SocialStatus status = (SocialStatus)packet.ReadByte();

            Server.WriteLog($@"Recv [{Enum.GetName(typeof(ClientOpCodes), packet.OpCode).ToUpper()}] Status: {status}", client);
            Game.ChangeSoocialStatus(client, status);
        }
        private void RequestMetaFile(Client client, ClientPacket packet)
        {
            bool all = packet.ReadBoolean();

            Server.WriteLog($@"Recv [{Enum.GetName(typeof(ClientOpCodes), packet.OpCode).ToUpper()}] All: {all}", client);
            Game.RequestMetaFile(client, all);
        }
    }
}
