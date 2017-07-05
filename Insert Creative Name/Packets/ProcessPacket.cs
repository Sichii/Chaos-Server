using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Chaos.Objects;

namespace Chaos
{
    internal static class ProcessPacket
    {
        internal static Server Server = null;
        internal static World World = null;

        internal static void JoinServer(Client client)
        {
            uint TableCRC = CRC32.Calculate(Server.Table);

            client.Enqueue(ServerPackets.ConnectionInfo(TableCRC, client.Crypto.Seed, client.Crypto.Key));
        }

        internal static void CreateCharA(Client client, string name, string password)
        {
            if (Regex.Match(name, @"^[A-Za-z]{4, 12}$").Success)
            {
                using (BinaryReader reader = new BinaryReader(Crypto.DecryptFile(Paths.UserHash)))
                    while (reader.BaseStream.Position != reader.BaseStream.Length)
                    {
                        if (reader.ReadString().Equals(name, StringComparison.CurrentCultureIgnoreCase))
                        {
                            client.Enqueue(ServerPackets.LobbyMessage(3, "Name is taken."));
                            return;
                        }
                        //skips the hash
                        reader.BaseStream.Position += 33;
                    }

                client.NewCharName = name;
                client.NewCharPw = password;
                client.Enqueue(ServerPackets.LobbyMessage(0, "\0"));
            }
            else
            {
                client.Enqueue(ServerPackets.LobbyMessage(3,
                    "Name must be 4-12 letter long \n" +
                    "Password must be 4-8 characters long"));
            }
        }

        internal static void Login(Client client, string name, string password)
        {
            User user = World.Objects.Values.OfType<User>().FirstOrDefault(obj => obj.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase));
            if (user != null)
            {
                client.Enqueue(ServerPackets.LobbyMessage(3, "Character is already logged on."));
                user.Client.Disconnect();
                return;
            }

            password = Crypto.GetHashString(password, "MD5");
        }

        internal static void CreateCharB(Client client, byte hairStyle, byte gender, byte hairColor)
        {
            throw new NotImplementedException();
        }

        internal static void RequestMapData(Client client)
        {
            throw new NotImplementedException();
        }

        internal static void Walk(Client client, Direction direction, int stepCount)
        {
            throw new NotImplementedException();
        }

        internal static void Pickup(Client client, byte inventorySlot, Point groundPoint)
        {
            throw new NotImplementedException();
        }

        internal static void Drop(Client client, byte inventorySlot, Point groundPoint, int count)
        {
            throw new NotImplementedException();
        }

        internal static void ClientExit(Client client, bool requestExit)
        {
            throw new NotImplementedException();
        }

        internal static void PublicChat(Client client, ClientMessageType type, string message)
        {
            throw new NotImplementedException();
        }

        internal static void UseSpell(Client client, byte slot, uint targetId, Point targetPoint)
        {
            throw new NotImplementedException();
        }

        internal static void ClientJoin(Client client, byte seed, string key, string name, uint id)
        {
            throw new NotImplementedException();
        }

        internal static void Turn(Client client, Direction direction)
        {
            throw new NotImplementedException();
        }

        internal static void Spacebar(Client client)
        {
            throw new NotImplementedException();
        }

        internal static void RequestWorldList(Client client)
        {
            throw new NotImplementedException();
        }

        internal static void Whisper(Client client, string targetName, string message)
        {
            throw new NotImplementedException();
        }

        internal static void UserOptions(Client client, UserOption option)
        {
            throw new NotImplementedException();
        }

        internal static void UseItem(Client client, byte slot)
        {
            throw new NotImplementedException();
        }

        internal static void Emote(Client client, byte index)
        {
            throw new NotImplementedException();
        }

        internal static void DropGold(Client client, uint amount, Point groundPoint)
        {
            throw new NotImplementedException();
        }

        internal static void ChangePassword(Client client, string name, string currentPw, string newPw)
        {
            throw new NotImplementedException();
        }

        internal static void DropItemOnCreature(Client client, byte inventorySlot, uint targetId, byte count)
        {
            throw new NotImplementedException();
        }

        internal static void DropGoldOnCreature(Client client, uint amount, uint targetId)
        {
            throw new NotImplementedException();
        }

        internal static void ProfileRequest(Client client)
        {
            throw new NotImplementedException();
        }

        internal static void GroupRequest(Client client, byte type, string targetName, GroupBox box)
        {
            throw new NotImplementedException();
        }

        internal static void ToggleGroup(Client client)
        {
            throw new NotImplementedException();
        }

        internal static void SwapSlot(Client client, byte pane, byte origSlot, byte endSlot)
        {
            throw new NotImplementedException();
        }

        internal static void RefreshRequest(Client client)
        {
            throw new NotImplementedException();
        }

        internal static void Pursuit(Client client, byte objType, uint objId, uint pursuitId, byte[] args)
        {
            throw new NotImplementedException();
        }

        internal static void DialogResponse(Client client, byte objType, uint objId, ushort pursuitId, ushort dialogId)
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

        internal static void ClickWorldMap(Client client, uint mapId, Point point)
        {
            throw new NotImplementedException();
        }

        internal static void ClickObject(Client client, uint objectId)
        {
            throw new NotImplementedException();
        }

        internal static void ClickObject(Client client, Point clickPoint)
        {
            throw new NotImplementedException();
        }

        internal static void RemoveEquipment(Client client, byte slot)
        {
            throw new NotImplementedException();
        }

        internal static void HeartBeat(Client client, byte a, byte b)
        {
            throw new NotImplementedException();
        }

        internal static void AdjustStat(Client client, Stat stat)
        {
            throw new NotImplementedException();
        }

        internal static void ExchangeWindow(Client client, byte type, uint targetId)
        {
            throw new NotImplementedException();
        }

        internal static void ExchangeWindow(Client client, byte type, uint targetId, byte slot)
        {
            throw new NotImplementedException();
        }

        internal static void ExchangeWindow(Client client, byte type, uint targetId, byte slot, byte count)
        {
            throw new NotImplementedException();
        }

        internal static void ExchangeWindow(Client client, byte type, uint targetId, uint amount)
        {
            throw new NotImplementedException();
        }

        internal static void ExchangeWindow(Client client, byte type)
        {
            throw new NotImplementedException();
        }

        internal static void RequestNotification(Client client)
        {
            throw new NotImplementedException();
        }

        internal static void BeginChant(Client client)
        {
            throw new NotImplementedException();
        }

        internal static void Chant(Client client, string chant)
        {
            throw new NotImplementedException();
        }

        internal static void PortraitText(Client client, ushort totalLength, ushort portraitLength, byte[] portraitData, string profileMsg)
        {
            throw new NotImplementedException();
        }

        internal static void ServerTable(Client client, byte type)
        {
            if (type == 1)
            {
                //send server table
            }
            else
            {
                //redirect to server type
            }
        }

        internal static void RequestHomepage(Client client)
        {
            throw new NotImplementedException();
        }

        internal static void HeartBeatTimer(Client client, TimeSpan serverTicks, TimeSpan clientTicks)
        {
            throw new NotImplementedException();
        }

        internal static void SocialStatus(Client client, SocialStatus status)
        {
            throw new NotImplementedException();
        }

        internal static void MetafileRequest(Client client, bool all)
        {
            throw new NotImplementedException();
        }
    }
}
