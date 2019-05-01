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
using System.Collections.Immutable;

namespace Chaos
{
    internal delegate void Handler(Client client, ClientPacket packet);
    /// <summary>
    /// A container for methods that read ClientPackets, based on an opcode/delegate system.
    /// </summary>
    internal sealed class ClientPackets
    {

        internal static ImmutableArray<Handler> Handlers
        {
            get
            {
                var cp = new ClientPackets();

                var handles = new Handler[byte.MaxValue];
                handles[(byte)ClientOpCodes.RequestConnectionInfo] = new Handler(cp.RequestConnectionInfo);
                handles[(byte)ClientOpCodes.CreateChar1] = new Handler(cp.CreateChar1);
                handles[(byte)ClientOpCodes.Login] = new Handler(cp.Login);
                handles[(byte)ClientOpCodes.CreateChar2] = new Handler(cp.CreateChar2);
                handles[(byte)ClientOpCodes.RequestMapData] = new Handler(cp.RequestMapData);
                handles[(byte)ClientOpCodes.ClientWalk] = new Handler(cp.ClientWalk);
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
                handles[(byte)ClientOpCodes.RequestLoginNotification] = new Handler(cp.RequestLoginNotification);
                handles[(byte)ClientOpCodes.BeginChant] = new Handler(cp.BeginChant);
                handles[(byte)ClientOpCodes.DisplayChant] = new Handler(cp.DisplayChant);
                handles[(byte)ClientOpCodes.Personal] = new Handler(cp.Personal);
                handles[(byte)ClientOpCodes.RequestServerTable] = new Handler(cp.RequestServerTable);
                handles[(byte)ClientOpCodes.ChangeSequence] = new Handler(cp.ChangeSequence);
                handles[(byte)ClientOpCodes.RequestHomepage] = new Handler(cp.RequestHomepage);
                handles[(byte)ClientOpCodes.SynchronizeTicks] = new Handler(cp.SynchronizeTicks);
                handles[(byte)ClientOpCodes.ChangeSocialStatus] = new Handler(cp.ChangeSocialStatus);
                handles[(byte)ClientOpCodes.RequestMetaFile] = new Handler(cp.RequestMetaFile);

                return handles.ToImmutableArray();
            }
        }

        internal ClientPackets() { }

        private void RequestConnectionInfo(Client client, ClientPacket packet)
        {
            Server.WriteLogAsync($@"Recv [{(ClientOpCodes)packet.OpCode}]", client);
            Game.RequestConnectionInfo(client);
        }

        private void CreateChar1(Client client, ClientPacket packet)
        {
            string name = packet.ReadString8();
            string pw = packet.ReadString8();

            Server.WriteLogAsync($@"Recv [{(ClientOpCodes)packet.OpCode}] NAME: {name} | PASSWORD: {pw}", client);
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

            Server.WriteLogAsync($@"Recv [{(ClientOpCodes)packet.OpCode}] NAME: {name} | PASSWORD: {pw}", client);
            Game.Login(client, name, pw);
        }

        private void CreateChar2(Client client, ClientPacket packet)
        {
            byte hairStyle = packet.ReadByte();
            var gender = (Gender)packet.ReadByte();
            byte hairColor = packet.ReadByte();

            Server.WriteLogAsync($@"Recv [{(ClientOpCodes)packet.OpCode}] HAIR: {hairStyle} | GENDER: {gender} | COLOR: {hairColor}", client);
            Game.CreateChar2(client, hairStyle, gender, hairColor);
        }

        private void RequestMapData(Client client, ClientPacket packet)
        {
            Server.WriteLogAsync($@"Recv [{(ClientOpCodes)packet.OpCode}] ", client);
            Game.RequestMapData(client);
        }
        private void ClientWalk(Client client, ClientPacket packet)
        {
            var direction = (Direction)packet.ReadByte();
            int stepCount = packet.ReadByte();

            Server.WriteLogAsync($@"Recv [{(ClientOpCodes)packet.OpCode}] DIRECTION: {direction} | STEPS: {stepCount}", client);
            Game.ClientWalk(client, direction, stepCount);
        }
        private void Pickup(Client client, ClientPacket packet)
        {
            byte slot = packet.ReadByte();
            Point groundPoint = packet.ReadPoint();

            Server.WriteLogAsync($@"Recv [{(ClientOpCodes)packet.OpCode}] DESTINATION_SLOT: {slot} | SOURCE_POINT: {groundPoint}", client);
            Game.Pickup(client, slot, groundPoint);
        }
        private void Drop(Client client, ClientPacket packet)
        {
            byte slot = packet.ReadByte();
            Point groundPoint = packet.ReadPoint();
            uint count = packet.ReadUInt32();

            Server.WriteLogAsync($@"Recv [{(ClientOpCodes)packet.OpCode}] SOURCE_SLOT: {slot} | DESTINATION_POINT: {groundPoint} | COUNT: {count}", client);
            Game.Drop(client, slot, groundPoint, count);
        }
        private void ExitClient(Client client, ClientPacket packet)
        {
            bool requestExit = false;

            if (packet.Position != packet.Length)
                requestExit = packet.ReadBoolean();

            Server.WriteLogAsync($@"Recv [{(ClientOpCodes)packet.OpCode}] EXIT: {requestExit}", client);
            Game.ExitClient(client, requestExit);
        }
        private void Ignore(Client client, ClientPacket packet)
        {
            var type = (IgnoreType)packet.ReadByte();
            string targetName = null;

            if (type != IgnoreType.Request)
                targetName = packet.ReadString8();

            Server.WriteLogAsync($@"Recv [{(ClientOpCodes)packet.OpCode}] TYPE: {type} | TARGET: {targetName ?? "n/a"}", client);
            Game.Ignore(client, type, targetName);
        }
        private void PublicChat(Client client, ClientPacket packet)
        {
            var type = (PublicMessageType)packet.ReadByte();
            string message = packet.ReadString8();

            Server.WriteLogAsync($@"Recv [{(ClientOpCodes)packet.OpCode}] TYPE: {type} | MESSAGE: {message}", client);
            Game.PublicChat(client, type, message);
        }
        private void UseSpell(Client client, ClientPacket packet)
        {
            byte slot = packet.ReadByte();
            int targetId = client.User.ID;
            Point targetPoint = client.User.Point;
            string prompt = "";

            if(client.User.SpellBook[slot]?.SpellType == SpellType.Prompt)
                prompt = packet.ReadString();
            else if (packet.Position != packet.Length)
            {
                targetId = packet.ReadInt32();
                targetPoint = packet.ReadPoint();
            }

            Server.WriteLogAsync($@"Recv [{(ClientOpCodes)packet.OpCode}] SOURCE_SLOT: {slot} | TARGET_ID: {targetId} | TARGET_POINT: {targetPoint}", client);
            Game.UseSpell(client, slot, targetId, targetPoint, prompt);
        }
        private void JoinClient(Client client, ClientPacket packet)
        {
            byte seed = packet.ReadByte();
            string key = packet.ReadString8();
            string name = packet.ReadString8();
            uint id = packet.ReadUInt32();

            Server.WriteLogAsync($@"Recv [{(ClientOpCodes)packet.OpCode}] SEED: {seed} | KEY: {key} | NAME: {name} | ID: {id}", client);
            Redirect redirect = client.Server.Redirects.FirstOrDefault(r => r.Id == id);

            if (redirect != null)
            {
                client.ServerType = redirect.Type;
                client.Server.Redirects.Remove(redirect);
                Game.JoinClient(client, seed, Encoding.ASCII.GetBytes(key), name, id);
            }
            else
                client.Disconnect();
        }
        private void Turn(Client client, ClientPacket packet)
        {
            var direction = (Direction)packet.ReadByte();

            Server.WriteLogAsync($@"Recv [{(ClientOpCodes)packet.OpCode}] DIRECTION: {direction}", client);
            Game.Turn(client, direction);
        }

        private void SpaceBar(Client client, ClientPacket packet)
        {
            Server.WriteLogAsync($@"Recv [{(ClientOpCodes)packet.OpCode}] ", client);
            Game.SpaceBar(client);
        }
        private void RequestWorldList(Client client, ClientPacket packet)
        {
            Server.WriteLogAsync($@"Recv [{(ClientOpCodes)packet.OpCode}] ", client);
            Game.RequestWorldList(client);
        }
        private void Whisper(Client client, ClientPacket packet)
        {
            string targetName = packet.ReadString8();
            string message = packet.ReadString8();

            Server.WriteLogAsync($@"Recv [{(ClientOpCodes)packet.OpCode}] TARGET: {targetName} | MESSAGE: {message}", client);
            Game.Whisper(client, targetName, message);
        }
        private void ToggleUserOption(Client client, ClientPacket packet)
        {
            var option = (UserOption)packet.ReadByte();

            Server.WriteLogAsync($@"Recv [{(ClientOpCodes)packet.OpCode}] OPTION: {option}", client);
            Game.ToggleUserOption(client, option);
        }
        private void UseItem(Client client, ClientPacket packet)
        {
            byte slot = packet.ReadByte();

            Server.WriteLogAsync($@"Recv [{(ClientOpCodes)packet.OpCode}] SOURCE_SLOT: {slot}", client);
            Game.UseItem(client, slot);
        }
        private void Emote(Client client, ClientPacket packet)
        {
            byte animNum = packet.ReadByte();
            var anim = (BodyAnimation)(animNum + 9);

            if (animNum > 35) return;

            Server.WriteLogAsync($@"Recv [{(ClientOpCodes)packet.OpCode}] EMOTE: {anim}", client);
            Game.AnimateCreature(client, anim);
        }
        private void DropGold(Client client, ClientPacket packet)
        {
            uint amount = packet.ReadUInt32();
            Point groundPoint = packet.ReadPoint();

            Server.WriteLogAsync($@"Recv [{(ClientOpCodes)packet.OpCode}] AMOUNT: {amount} | DESTINATION_POINT: {groundPoint}", client);
            Game.DropGold(client, amount, groundPoint);
        }
        private void ChangePassword(Client client, ClientPacket packet)
        {
            string name = packet.ReadString8();
            string currentPw = packet.ReadString8();
            string newPw = packet.ReadString8();

            Server.WriteLogAsync($@"Recv [{(ClientOpCodes)packet.OpCode}] NAME: {name} | CURRENT: {currentPw} | NEW: {newPw}", client);
            Game.ChangePassword(client, name, currentPw, newPw);
        }
        private void DropItemOnCreature(Client client, ClientPacket packet)
        {
            byte slot = packet.ReadByte();
            int targetId = packet.ReadInt32();
            byte count = packet.ReadByte();

            Server.WriteLogAsync($@"Recv [{(ClientOpCodes)packet.OpCode}] SOURCE_SLOT: {slot} | TARGET_ID: {targetId} | COUNT: {count}", client);
            Game.DropItemOnCreature(client, slot, targetId, count);
        }
        private void DropGoldOnCreature(Client client, ClientPacket packet)
        {
            uint amount = packet.ReadUInt32();
            int targetId = packet.ReadInt32();

            Server.WriteLogAsync($@"Recv [{(ClientOpCodes)packet.OpCode}] AMOUNT: {amount} | TARGET_ID: {targetId}", client);
            Game.DropGoldOnCreature(client, amount, targetId);
        }
        private void RequestProfile(Client client, ClientPacket packet)
        {
            Server.WriteLogAsync($@"Recv [{(ClientOpCodes)packet.OpCode}] ", client);
            Game.RequestProfile(client);
        }
        private void RequestGroup(Client client, ClientPacket packet)
        {
            GroupBox box = default;
            var type = (GroupRequestType)packet.ReadByte();

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

                box = new GroupBox(text, maxLevel, maxOfEach);
            }
            string targetName = packet.ReadString8();

            Server.WriteLogAsync($@"Recv [{(ClientOpCodes)packet.OpCode}] TYPE: {type} | TARGET: {targetName}", client);
            Game.RequestGroup(client, type, targetName, box);
        }
        private void ToggleGroup(Client client, ClientPacket packet)
        {
            Server.WriteLogAsync($@"Recv [{(ClientOpCodes)packet.OpCode}] ", client);
            Game.ToggleGroup(client);
        }
        private void SwapSlot(Client client, ClientPacket packet)
        {
            var panelType = (PanelType)packet.ReadByte();
            byte origSlot = packet.ReadByte();
            byte endSlot = packet.ReadByte();

            Server.WriteLogAsync($@"Recv [{(ClientOpCodes)packet.OpCode}] PANEL: {panelType} | FROM: {origSlot} | TO: {endSlot}", client);
            Game.SwapSlot(client, panelType, origSlot, endSlot);
        }
        private void RequestRefresh(Client client, ClientPacket packet)
        {
            Server.WriteLogAsync($@"Recv [{(ClientOpCodes)packet.OpCode}] ", client);
            Game.RequestRefresh(client);
        }
        private void RequestPursuit(Client client, ClientPacket packet)
        {
            var objType = (GameObjectType)packet.ReadByte();
            int objId = packet.ReadInt32();
            ushort pid = packet.ReadUInt16();
            PursuitIds pursuitId = Enum.IsDefined(CONSTANTS.PURSUITIDS_TYPE, pid) ? (PursuitIds)pid 
                : PursuitIds.None;
            byte[] args = packet.ReadBytes(packet.Length - packet.Position);

            Server.WriteLogAsync($@"Recv [{(ClientOpCodes)packet.OpCode}] OBJECT: {objType} | OBJECT_ID: {objId} | PURSUIT: {pursuitId} | ARGS: {args.Length > 0}", client);
            Game.RequestPursuit(client, objType, objId, pursuitId, args);
        }
        private void ReplyDialog(Client client, ClientPacket packet)
        {
            var objType = (GameObjectType)packet.ReadByte();
            int objId = packet.ReadInt32();
            ushort pid = packet.ReadUInt16();
            PursuitIds pursuitId = Enum.IsDefined(CONSTANTS.PURSUITIDS_TYPE, pid) ? (PursuitIds)pid 
                : PursuitIds.None;
            ushort dialogId = packet.ReadUInt16();

            DialogArgsType argsType = DialogArgsType.None;
            byte opt = 0;
            string input = "";

            if(packet.Position != packet.Length)
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

            Server.WriteLogAsync($@"Recv [{(ClientOpCodes)packet.OpCode}] OBJECT: {objType} | OBJECT_ID: {objId} | PURSUIT: {pursuitId} | DIALOG_ID: {dialogId} | ARGS: {argsType}", client);
            Game.ReplyDialog(client, objType, objId, pursuitId, dialogId, argsType, opt, input);
        }

        //this packet is literally retarded
        private void Board(Client client, ClientPacket packet)
        {
            var type = (BoardRequestType)packet.ReadByte();

            switch (type) //request type
            {
                case BoardRequestType.BoardList:
                    //Board List
                    //client.Enqueue(client.ServerPackets.BulletinBoard);
                    break;
                case BoardRequestType.ViewBoard:
                    {
                        //Post list for boardNum
                        ushort boardNum = packet.ReadUInt16();
                        ushort startPostNum = packet.ReadUInt16(); //you send the newest mail first, which will have the highest number. startPostNum counts down.
                        //packet.ReadByte() is always 0xF0(240) ???
                        //the client spam requests this like holy fuck, put a timer on this so you only send 1 packet
                        break;
                    }
                case BoardRequestType.ViewPost:
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
                case BoardRequestType.NewPost: //new post
                    {
                        ushort boardNum = packet.ReadUInt16();
                        string subject = packet.ReadString8();
                        string message = packet.ReadString16();
                        break;
                    }
                case BoardRequestType.Delete: //delete post
                    {
                        ushort boardNum = packet.ReadUInt16();
                        ushort postId = packet.ReadUInt16(); //the post number they want to delete, counting up
                        break;
                    }

                case BoardRequestType.SendMail: //send mail
                    {
                        ushort boardNum = packet.ReadUInt16();
                        string targetName = packet.ReadString8();
                        string subject = packet.ReadString8();
                        string message = packet.ReadString16();
                        break;
                    }
                case BoardRequestType.Highlight: //highlight message
                    {
                        ushort boardNum = packet.ReadUInt16();
                        ushort postId = packet.ReadUInt16();
                        break;
                    }
            }

            Server.WriteLogAsync($@"Recv [{(ClientOpCodes)packet.OpCode}] TYPE: {type}", client);
            Game.Boards(client);
        }
        private void UseSkill(Client client, ClientPacket packet)
        {
            byte slot = packet.ReadByte();

            Server.WriteLogAsync($@"Recv [{(ClientOpCodes)packet.OpCode}] SOURCE_SLOT: {slot}", client);
            Game.UseSkill(client, slot);
        }

        private void ClickWorldMap(Client client, ClientPacket packet)
        {
            ushort nodeCheckSum = packet.ReadUInt16();
            Location targetLocation = (packet.ReadUInt16(), packet.ReadPoint());

            Server.WriteLogAsync($@"Recv [{(ClientOpCodes)packet.OpCode}] CHKSUM: {nodeCheckSum} TARGET_LOCATION: {targetLocation}", client);
            Game.ClickWorldMap(client, nodeCheckSum, targetLocation);
        }
        private void ClickObject(Client client, ClientPacket packet)
        {
            byte type = packet.ReadByte();
            switch (type)
            {
                case 1:
                    int objectId = packet.ReadInt32();
                    Server.WriteLogAsync($@"Recv [{(ClientOpCodes)packet.OpCode}] TYPE: {type} | OBJECT_ID: {objectId}", client);
                    Game.ClickObject(client, objectId);
                    break;
                case 3:
                    Point clickPoint = packet.ReadPoint();
                    Server.WriteLogAsync($@"Recv [{(ClientOpCodes)packet.OpCode}] TYPE: {type} | CLICK_POINT: {clickPoint}", client);
                    Game.ClickObject(client, clickPoint);
                    break;
            }
        }
        private void RemoveEquipment(Client client, ClientPacket packet)
        {
            var slot = (EquipmentSlot)packet.ReadByte();

            Server.WriteLogAsync($@"Recv [{(ClientOpCodes)packet.OpCode}] SLOT: {slot}", client);
            Game.RemoveEquipment(client, slot);
        }
        private void KeepAlive(Client client, ClientPacket packet)
        {
            byte b = packet.ReadByte();
            byte a = packet.ReadByte();

            Server.WriteLogAsync($@"Recv [{(ClientOpCodes)packet.OpCode}] ", client);
            Game.KeepAlive(client, a, b);
        }
        private void ChangeStat(Client client, ClientPacket packet)
        {
            var stat = (Stat)packet.ReadByte();

            Server.WriteLogAsync($@"Recv [{(ClientOpCodes)packet.OpCode}] STAT: {stat}", client);
            Game.ChangeStat(client, stat);
        }
        private void Exchange(Client client, ClientPacket packet)
        {
            var type = (ExchangeType)packet.ReadByte();
            int targetId = packet.ReadInt32();

            switch (type)
            {
                case ExchangeType.StartExchange:
                    {
                        Server.WriteLogAsync($@"Recv [{(ClientOpCodes)packet.OpCode}] TYPE: {type} | TARGET_ID: {targetId}", client);
                        Game.Exchange(client, type, targetId);
                        break;
                    }
                case ExchangeType.RequestAmount:
                    {
                        byte slot = packet.ReadByte();

                        Server.WriteLogAsync($@"Recv [{(ClientOpCodes)packet.OpCode}] TYPE: {type} | TARGET_ID: {targetId} | SOURCE_SLOT: {slot}", client);
                        Game.Exchange(client, type, targetId, 0, slot, 0);
                        break;
                    }
                case ExchangeType.AddItem:
                    {
                        byte slot = packet.ReadByte();
                        byte count = packet.ReadByte();

                        Server.WriteLogAsync($@"Recv [{(ClientOpCodes)packet.OpCode}] TYPE: {type} | TARGET_ID: {targetId} | SOURCE_SLOT: {slot} | COUNT: {count}", client);
                        Game.Exchange(client, type, targetId, 0, slot, count);
                        break;
                    }
                case ExchangeType.SetGold:
                    {
                        uint amount = packet.ReadUInt32();

                        Server.WriteLogAsync($@"Recv [{(ClientOpCodes)packet.OpCode}] TYPE: {type} | TARGET_ID: {targetId} | AMOUNT: {amount}", client);
                        Game.Exchange(client, type, targetId, amount);
                        break;
                    }
                case ExchangeType.Cancel:
                case ExchangeType.Accept:
                    Server.WriteLogAsync($@"Recv [{(ClientOpCodes)packet.OpCode}] TYPE: {type}", client);
                    Game.Exchange(client, type);
                    break;
            }
        }
        private void RequestLoginNotification(Client client, ClientPacket packet)
        {
            Server.WriteLogAsync($@"Recv [{(ClientOpCodes)packet.OpCode}] ", client);
            Game.RequestLoginNotification(true, client);
        }

        private void BeginChant(Client client, ClientPacket packet)
        {
            Server.WriteLogAsync($@"Recv [{(ClientOpCodes)packet.OpCode}] ", client);
            Game.BeginChant(client);
        }
        private void DisplayChant(Client client, ClientPacket packet)
        {
            string chant = packet.ReadString8();

            Server.WriteLogAsync($@"Recv [{(ClientOpCodes)packet.OpCode}] CHANT: {chant}", client);
            Game.DisplayChant(client, chant);
        }
        private void Personal(Client client, ClientPacket packet)
        {
            ushort totalLength = packet.ReadUInt16();
            ushort portraitLength = packet.ReadUInt16();
            byte[] portraitData = packet.ReadBytes(portraitLength);
            string profileMsg = packet.ReadString16();

            Server.WriteLogAsync($@"Recv [{(ClientOpCodes)packet.OpCode}] DATA_SIZE: {portraitLength} | MESSAGE_LENGTH: {profileMsg.Length}", client);
            Game.Personal(client, portraitData, profileMsg);
        }
        private void RequestServerTable(Client client, ClientPacket packet)
        {
            bool requestTable = packet.ReadBoolean();
            Server.WriteLogAsync($@"Recv [{(ClientOpCodes)packet.OpCode}] REQUEST: {requestTable}", client);
            Game.RequestServerTable(client, requestTable);
        }
        private void ChangeSequence(Client client, ClientPacket packet) => client.ReceiveSequence = packet.Sequence;
        private void RequestHomepage(Client client, ClientPacket packet)
        {
            Server.WriteLogAsync($@"Recv [{(ClientOpCodes)packet.OpCode}] ", client);
            Game.RequestHomepage(client);
        }
        private void SynchronizeTicks(Client client, ClientPacket packet)
        {
            var serverTicks = new TimeSpan(packet.ReadUInt32());
            var clientTicks = new TimeSpan(packet.ReadUInt32());

            Server.WriteLogAsync($@"Recv [{(ClientOpCodes)packet.OpCode}] SERVER_TICKS: {serverTicks} | CLIENT_TICKS: {clientTicks}", client);
            Game.SynchronizeTicks(client, serverTicks, clientTicks);
        }
        private void ChangeSocialStatus(Client client, ClientPacket packet)
        {
            var status = (SocialStatus)packet.ReadByte();

            Server.WriteLogAsync($@"Recv [{(ClientOpCodes)packet.OpCode}] DESIRED_STATUS: {status}", client);
            Game.ChangeSocialStatus(client, status);
        }
        private void RequestMetaFile(Client client, ClientPacket packet)
        {
            bool requestFile = packet.ReadBoolean();

            Server.WriteLogAsync($@"Recv [{(ClientOpCodes)packet.OpCode}] REQUEST_FILE: {requestFile}", client);
            Game.RequestMetaFile(client, requestFile);
        }
    }
}
